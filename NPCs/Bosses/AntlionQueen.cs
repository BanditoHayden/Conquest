
using Conquest.Assets.Systems;
using Conquest.Items.BossBags;
using Conquest.Items.Tile;
using Conquest.Items.Weapons.Magic;
using Conquest.Items.Weapons.Melee;
using Conquest.Items.Weapons.Ranged;
using Conquest.Projectiles.Hostile;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;

namespace Conquest.NPCs.Bosses
{
    [AutoloadBossHead]
    public class AntlionQueen : ModNPC
    {

        public override void SetStaticDefaults()
            {
                NPCID.Sets.BossBestiaryPriority.Add(Type);
                Main.npcFrameCount[Type] = 4;
                NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
                {
                    PortraitScale = 1f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                    PortraitPositionYOverride = 0f,
                };
                NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }
            public override void SetDefaults()
            {
                NPC.width = 160;
                NPC.height = 160;
                NPC.damage = 33;
                NPC.defense = 15;
                NPC.lifeMax = 2525;
                NPC.HitSound = SoundID.NPCHit1;
                NPC.DeathSound = SoundID.NPCDeath1;
                NPC.knockBackResist = 0f;
                NPC.noGravity = true;
                NPC.value = Item.buyPrice(gold: 2);
                NPC.SpawnWithHigherTime(30);
                NPC.boss = true;
                NPC.noTileCollide = true;
                NPC.npcSlots = 10f;
                NPC.aiStyle = -1;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/AntlionQueenTheme");
            }
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.625f * balance);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }
        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.DownedQueen, -1);
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.WorldData);
            }
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Queen1").Type, 1f);
            }
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
            {
                // Sets the description of this NPC that is listed in the bestiary
                bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement("The mother of all antlions")
            });
            }
            private enum Frame
            {
                Frame1,
                Frame2,
                Frame3,
                Frame4
            }
            public bool SpawnedMinions
            {
                get => NPC.localAI[0] == 1f;
                set => NPC.localAI[0] = value ? 1f : 0f;
            }
            public static int MinionCount()
            {
                int count = 7;

                if (Main.expertMode)
                {
                    count += 3;
                }

                if (Main.getGoodWorld)
                {
                    count += 3;
                }

                return count;
            }
            private void makebabiesaesexually()
            {

                int count = MinionCount();
                var entitySource = NPC.GetSource_FromAI();
                AI_Timer++;

                if (AI_Timer >= 30)
                {
                    for (int i = 0; i < count; i++)
                    {
                        int index = NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.GiantFlyingAntlion, NPC.whoAmI);
                        if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, number: index);
                        }
                    }

                    AI_Timer = 0;
                }
                if (AI_Timer >= 35)
                {
                }

            }
            int AI_Timer;
            int Secondstagetimer;
            int bombingtimer;
            int bombsdropped;
            bool secondstage = false;
            public override void AI()
            {

            Player player = Main.player[NPC.target];
            if (player.dead)
            {
                NPC.EncourageDespawn(5);
                NPC.velocity.Y -= 0.04f;
                NPC.EncourageDespawn(10);
                return;
            }
            AI_Timer++;
            dashtimer++;
            if (dashtimer > dashtimermax)
            {
                dashtimer = 0;
            }
            float distance = 200;

            if (dashtimer == 0)
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
            Vector2 toDestination = FirstStageDestination - NPC.Center;
            Vector2 toDestinationNormalized = toDestination.SafeNormalize(Vector2.UnitY);
            
            float speed = Math.Min(distance, toDestination.Length());
            if (secondstage)
            {
                Secondstagetimer++;
                bombingtimer++;
                speed *= 2;
            }
            NPC.velocity = toDestinationNormalized * speed / 30;
           
            if (FirstStageDestination != LastFirstStageDestination)
            {
                NPC.TargetClosest(); 
                

                if (Main.netMode != NetmodeID.Server)
                {
                    NPC.position += NPC.netOffset;

                    //  Dust.QuickDustLine(NPC.Center + toDestinationNormalized * NPC.width, FirstStageDestination, toDestination.Length() / 20f, Color.Yellow);

                    NPC.position -= NPC.netOffset;
                }
            }
            LastFirstStageDestination = FirstStageDestination;

          
            if (NPC.life <= (NPC.lifeMax / 2))
            {
                secondstage = true;
            }
        
            if (AI_Timer == 320 && !secondstage)
            {
                Player target = Main.player[NPC.target];
                Vector2 ToPlayer = NPC.DirectionTo(target.Center) * 10;
                NPC.velocity = ToPlayer;
                Vector2 position1 = NPC.position;
                Vector2 position2 = target.position + new Vector2(200, 30);
                Vector2 position3 = target.position + new Vector2(-200, 30);
                Projectile.NewProjectile(NPC.GetSource_FromThis(), position1, -Vector2.UnitX, ProjectileID.SandnadoHostileMark, NPC.damage / 2, 0f, Main.myPlayer);
                if (NPC.direction == 1)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), position2, -Vector2.UnitX, ProjectileID.SandnadoHostileMark, NPC.damage / 2, 0f, Main.myPlayer);

                }
                if (NPC.direction == -1)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), position3, -Vector2.UnitX, ProjectileID.SandnadoHostileMark, NPC.damage / 2 , 0f, Main.myPlayer);

                }
            }
            if (AI_Timer == 500 && !secondstage)
            {
                Vector2 direction = Main.player[NPC.target].Center - NPC.Center;
                direction.Normalize();
                direction *= 12f;
                const int AmountOfProjectiles = 12;

                for (int i = 0; i < AmountOfProjectiles; ++i)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, direction.RotatedByRandom(MathHelper.Pi / 4) * Main.rand.NextFloat(0.5f, 1f), ModContent.ProjectileType<SandFall>(), NPC.damage / 3, 1, Main.myPlayer, 0, 0);
                }
            }
            if (AI_Timer == 630 && !secondstage)
            {
                Vector2 direction = Main.player[NPC.target].Center - NPC.Center;
                direction.Normalize();
                direction *= 12f;
                const int AmountOfProjectiles = 3;
                    for (int i = 0; i < AmountOfProjectiles; ++i)
                    {

                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, direction.RotatedByRandom(MathHelper.Pi / 4) * Main.rand.NextFloat(0.5f, 1f), ModContent.ProjectileType<SandExplosion>(), NPC.damage / 3, 1, Main.myPlayer, 0, 0);
                    }
              
            }
            if (AI_Timer == 880)
            {
                AI_Timer = 0;
            }
            if (bombingtimer >= 1)
            {

                Player target = Main.player[NPC.target];
                int offsetX = Main.rand.Next(-200, 200) * 6;
                if (bombingtimer >= 400)
                {
                    if (Main.rand.NextBool(6))
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), target.Center.X + offsetX, target.Center.Y - 1200, 0f, 5f, ModContent.ProjectileType<SandExplosion>(), NPC.damage / 3, 1, Main.myPlayer, 0, 0);
                        bombsdropped++;
                    }
                }
                if (bombsdropped == 5)
                {
                    bombsdropped = 0;
                    bombingtimer -= 399;
                }
            }
            if(Secondstagetimer == 240)
            {
                Vector2 direction = Main.player[NPC.target].Center - NPC.Center;
                direction.Normalize();
                direction *= 14f;
                const int AmountOfProjectiles = 2;
                for (int i = 0; i < AmountOfProjectiles; ++i)
                {
                  Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, direction, ModContent.ProjectileType<HelixSand>(), NPC.damage / 2, 1, Main.myPlayer, 0, 0);
                }
            }
            if (Secondstagetimer == 500)
            {
                Vector2 direction = Main.player[NPC.target].Center - NPC.Center;
                direction.Normalize();
                direction *= 12f;
                const int AmountOfProjectiles = 3;
                for (int i = 0; i < AmountOfProjectiles; ++i)
                {

                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, direction.RotatedByRandom(MathHelper.Pi / 4) * Main.rand.NextFloat(0.5f, 1f), ModContent.ProjectileType<HelixSand>(), NPC.damage / 3, 1, Main.myPlayer, 0, 0);
                }
            }
            if (Secondstagetimer == 800)
            {
                Player target = Main.player[NPC.target];
                Vector2 ToPlayer = NPC.DirectionTo(target.Center) * 10;
                NPC.velocity = ToPlayer;
                Vector2 position1 = NPC.position;
                Vector2 position2 = target.position + new Vector2(200, 30);
                Vector2 position3 = target.position + new Vector2(-200, 30);
                Projectile.NewProjectile(NPC.GetSource_FromThis(), position1, -Vector2.UnitX, ProjectileID.SandnadoHostileMark, NPC.damage /2, 0f, Main.myPlayer);
                if (NPC.direction == 1)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), position2, -Vector2.UnitX, ProjectileID.SandnadoHostileMark, NPC.damage / 2, 0f, Main.myPlayer);

                }
                if (NPC.direction == -1)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), position3, -Vector2.UnitX, ProjectileID.SandnadoHostileMark, NPC.damage / 2, 0f, Main.myPlayer);

                }
                Secondstagetimer = 0;
            }
            

        }
        public override void FindFrame(int frameHeight)
            {
                NPC.frameCounter++;
                NPC.spriteDirection = NPC.direction;

                if (NPC.frameCounter < 3)
                {
                    NPC.frame.Y = (int)Frame.Frame1 * frameHeight;
                }
                else if (NPC.frameCounter < 6)
                {
                    NPC.frame.Y = (int)Frame.Frame2 * frameHeight;
                }
                else if (NPC.frameCounter < 9)
                {
                    NPC.frame.Y = (int)Frame.Frame3 * frameHeight;
                }
                else if (NPC.frameCounter < 12)
                {
                    NPC.frame.Y = (int)Frame.Frame3 * frameHeight;
                }
                else
                {
                    NPC.frameCounter = 0;
                }

            }

        
        int dashtimer;
        private const int dashtimermax = 90;
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
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<QueenBag>()));
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<QueenRelicItem>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<QueenTrophyItem>(), 10));
            npcLoot.Add(ItemDropRule.OneFromOptions(1, ModContent.ItemType<GloriousScepter>(), ModContent.ItemType<GloriousLauncher>(), ModContent.ItemType<GloriousSpear>()));
        }
        public override void BossLoot(ref string name, ref int potionType) => potionType = ItemID.GreaterHealingPotion;





    }
}

