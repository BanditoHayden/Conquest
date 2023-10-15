using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Security.Cryptography;
using Terraria.ID;
using Terraria.Audio;
using SubworldLibrary;
using Terraria.WorldBuilding;
using Terraria.IO;
using StructureHelper;
using Terraria.DataStructures;
using Conquest.Subworlds;
using Conquest.Items.Weapons.Melee;
using Point16 = Terraria.DataStructures.Point16;
using Terraria.GameContent;
using static Terraria.Player;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using static Conquest.Assets.Common.CinematicSystem;
using Terraria.ObjectData;

namespace Conquest.Assets.Common
{
    // Sample Activation Method
    //
    // ArenaSpawnInfo info = new ArenaSpawnInfo(new List<int[]>(), 0); <- initialize new spawn info
    // info.Enemies.Add(new int[]{ NPCID.Zombie, NPCID.Zombie, NPCID.Zombie, NPCID.Zombie}); <- add wave 1
    // ModContent.GetInstance<ArenaSystem>().ActivateArena(player.Center, info, new Vector2[]{ player.Center, player.Center + Vector2.UnitX * 150, player.Center - Vector2.UnitX * 150 }, 50 * 16, 50 * 16);
public class ArenaSystem : ModSystem
	{
        private bool isActive = false;

        private const float LightMax = 1000;
        private const float SwingInit = MathHelper.PiOver4/3;
        private const float SwingRange = MathHelper.Pi/2;
        private const float SwingTime = 120;

        private int maxNPCs = 3; //maximum number of npcs allowed at once (-1 = unlimited)
        private ArenaSpawnInfo spawnInfo; //down
        private Point[] ActivationWires; //coords of wires to activate
        private Vector2 arenaCenter; //center world coord of the arena
        private Vector2[] spawnPoints; //spawning positions of the arena
        private Rectangle arenaArea; //dimensions of arena
        private List<int> NPCs = new List<int>(); //active npc.whoami
        private int currentWave = 0; //yeah
        private int SpawnQueueIndex = 0; //yeah
        private int timer = 0; //main timer
        private int spawnTimer; //cooldown in ticks between waves
        private int spawnLocation; //cycles through spawners
        private bool wavesCompleted = false;
        //hi goose

        public override void NetSend(BinaryWriter writer)
        {
            base.NetSend(writer);
        }
        public override void NetReceive(BinaryReader reader)
        {
            base.NetReceive(reader);
        }//TODO sync

        private void UpdateLights()
		{
            Vector2 LeftLight = arenaCenter + new Vector2(-10, -20) * 16;
            Vector2 RightLight = arenaCenter + new Vector2(10, -20) * 16;

            float rotation = (float)((SwingRange - SwingInit) * Math.Sin(MathHelper.Pi * timer/SwingTime));
            Vector2 direction = Vector2.UnitX.RotatedBy(rotation + MathHelper.PiOver2);
            Vector2 direction2 =  Vector2.UnitX.RotatedBy(-rotation + MathHelper.PiOver2);

            DelegateMethods.v3_1 = Color.Purple.ToVector3() * 2;
            Utils.PlotTileLine(LeftLight, LeftLight + direction * PerformBeamHitscan(LeftLight, direction), 1, new Utils.TileActionAttempt(DelegateMethods.CastLight));
            Utils.PlotTileLine(RightLight, RightLight + direction2 * PerformBeamHitscan(RightLight, direction2), 1, new Utils.TileActionAttempt(DelegateMethods.CastLight));
        }
        private float PerformBeamHitscan(Vector2 lightPos, Vector2 direction)
        {
            Vector2 samplingPoint = lightPos;
            float[] laserScanResults = new float[3];
            Collision.LaserScan(samplingPoint, direction, 1, LightMax, laserScanResults);
            float averageLengthSample = 0f;
            for (int i = 0; i < laserScanResults.Length; ++i)
            {
                averageLengthSample += laserScanResults[i];
            }
            averageLengthSample /= 3;

            return averageLengthSample;
        }
        private void closeDoors()
        {
            //actuate doors or otherwise block the player from leaving while arena is active
        }
        private void UpdateSpawning()
        {
            
            for (int i = NPCs.Count(); i < maxNPCs; i++)
            {
                if (SpawnQueueIndex < spawnInfo.Enemies[currentWave].Count() && --spawnTimer <= 0)
                {
                    Vector2 spawnPos = spawnPoints[spawnLocation];
                    int Type = spawnInfo.Enemies[currentWave][SpawnQueueIndex];
                    int a = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), ((int)spawnPos.X), ((int)spawnPos.Y + ContentSamples.NpcsByNetId[Type].height/2), Type);
                    NPCs.Add(a);
                    SpawnQueueIndex += 1;
                    if (spawnLocation++ == spawnPoints.Count()-1)
                    {
                        spawnLocation = 0;
                    }
                }
            }
            if (SpawnQueueIndex > spawnInfo.Enemies[currentWave].Count()-1 && NPCs.Count() == 0 && currentWave <= spawnInfo.numWaves + 1)
            {
                queueNextWave();
            } else if (SpawnQueueIndex > spawnInfo.Enemies[currentWave].Count() - 1 && NPCs.Count() == 0 && currentWave > spawnInfo.numWaves)
            {
                wavesCompleted = !!!!!!!false; //true
            }
            List<int> ints = new List<int>();
            foreach (int i in NPCs)
            {
                var a = Main.npc[i];
                if (!a.active)
                {
                    ints.Add(i);
                }
            }
            foreach (var i in ints)
            {
                if (NPCs.Contains(i))
                    NPCs.Remove(i);
            }
        }
        void queueNextWave()
        {
            spawnTimer = 360;
            currentWave += 1;
            SpawnQueueIndex = 0;
        }
        /// <summary>
        /// Creates arena effects and spawns enemies
        /// </summary>
        /// <param name="spawns">SpawnInfo, use SpawnInfo.none to prevent spawning</param>
        /// <param name="ArenaCenter">The center tile coord of the arena</param>
        /// <param name="spawners">List of positions that enemies will spawn on top of relative to the arena's center</param>
        /// <param name="wires">Coords of wires that activate when this method is called</param>
        public void ActivateArena(Vector2 ArenaCenter, ArenaSpawnInfo spawns, Vector2[] spawners = null, int width = 0, int height = 0, Point[] wires = null)
        {
            spawnInfo = spawns;
            arenaCenter = ArenaCenter;
            isActive = true;
            arenaArea = new Rectangle((int)(arenaCenter.X - width / 2), (int)(arenaCenter.Y - height / 2), width, height);
            spawnPoints = spawners;
            ActivationWires = wires;
            timer = 0;
            wavesCompleted = false;
        }
        public override void PreUpdateWorld()
        {
            var player = Main.LocalPlayer;
            var ArenaPlayer = Main.LocalPlayer.GetModPlayer<ArenaPlayer>();
            var Cinematic = GetInstance<CinematicSystem>();
            if (isActive)
            {
                /*
                foreach (var wire in ActivationWires)
                {
                    //Wiring.TripWire(wire.X, wire.Y, 1, 1);
                    
                }*/
                if (timer == 0)
                {
                    Cinematic.PanCamera(player.Center, player.Center + Vector2.UnitX * 160, 300, 120, 300, EasingStyle.easeInOut, true);
                }

                UpdatePlayer();
                UpdateSpawning();
                UpdateLights();
                

                if (timer >= 6000)
                {
                    closeArena();
                }
                if (wavesCompleted)
                {
                    closeArena();
                }
            }

            timer++;
        }
        public void UpdatePlayer()
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (arenaArea.Contains(((int)Main.player[i].Center.X), ((int)Main.player[i].Center.Y)))
                {
                    Main.player[i].GetModPlayer<ArenaPlayer>().inArena = true;
                }
            }
        }
        public void closeArena()
        {

            //reset effects
            Main.LocalPlayer.GetModPlayer<ArenaPlayer>().inArena = false;
            isActive = false;
            foreach (int nPC in NPCs)
            {
                var a = Main.npc[nPC];
                a.StrikeInstantKill();
            }
            NPCs.Clear();
            currentWave = 0;
            SpawnQueueIndex = 0;
        }
    }
    public class ArenaPlayer : ModPlayer
    {
        public bool inArena = false;
        public bool cinematic = false;
        public override bool CanUseItem(Item item)
        {
            //block items here
            return base.CanUseItem(item);
        }
        public override void PreUpdate()
        {
        }
        public override void PostUpdate()
        {
            inArena = false;
            base.PostUpdate();
        }
    }
    public class ArenaNPC : GlobalNPC
    {
        private bool isArenaEnemy;
        public override bool InstancePerEntity => true;

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (source != null)
            {
                if (source.Context == "Conquest/ArenaEnemy")
                {
                    isArenaEnemy = true;
                    npc.dontCountMe = true;
                }
            }
        }
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (isArenaEnemy)
            {
                foreach (var i in npcLoot.Get())
                {
                    npcLoot.Remove(i);
                }
            }
        }
        public override bool CheckActive(NPC npc)
        {
            if (isArenaEnemy)
            {
                return false;
            }
            return base.CheckActive(npc);
        }
    }
    public class FromArena : IEntitySource
    {
        public string Context => "Conquest/ArenaEnemy";
    }
    /// <summary>
    /// use ArenaSpawnInfo.none for no spawning
    /// </summary>
    public struct ArenaSpawnInfo
    {
        public List<int[]> Enemies;
        readonly public int numWaves;
        public float waveTime;

        /// <summary>
        /// Contains information about enemy spawning in an arena
        /// </summary>
        /// <param name="enemies">An array of int arrays containing the NPCIDs of the spawned enemies in each wave in spawning order </param>
        /// <param name="wavetime">The duration of each wave in seconds</param>
        public ArenaSpawnInfo(List<int[]> enemies, float wavetime)
        {
            Enemies = enemies;
            waveTime = wavetime;

            numWaves = enemies.Count();
        }

        public static ArenaSpawnInfo none => new ArenaSpawnInfo(new List<int[]>(),0); //empty spawn info
    }
    [Autoload(Side = ModSide.Client)] // This attribute makes this class only load on a particular side. Naturally this makes sense here since UI should only be a thing clientside. Be wary though that accessing this class serverside will error
    public class CinematicSystem : ModSystem
    {
        private int UIHideTimer;
        private Vector2 oldMouseScreen;
        private int stillCursor;

        public override void PostUpdatePlayers()
        {
            if (Main.MouseScreen == oldMouseScreen)
            {
                stillCursor++;
            }
            else
            {
                stillCursor = 0;
            }
            oldMouseScreen = Main.MouseScreen;
        }
        public override void UpdateUI(GameTime gameTime)
        {
            
            if (UIHideTimer > 0)
                --UIHideTimer;
        }

        // Adding a custom layer to the vanilla layer list that will call .Draw on your interface if it has a state
        // Setting the InterfaceScaleType to UI for appropriate UI scaling
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            if (UIHideTimer > 0 || UIHideTimer == -1)
                foreach (var layer in layers)
                {
                    if (layer.Name.ToLower() == "vanilla: cursor" && stillCursor > 60)
                    {
                        layer.Active = false;
                    } else { continue; }
                    if (layer.Name.ToLower() != "vanilla: settings button" || layer.Name.ToLower() == "vanilla: ingame options")
                        layer.Active = false;
                }
        }
        /// <summary>
        /// Pans your camera from the start position to the end position.
        /// <br/>
        /// Dont start a new <see cref="PanCameraModifier"/> before the last one is finished or the old one will be interuppted.
        /// </summary>
        /// <param name="startPosition">World Postion</param>
        /// <param name="endPostion">World Postion</param>
        /// <param name="framesToEnd">How long it takes to pan to the <paramref name="endPostion"/> in frames (1 frame = 1/60 second)</param>
        /// <param name="FramesToRest">How long the camera rests at the <paramref name="endPostion"/> in frames (1 frame = 1/60 second)</param>
        /// <param name="FramesToReturn">How long it takes to pan from the <paramref name="endPostion"/> to the <paramref name="startPosition"/> in frames (1 frame = 1/60 second)</param>
        public void PanCamera(Vector2 startPosition, Vector2 endPostion, int framesToEnd, int FramesToRest, int FramesToReturn, EasingStyle style, bool hideUI = false)
        {
            UIHideTimer += framesToEnd + FramesToRest + FramesToReturn;
            var a = new PanCameraModifier(startPosition - Main.screenPosition, endPostion - Main.screenPosition, framesToEnd, FramesToReturn, FramesToRest, style, FullName);
            Main.instance.CameraModifiers.Add(a);
        }
        public void ShakeCamera(Vector2 start, float strength, int frames)
        {
            PunchCameraModifier modifier = new PunchCameraModifier(start, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), strength, 6f, frames, 1000f, FullName);
            Main.instance.CameraModifiers.Add(modifier);
        }

        /// <summary>
        /// Hide all UI for <paramref name="duration"/> frames. Leave at -1 for indefinent.
        /// </summary>
        /// <param name="duration"></param>
        public void HideAllUI(int duration = -1)
        {
            UIHideTimer = duration;
        }
        /// <summary>
        /// Shows all UI if UI is hidden.
        /// </summary>
        public void ShowAllUI()
        {
            UIHideTimer = 0;
        }
        /// <summary>
        /// Easing function types for panning 
        /// </summary>
        public enum EasingStyle
        {
            linear,
            easeIn,
            easeOut,
            easeInOut,
        }
    }
    public class PanCameraModifier : ICameraModifier
    {
        private int _goFrames;

        private int _returnFrames;

        private int _framesLasted;

        private int _focusFrames;

        private EasingStyle _style;

        private Vector2 _startPosition;

        private Vector2 _endPosition;

        public string UniqueIdentity { get; private set; }

        public bool Finished { get; private set; }

        public PanCameraModifier(Vector2 startPosition, Vector2 endPosition, int goFrames, int returnFrames, int focusFrames, EasingStyle style = EasingStyle.linear, string uniqueIdentity = null)
        {
            _startPosition = startPosition;
            _endPosition = endPosition;
            _goFrames = goFrames;
            _returnFrames = returnFrames;
            _focusFrames = focusFrames;
            _style = style;

            UniqueIdentity = uniqueIdentity;
        }
        public float Ease(float value)
        {
            switch (_style)
            {
                case EasingStyle.linear:
                    return value;
                case EasingStyle.easeInOut:
                    if (value <= 0.5f)
                        return 2.0f * value * value;
                    value -= 0.5f;
                    return 2.0f * value * (1.0f - value) + 0.5f;
                default:
                    return value;
            }
        }
        public void Update(ref CameraInfo cameraInfo)
        {
            Vector2 direction = _startPosition.DirectionTo(_endPosition);
            float distance = _startPosition.Distance(_endPosition);
            float num1 = Ease((float)_framesLasted / _goFrames);
            float num2 = 1-Ease((float)(_framesLasted - _goFrames - _focusFrames) / _returnFrames);
            ref Vector2 cameraPosition = ref cameraInfo.CameraPosition;
            if (_framesLasted <= _goFrames)
            {
                cameraPosition += direction * distance * num1;
            }
            if (_framesLasted > _goFrames && _framesLasted <= _goFrames + _focusFrames)
            {
                cameraPosition += direction * distance;
            }
            if (_framesLasted > _goFrames + _focusFrames)
            {
                cameraPosition += direction * distance * num2;
            }

            _framesLasted++;
            if (_framesLasted >= _goFrames + _focusFrames + _returnFrames)
            {
                Finished = true;
            }
        }
    }
}

