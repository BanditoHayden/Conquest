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

        private int maxNPCs = 5; //maximum number of npcs allowed at once (-1 = unlimited)
        private ArenaSpawnInfo spawnInfo;
        private Vector2 arenaCenter; //center world coord of the arena
        private Vector2[] spawnPoints; //spawning positions of the arena
        private Rectangle arenaArea; //dimensions of arena
        private List<int> NPCs = new List<int>(); //active npc.whoami
        private int currentWave = 0; //yeah
        private int SpawnQueueIndex = 0; //yeah
        private int timer;
        private int spawnTimer; //god please sync
        private int spawnLocation;

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
                    int a = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), ((int)spawnPos.X), ((int)spawnPos.Y), Type);
                    NPCs.Add(a);
                    SpawnQueueIndex += 1;
                    if (spawnLocation++ == spawnPoints.Count()-1)
                    {
                        spawnLocation = 0;
                    }
                }
            }
            Main.NewText(string.Join(", ", NPCs) + ", " + NPCs.Count().ToString());
            if (SpawnQueueIndex > spawnInfo.Enemies[currentWave].Count() && --spawnTimer <= 0)
            {
                //queueNextWave();
            }
            foreach (int i in NPCs)
            {
                var a = Main.npc[i];
            }
        }
        void queueNextWave()
        {
            spawnTimer = 240;
            currentWave += 1;
            SpawnQueueIndex = 0;
        }
        /// <summary>
        /// Creates arena effects and spawns enemies
        /// </summary>
        /// <param name="spawns">SpawnInfo, use SpawnInfo.none to prevent spawning</param>
        /// <param name="ArenaCenter">The center tile coord of the arena</param>
        /// <param name="spawners">List of points of blocks which enemies will spawn on relative to the arena's center</param>
        public void ActivateArena(Vector2 ArenaCenter, ArenaSpawnInfo spawns, Vector2[] spawners = null, int width = 0, int height = 0)
        {
            spawnInfo = spawns;
            arenaCenter = ArenaCenter;
            isActive = true;
            arenaArea = new Rectangle((int)(arenaCenter.X - width / 2), (int)(arenaCenter.Y - height / 2), width, height);
            spawnPoints = spawners;
        }
        public override void PreUpdateWorld()
        {
            
            if (isActive)
            {
                
                UpdatePlayer();
                UpdateSpawning();
                UpdateLights();
                
                timer++;

                if (timer >= 600)
                {
                    closeArena();
                    timer = 0;
                    
                }
            }
            base.PreUpdateWorld();
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
            Main.NewText("deactivate");
        }
    }
    public class ArenaPlayer : ModPlayer
    {
        public bool inArena = false;
        public override bool CanUseItem(Item item)
        {
            if (item.type == ItemID.RodofDiscord && inArena)
            {
                return false;
            }
            return base.CanUseItem(item);
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
	    if (source != null) {
            	if (source.Context == "Conquest/ArenaEnemy")
            	{
                    isArenaEnemy = true;
            	}
	    }
            base.OnSpawn(npc, source);
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
        public int numWaves;
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
}

