using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Bestiary;
using System.IO;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Conquest.Items.Weapons.Melee;
namespace Conquest.NPCs.Miniboss.Beholder
{
    public class BeholderEnemy : ModNPC
    {

        public override void SetDefaults()
        {
            NPC.width = 132;
            NPC.height = 122;
            NPC.damage = 20;
            NPC.defense = 30;
            NPC.lifeMax = 1200;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 500f;
            NPC.knockBackResist = 0f;
            NPC.noTileCollide = true;
        }
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 12;
        }
        public Vector2 FirstStageDestination
        {
            get => new Vector2(NPC.ai[1], NPC.ai[2]);
            set
            {
                NPC.ai[1] = value.X;
                NPC.ai[2] = value.Y;
            }
        }
        public Vector2 LastFirstStageDestination { get; set; } = Vector2.Zero;
        public ref float FirstStageTimer => ref NPC.localAI[1];
        private const int FirstStageTimerMax = 90;

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }
            if (player.dead)
            {
                NPC.velocity.Y -= 0.04f;
                NPC.EncourageDespawn(10);
                return;
            }
            DoFirstStage(player);

        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (NPC.CountNPCS(ModContent.NPCType<BeholderEnemy>()) != 2 && NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
            {
                return SpawnCondition.Underworld.Chance * 0.001f;
            }
            else 
            return SpawnCondition.Underworld.Chance * 0f;
        }
        private void DoFirstStage(Player player)
        {
            FirstStageTimer++;
            if (FirstStageTimer > FirstStageTimerMax)
            {
                FirstStageTimer = 0;
            }

            float distance = 300; 

            if (FirstStageTimer == 0)
            {
                Vector2 fromPlayer = NPC.Center - player.Center;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {

                    float angle = fromPlayer.ToRotation();
                    float twelfth = MathHelper.Pi / 6;

                    angle += MathHelper.Pi + Main.rand.NextFloat(-twelfth, twelfth);
                    if (angle > MathHelper.TwoPi)
                    {
                        angle -= MathHelper.TwoPi;
                    }
                    else if (angle < 0)
                    {
                        angle += MathHelper.TwoPi;
                    }

                    Vector2 relativeDestination = angle.ToRotationVector2() * distance;

                    FirstStageDestination = player.Center + relativeDestination;
                    NPC.netUpdate = true;
                }
            }

            // Move along the vector
            Vector2 toDestination = FirstStageDestination - NPC.Center;
            Vector2 toDestinationNormalized = toDestination.SafeNormalize(Vector2.UnitY);
            float speed = Math.Min(distance, toDestination.Length());
            NPC.velocity = toDestinationNormalized * speed / 30;

            if (FirstStageDestination != LastFirstStageDestination)
            {
                NPC.TargetClosest(); 

                if (Main.netMode != NetmodeID.Server)
                {
                    NPC.position += NPC.netOffset;
                    NPC.position -= NPC.netOffset;
                }
            }
            LastFirstStageDestination = FirstStageDestination;
        }
        private enum Frame
        {
            Frame1,
            Frame2,
            Frame3,
            Frame4,
            Frame5,
            Frame6,
            Frame7,
            Frame8,
            Frame9,
            Frame10,
            Frame11,
            Frame12,

        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            NPC.spriteDirection = NPC.direction;

            if (NPC.frameCounter < 5)
            {
                NPC.frame.Y = (int)Frame.Frame1 * frameHeight;
            }
            else if (NPC.frameCounter < 10)
            {
                NPC.frame.Y = (int)Frame.Frame2 * frameHeight;
            }
            else if (NPC.frameCounter < 15)
            {
                NPC.frame.Y = (int)Frame.Frame3 * frameHeight;
            }
            else if (NPC.frameCounter < 20)
            {
                NPC.frame.Y = (int)Frame.Frame4 * frameHeight;
            }
            else if (NPC.frameCounter < 25)
            {
                NPC.frame.Y = (int)Frame.Frame5 * frameHeight;
            }
            else if (NPC.frameCounter < 30)
            {
                NPC.frame.Y = (int)Frame.Frame6 * frameHeight;
            }
            else if (NPC.frameCounter < 35)
            {
                NPC.frame.Y = (int)Frame.Frame7 * frameHeight;

            }
            else if (NPC.frameCounter < 40)
            {
                NPC.frame.Y = (int)Frame.Frame8 * frameHeight;
            }
            else if (NPC.frameCounter < 45)
            {
                NPC.frame.Y = (int)Frame.Frame9 * frameHeight;
            }
            else if (NPC.frameCounter < 50)
            {
                NPC.frame.Y = (int)Frame.Frame10 * frameHeight;
            }
            else if (NPC.frameCounter < 55)
            {
                NPC.frame.Y = (int)Frame.Frame11 * frameHeight;
            }
            else if (NPC.frameCounter < 60)
            {
                NPC.frame.Y = (int)Frame.Frame12 * frameHeight;
            }
            else
            {
                NPC.frameCounter = 0;
            }

        }
    }

    }

