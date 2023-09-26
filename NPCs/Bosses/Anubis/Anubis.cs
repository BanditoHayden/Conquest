using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Conquest.Items.BossBags;
using Terraria.GameContent.ItemDropRules;
using Conquest.Items.Weapons.Ranged;
using Conquest.Items.Weapons.Magic;
using Conquest.Items.Weapons.Summon;
using Conquest.Items.Weapons.Melee;
using Terraria.Audio;
using Conquest.Projectiles.Summoner;
using Conquest.Assets.Systems;

namespace Conquest.NPCs.Bosses.Anubis
{
    [AutoloadBossHead]
    public class Anubis : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 9;
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Velocity = 1f,
                Direction = -1

            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Desert,

                new FlavorTextBestiaryInfoElement("..."),

            });
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<AnubisBossBag>()));
            npcLoot.Add(ItemDropRule.OneFromOptions(1, ModContent.ItemType<AnubisCannon>(), ModContent.ItemType<AnubisStaff>(), ModContent.ItemType<EmeraldSlate>(), ModContent.ItemType<AnubisHammer>()));
        }
        private enum ActionState
        {
            Notice,
            Move,
            Attack,
            Move2,
            Attack2,
            Move3,
            Attack3,
            Move4,
            Attack4,
            Move5,
            Attack5,
            Move6,
            Attack6,
            Heal,
            SecondStage1,
            SecondStage2,
            SecondStage3,
            SecondStage4,
            SecondStage5,
            SecondStage6,
            SecondStage7,
            SecondStage8,
            SecondStage9,
            SecondStage10,
            SecondStage11,
            SecondStage12,
        }
        private enum Frame
        {
            Move1,
            Move2,
            Move3,
            Attack1,
            Attack2,
            Attack3,
            Attack4,
            Attack5,
            Attack6,
        }


        int timesGoneWithoutHeal = 0;
        int timesGoneWithoutHealMax = 9;
        int healsDone = 0;
        int maxHeals = 3;
        public void ChooseMove()
        {
            if (SecondStage && timesGoneWithoutHeal > timesGoneWithoutHealMax && healsDone < maxHeals)
            {
                AI_State = (float)ActionState.Heal;
                timesGoneWithoutHeal = 0;
                healsDone++;
            }
            else
            {
                if (SecondStage) timesGoneWithoutHeal++;
                if (Main.rand.NextBool(6)) AI_State = (float)ActionState.Move6;
                else if (Main.rand.NextBool(5)) AI_State = (float)ActionState.Move5;
                else if (Main.rand.NextBool(4)) AI_State = (float)ActionState.Move4;
                else if (Main.rand.NextBool(3)) AI_State = (float)ActionState.Move3;
                else if (Main.rand.NextBool()) AI_State = (float)ActionState.Move2;
                else AI_State = (float)ActionState.Move;
            }
            AI_Timer = 0;
        }

        public override void SetDefaults()
        {
            NPC.width = 72;
            NPC.height = 116 ;
            NPC.damage = 66;
            NPC.defense = 25;
            NPC.lifeMax = 45000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath9;
            NPC.value = 60f;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = 0;
            NPC.lavaImmune = true; 
            NPC.noGravity = true;
            NPC.boss = true;
            NPC.noTileCollide = true;
          
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/AnubisTheme");
            }
        }
        public ref float AI_State => ref NPC.ai[0];
        public ref float AI_Timer => ref NPC.ai[1];
        public bool InGuardingState;
       
        private void Notice()
        {
            if (Main.player[NPC.target].Distance(NPC.Center) < 1250f)
            {
                AI_Timer++;

                if (AI_Timer >= 20)
                {
                    ChooseMove();
                    AI_Timer = 0;
                }
            }
            else
            {
                NPC.TargetClosest(true);

                if (!NPC.HasValidTarget || Main.player[NPC.target].Distance(NPC.Center) > 500f)
                {
                    ChooseMove();
                    AI_Timer = 0;
                }
            }
        }
        
        private void Move()
        {
            AI_Timer++;
            if (AI_Timer >= 60)
            {
                AI_State = (float)ActionState.Attack;
                AI_Timer = 0;
            }

        }
        private void Attack()
        {
            NPC.velocity = Vector2.Zero;
            if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
            {
                AI_Timer++;
                var source = NPC.GetSource_FromAI();
                Vector2 position = NPC.Center;
                Vector2 targetPosition = Main.player[NPC.target].Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                float speed = 14f;
                int type = ModContent.ProjectileType<AnubisChaser>();
                int damage = NPC.damage;
                if (AI_Timer == 31)
                {
                    SoundEngine.PlaySound(Begone, NPC.position);
                }
                if (AI_Timer >= 31 && AI_Timer % 9 == 0)
                {
                    if (AI_Timer % 9 == 0) SoundEngine.PlaySound(SoundID.Item124, NPC.Center);
                    Projectile.NewProjectile(source, position, direction * speed, type, damage / 5, 0f, Main.myPlayer);
                }
                if (AI_Timer >= 61)
                {
                    AI_Timer = 0;
                    ChooseMove();
                }

            }

        }
        private void Move2()
        {
            AI_Timer++;
            if (AI_Timer >= 90)
            {
                AI_State = (float)ActionState.Attack2;
                AI_Timer = 0;
            }
        }
        SoundStyle Begone = new SoundStyle($"{nameof(Conquest)}/Assets/Sounds/Begone");
        SoundStyle Fire = new SoundStyle($"{nameof(Conquest)}/Assets/Sounds/Fire");
        SoundStyle Arise = new SoundStyle($"{nameof(Conquest)}/Assets/Sounds/Arise");
        SoundStyle Pierce = new SoundStyle($"{nameof(Conquest)}/Assets/Sounds/Pierce");
        SoundStyle Guard = new SoundStyle($"{nameof(Conquest)}/Assets/Sounds/Guard");

        private void Attack2()
        {
            AI_Timer++;
            Player target = Main.player[NPC.target];
            // Credit to PaperLuigi
            float Speed = 3f;  
            Vector2 StartPosition = new Vector2(NPC.Center.X, NPC.Center.Y - 800);
            int damage = 30;
            int type = ModContent.ProjectileType<AnubisBeam>();

            float rotation = (float)Math.Atan2(StartPosition.Y - (target.position.Y + (target.height * 0.5f)), StartPosition.X - (target.position.X + (target.width * 0.5f)));
            Vector2 velocity = new Vector2((float)((Math.Cos(rotation) * Speed) * -1), (float)((Math.Sin(rotation) * Speed) * -1));
            if(AI_Timer == 51)
            {
                SoundEngine.PlaySound(Arise, NPC.position);
            }
            if (AI_Timer >= 61)
            {
                SoundEngine.PlaySound(SoundID.Item124, NPC.Center);
                for (int d = 0; d < 30; d++)
                {
                    Dust.NewDust(NPC.Center, 0, 0, DustID.BubbleBurst_Purple, 0f + Main.rand.Next(-20, 20), 0f + Main.rand.Next(-20, 20), 150, default(Color), 1.5f);
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float numberProjectiles = 12;
                    float adjustedRotation = MathHelper.ToRadians(40);
                    for (int i = 0; i < numberProjectiles; i++)
                    {
                        Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-adjustedRotation, adjustedRotation, i / (numberProjectiles - 1))) * .2f; // Watch out for dividing by 0 if there is only 1 projectile.
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), StartPosition.X, StartPosition.Y, perturbedSpeed.X * 5, perturbedSpeed.Y * 5, type, damage, 0, Main.myPlayer);
                    }
                }

                AI_Timer = 0;
                ChooseMove();
            }
           
        }
        private void Move3()
        {

            AI_Timer++;
            if (AI_Timer >= 90)
            {
                AI_State = (float)ActionState.Attack3;
                AI_Timer = 0;
            }
        }
        private void Attack3()
        {
            
            AI_Timer++;
            if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
            {
                AI_Timer++;
                var source = NPC.GetSource_FromAI();
                Vector2 position = NPC.Center + new Vector2(0, -NPC.height * 1.5f);
                int type = ModContent.ProjectileType<AnubisBouncyBeam>();
                int damage = NPC.damage;
                if (AI_Timer >= 1 && AI_Timer % 20 == 0)
                {
                    for (float angle = MathF.PI / 4f; angle < 2 * MathF.PI; angle += MathF.PI / 2f)
                    {
                        Vector2 vel = new Vector2(15, 0).RotatedBy(angle);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), position, vel, ModContent.ProjectileType<AnubisBouncyBeam>(), NPC.damage / 5, 1, Main.myPlayer, 0, 0);
                    }
                }
                if (AI_Timer >= 61)
                {
                    AI_Timer = 0;
                    ChooseMove();
                }
            }
        }
        private void Move4()
        {
            AI_Timer++;
            if (AI_Timer >= 180)
            {
                AI_State = (float)ActionState.Attack4;
                AI_Timer = 0;
            }
        }

        int attack4Offset = 100;
        private void Attack4()
        {
            AI_Timer++;
            Player target = Main.player[NPC.target];
            
            if (AI_Timer == 1)
            {
                SoundEngine.PlaySound(Arise, NPC.position);
                attack4Offset = NPC.direction * 100;
            }
            if (AI_Timer >= 1 && AI_Timer % 5 == 0)
            {
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + attack4Offset, NPC.Center.Y + 1200, 0f, -10f, ModContent.ProjectileType<AnubisBeam>(), NPC.damage / 5, 1, Main.myPlayer, 0, 0);
                attack4Offset += 50 * NPC.direction;

            }
            if (AI_Timer >= 61)
            {
                AI_Timer = 0;
                attack4Offset = NPC.direction * 100;
                ChooseMove();
            }


        }

        private void Attack4Alt()
        {
            AI_Timer++;
            Player target = Main.player[NPC.target];

            if (AI_Timer == 1)
            {
                SoundEngine.PlaySound(Arise, NPC.position);
                attack4Offset = NPC.direction * 1200;
            }
            if (AI_Timer >= 1 && AI_Timer % 5 == 0)
            {
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + attack4Offset, NPC.Center.Y + 1200, 0f, -10f, ModContent.ProjectileType<AnubisBeam>(), NPC.damage / 5, 1, Main.myPlayer, 0, 0);
                attack4Offset -= 50 * NPC.direction;

            }
            if (AI_Timer >= 61)
            {
                AI_Timer = 0;
                attack4Offset = NPC.direction * 1200;
                ChooseMove();
            }


        }

        private void Move5()
        {
            AI_Timer++;
            if (AI_Timer >= 180)
            {
                AI_State = (float)ActionState.Attack5;
                AI_Timer = 0;
            }
        }
        private void Attack5()
        {
            AI_Timer++;
            Vector2 direction = Main.player[NPC.target].Center - NPC.Center;
            direction.Normalize();
            direction *= 6f;
            const int AmountOfProjectiles = 5;
            if (AI_Timer == 51)
            {
                SoundEngine.PlaySound(Fire, NPC.position);
            }
            if (AI_Timer == 61)
            {
                SoundEngine.PlaySound(SoundID.Item124, NPC.Center);
                for (int i = 0; i < AmountOfProjectiles; ++i)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, direction.RotatedByRandom(MathHelper.Pi / 4) * Main.rand.NextFloat(0.5f, 1f), ModContent.ProjectileType<AnubisBeam>(), NPC.damage / 5, 1, Main.myPlayer, 0, 0);
                }
            }
            if (AI_Timer == 121)
            {
                SoundEngine.PlaySound(SoundID.Item124, NPC.Center);
                for (int i = 0; i < AmountOfProjectiles; ++i)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, direction.RotatedByRandom(MathHelper.Pi / 4) * Main.rand.NextFloat(0.5f, 1f), ModContent.ProjectileType<AnubisBeam>(), NPC.damage / 5, 1, Main.myPlayer, 0, 0);
                }
            }
            if (AI_Timer == 181)
            {
                SoundEngine.PlaySound(SoundID.Item124, NPC.Center);
                for (int i = 0; i < AmountOfProjectiles; ++i)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, direction.RotatedByRandom(MathHelper.Pi / 4) * Main.rand.NextFloat(0.5f, 1f), ModContent.ProjectileType<AnubisBeam>(), NPC.damage / 3, 1, Main.myPlayer, 0, 0);
                }
                ChooseMove();
                AI_Timer = 0;
              
            }


        }
        private void Move6()
        {

            AI_Timer++;
            if (AI_Timer >= 180)
            {
                AI_State = (float)ActionState.Attack6;
                AI_Timer = 0;
            }
        }




        private void Attack6()
        {
            AI_Timer++;
            Player target = Main.player[NPC.target];
            // Credit to goose!!!
            Vector2 StartPosition = target.Center;
            float angle = Main.rand.NextFloat(2f * MathF.PI);
            int damage = NPC.damage / 4;

            if (AI_Timer == 41)
            {
                SoundEngine.PlaySound(Arise, NPC.position);
                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<AnubisChaser>(), NPC.damage / 4, 1);
            }
            if (AI_Timer >= 51 && (AI_Timer - 41) % 20 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item124, NPC.Center);
                Vector2 vel = NPC.DirectionTo(target.Center + (target.velocity)) * 12;
                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, vel, ModContent.ProjectileType<AnubisBeam>(), NPC.damage / 4, 1);
            }
            if (AI_Timer == 181)
            {
                ChooseMove();
                AI_Timer = 0;
            }
        }
        private void Heal()
        {
            AI_Timer++;
            if(NPC.life != NPC.lifeMax)
            {
                int difficultyScaling = 1;
                if (Main.expertMode) difficultyScaling *= 2;
                if (Main.masterMode) difficultyScaling *= 2;
                NPC.life += 56 * difficultyScaling;
                InGuardingState = true;
                if (AI_Timer % 10 == 0) NPC.HealEffect(560 * difficultyScaling, false);
            }
            if (AI_Timer >= 160)
            {
                ChooseMove();
                AI_Timer = 0;
                InGuardingState = false;
            }
        }
        private void SecondStage1()
        {
            // Guard
            AI_Timer++;
            Player target = Main.player[NPC.target];
            Vector2 position1 = NPC.Center;
            if (AI_Timer == 51)
            {
                SoundEngine.PlaySound(Guard, NPC.position);
            }
            if (AI_Timer == 61)
            {
                Projectile.NewProjectile(NPC.GetSource_FromThis(), position1, Vector2.Zero, ModContent.ProjectileType<EmeraldWall>(), NPC.damage / 2, 0f, Main.myPlayer);
            }
            if (AI_Timer == 62)
            {
                InGuardingState = true;
            }
            if (AI_Timer >= 520)
            {
                InGuardingState = false;
                AI_State = (float)ActionState.SecondStage2;
                AI_Timer = 0;
            }

        }
        private void SecondStage2()
        {
            AI_Timer++;
            if (AI_Timer >= 180)
            {
                AI_State = (float)ActionState.SecondStage3;
                AI_Timer = 0;
            }
        }
        private void SecondStage3()
        {
            AI_Timer++;
            // More Credit to PaperLuigi
            float speed = 5f;
            int type = ModContent.ProjectileType<AnubisBeam>();
            int damage = NPC.damage;
            var entitySource = NPC.GetSource_FromAI();
            if (AI_Timer == 61)
            {
                SoundEngine.PlaySound(Pierce, NPC.position);
                for (int ir = 0; ir < 9; ir++)
                {
                    Vector2 positionNew = Vector2.Lerp(new Vector2(NPC.Center.X + 1600, NPC.Center.Y - 800), new Vector2(NPC.Center.X + 800, NPC.Center.Y + 800), (float)ir / 9);
                    Vector2 velocity = new Vector2(-speed, 0);

                    Projectile.NewProjectile(entitySource, positionNew, velocity, type, damage / 4, 0f, Main.myPlayer);


                }
                for (int ir = 0; ir < 7; ir++)
                {
                    Vector2 positionNew = Vector2.Lerp(new Vector2(NPC.Center.X - 1600, NPC.Center.Y + 800), new Vector2(NPC.Center.X - 800, NPC.Center.Y - 800), (float)ir / 7);
                    Vector2 velocity = new Vector2(speed, 0);

                    Projectile.NewProjectile(entitySource, positionNew, velocity, type, damage / 4, 0f, Main.myPlayer);


                }
                AI_State = (float)ActionState.SecondStage4;
                AI_Timer = 0;

            }
           
        }
        private void SecondStage4()
        {
            AI_Timer++;
            Player target = Main.player[NPC.target];
            int offsetX = Main.rand.Next(-200, 200) * 6;
            if (AI_Timer == 51)
            {
                SoundEngine.PlaySound(Arise, NPC.position);
            }
            if (AI_Timer >= 61)
            {
                if (Main.rand.NextBool(5))
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), target.Center.X + offsetX, target.Center.Y - 1200, 0f, 5f, ModContent.ProjectileType<AnubisBeam>(), NPC.damage / 5, 1, Main.myPlayer, 0, 0);
                }

            }
            if (AI_Timer >= 121)
            {
                AI_Timer = 0;
                AI_State = (float)ActionState.SecondStage5;
            }

        }
        private void SecondStage5()
        {
            AI_Timer++;
            if (AI_Timer >= 180)
            {
                AI_State = (float)ActionState.SecondStage6;
                AI_Timer = 0;
            }
        }
        private void SecondStage6()
        {
            AI_Timer++;
            Vector2 direction = Main.player[NPC.target].Center - NPC.Center;
            direction.Normalize();
            direction *= 6f;
            const int AmountOfProjectiles = 6;
            if (AI_Timer == 51)
            {
                SoundEngine.PlaySound(Fire, NPC.position);
            }
            if (AI_Timer == 61)
            {
                SoundEngine.PlaySound(SoundID.Item124, NPC.Center);
                for (int i = 0; i < AmountOfProjectiles; ++i)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, direction.RotatedByRandom(MathHelper.Pi / 4) * Main.rand.NextFloat(0.5f, 1f), ModContent.ProjectileType<AnubisBeam>(), NPC.damage / 3, 1, Main.myPlayer, 0, 0);
                }
            }
            if (AI_Timer == 121)
            {
                SoundEngine.PlaySound(SoundID.Item124, NPC.Center);
                for (int i = 0; i < AmountOfProjectiles; ++i)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, direction.RotatedByRandom(MathHelper.Pi / 4) * Main.rand.NextFloat(0.5f, 1f), ModContent.ProjectileType<AnubisBeam>(), NPC.damage / 3, 1, Main.myPlayer, 0, 0);
                }
            }
            if (AI_Timer == 181)
            {
                SoundEngine.PlaySound(SoundID.Item124, NPC.Center);
                for (int i = 0; i < AmountOfProjectiles; ++i)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, direction.RotatedByRandom(MathHelper.Pi / 4) * Main.rand.NextFloat(0.5f, 1f), ModContent.ProjectileType<AnubisBeam>(), NPC.damage / 3, 1, Main.myPlayer, 0, 0);
                }
                AI_State = (float)ActionState.SecondStage7;
                AI_Timer = 0;

            }
        }
        private void SecondStage7()
        {
            AI_Timer++;
            Player target = Main.player[NPC.target];
            // Credit to PaperLuigi
            float Speed = 4f;
            Vector2 StartPosition = new Vector2(NPC.Center.X, NPC.Center.Y - 800);
            int damage = NPC.damage / 4;
            int type = ModContent.ProjectileType<AnubisBeam>();

            float rotation = (float)Math.Atan2(StartPosition.Y - (target.position.Y + (target.height * 0.5f)), StartPosition.X - (target.position.X + (target.width * 0.5f)));
            Vector2 velocity = new Vector2((float)((Math.Cos(rotation) * Speed) * -1), (float)((Math.Sin(rotation) * Speed) * -1));
            if (AI_Timer == 51)
            {
                SoundEngine.PlaySound(Arise, NPC.position);
            }
            if (AI_Timer == 61)
            {
                SoundEngine.PlaySound(SoundID.Item124, NPC.Center);
                for (int d = 0; d < 30; d++)
                {
                    Dust.NewDust(NPC.Center, 0, 0, DustID.BubbleBurst_Purple, 0f + Main.rand.Next(-20, 20), 0f + Main.rand.Next(-20, 20), 150, default(Color), 1.5f);
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float numberProjectiles = 12;
                    float adjustedRotation = MathHelper.ToRadians(45);
                    for (int i = 0; i < numberProjectiles; i++)
                    {
                        Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-adjustedRotation, adjustedRotation, i / (numberProjectiles - 1))) * .2f; // Watch out for dividing by 0 if there is only 1 projectile.
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), StartPosition.X, StartPosition.Y, perturbedSpeed.X * 5, perturbedSpeed.Y * 5, type, damage, 0, Main.myPlayer);
                    }
                }
            }
            if (AI_Timer == 121)
            {
                SoundEngine.PlaySound(SoundID.Item124, NPC.Center);
                for (int d = 0; d < 30; d++)
                {
                    Dust.NewDust(NPC.Center, 0, 0, DustID.BubbleBurst_Purple, 0f + Main.rand.Next(-20, 20), 0f + Main.rand.Next(-20, 20), 150, default(Color), 1.5f);
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float numberProjectiles = 12;
                    float adjustedRotation = MathHelper.ToRadians(40);
                    for (int i = 0; i < numberProjectiles; i++)
                    {
                        Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-adjustedRotation, adjustedRotation, i / (numberProjectiles - 1))) * .2f; // Watch out for dividing by 0 if there is only 1 projectile.
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), StartPosition.X, StartPosition.Y, perturbedSpeed.X * 5, perturbedSpeed.Y * 5, type, damage, 0, Main.myPlayer);
                    }
                }
            }
            if (AI_Timer == 181)
            {
                SoundEngine.PlaySound(SoundID.Item124, NPC.Center);
                for (int d = 0; d < 30; d++)
                {
                    Dust.NewDust(NPC.Center, 0, 0, DustID.BubbleBurst_Purple, 0f + Main.rand.Next(-20, 20), 0f + Main.rand.Next(-20, 20), 150, default(Color), 1.5f);
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float numberProjectiles = 12;
                    float adjustedRotation = MathHelper.ToRadians(35);
                    for (int i = 0; i < numberProjectiles; i++)
                    {
                        Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-adjustedRotation, adjustedRotation, i / (numberProjectiles - 1))) * .2f; // Watch out for dividing by 0 if there is only 1 projectile.
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), StartPosition.X, StartPosition.Y, perturbedSpeed.X * 5, perturbedSpeed.Y * 5, type, damage, 0, Main.myPlayer);
                    }
                }


            }
            float speed2 = 5f;
            int type2 = ModContent.ProjectileType<AnubisBeam>();
            int damage2 = NPC.damage;
            var entitySource = NPC.GetSource_FromAI();
            if (AI_Timer == 241)
            {
                SoundEngine.PlaySound(Pierce, NPC.position);
                for (int ir = 0; ir < 9; ir++)
                {
                    Vector2 positionNew = Vector2.Lerp(new Vector2(NPC.Center.X + 1600, NPC.Center.Y - 800), new Vector2(NPC.Center.X + 800, NPC.Center.Y + 800), (float)ir / 9);
                    Vector2 velocity2 = new Vector2(-speed2, 0);

                    Projectile.NewProjectile(entitySource, positionNew, velocity2, type2, damage2 / 4, 0f, Main.myPlayer);


                }
            }
            if (AI_Timer == 301)
            {
                for (int ir = 0; ir < 7; ir++)
                {
                    Vector2 positionNew = Vector2.Lerp(new Vector2(NPC.Center.X - 1600, NPC.Center.Y + 800), new Vector2(NPC.Center.X - 800, NPC.Center.Y - 800), (float)ir / 7);
                    Vector2 velocity2 = new Vector2(speed2, 0);

                    Projectile.NewProjectile(entitySource, positionNew, velocity2, type2, damage2 / 4, 0f, Main.myPlayer);


                }
               
            }
            if (AI_Timer == 351)
            {
                SoundEngine.PlaySound(Arise, NPC.position);
            }
            if (AI_Timer == 361)
            {
                SoundEngine.PlaySound(SoundID.Item124, NPC.Center);
                for (int d = 0; d < 30; d++)
                {
                    Dust.NewDust(NPC.Center, 0, 0, DustID.BubbleBurst_Purple, 0f + Main.rand.Next(-20, 20), 0f + Main.rand.Next(-20, 20), 150, default(Color), 1.5f);
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float numberProjectiles = 12;
                    float adjustedRotation = MathHelper.ToRadians(45);
                    for (int i = 0; i < numberProjectiles; i++)
                    {
                        Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-adjustedRotation, adjustedRotation, i / (numberProjectiles - 1))) * .2f; // Watch out for dividing by 0 if there is only 1 projectile.
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), StartPosition.X, StartPosition.Y, perturbedSpeed.X * 5, perturbedSpeed.Y * 5, type, damage, 0, Main.myPlayer);
                    }
                }
            }
            if (AI_Timer == 421)
            {
                SoundEngine.PlaySound(SoundID.Item124, NPC.Center);
                for (int d = 0; d < 30; d++)
                {
                    Dust.NewDust(NPC.Center, 0, 0, DustID.BubbleBurst_Purple, 0f + Main.rand.Next(-20, 20), 0f + Main.rand.Next(-20, 20), 150, default(Color), 1.5f);
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float numberProjectiles = 12;
                    float adjustedRotation = MathHelper.ToRadians(40);
                    for (int i = 0; i < numberProjectiles; i++)
                    {
                        Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-adjustedRotation, adjustedRotation, i / (numberProjectiles - 1))) * .2f; // Watch out for dividing by 0 if there is only 1 projectile.
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), StartPosition.X, StartPosition.Y, perturbedSpeed.X * 5, perturbedSpeed.Y * 5, type, damage, 0, Main.myPlayer);
                    }
                }
            }
            if (AI_Timer == 481)
            {
                SoundEngine.PlaySound(SoundID.Item124, NPC.Center);
                for (int d = 0; d < 30; d++)
                {
                    Dust.NewDust(NPC.Center, 0, 0, DustID.BubbleBurst_Purple, 0f + Main.rand.Next(-20, 20), 0f + Main.rand.Next(-20, 20), 150, default(Color), 1.5f);
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float numberProjectiles = 12;
                    float adjustedRotation = MathHelper.ToRadians(35);
                    for (int i = 0; i < numberProjectiles; i++)
                    {
                        Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-adjustedRotation, adjustedRotation, i / (numberProjectiles - 1))) * .2f; // Watch out for dividing by 0 if there is only 1 projectile.
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), StartPosition.X, StartPosition.Y, perturbedSpeed.X * 5, perturbedSpeed.Y * 5, type, damage, 0, Main.myPlayer);
                    }
                }

            }
            if (AI_Timer == 541)
            {
                SoundEngine.PlaySound(SoundID.Item124, NPC.Center);
                for (int d = 0; d < 30; d++)
                {
                    Dust.NewDust(NPC.Center, 0, 0, DustID.BubbleBurst_Purple, 0f + Main.rand.Next(-20, 20), 0f + Main.rand.Next(-20, 20), 150, default(Color), 1.5f);
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float numberProjectiles = 12;
                    float adjustedRotation = MathHelper.ToRadians(40);
                    for (int i = 0; i < numberProjectiles; i++)
                    {
                        Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-adjustedRotation, adjustedRotation, i / (numberProjectiles - 1))) * .2f; // Watch out for dividing by 0 if there is only 1 projectile.
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), StartPosition.X, StartPosition.Y, perturbedSpeed.X * 5, perturbedSpeed.Y * 5, type, damage, 0, Main.myPlayer);
                    }
                }
            }
            if (AI_Timer == 601)
            {
                SoundEngine.PlaySound(SoundID.Item124, NPC.Center);
                for (int d = 0; d < 30; d++)
                {
                    Dust.NewDust(NPC.Center, 0, 0, DustID.BubbleBurst_Purple, 0f + Main.rand.Next(-20, 20), 0f + Main.rand.Next(-20, 20), 150, default(Color), 1.5f);
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float numberProjectiles = 12;
                    float adjustedRotation = MathHelper.ToRadians(35);
                    for (int i = 0; i < numberProjectiles; i++)
                    {
                        Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-adjustedRotation, adjustedRotation, i / (numberProjectiles - 1))) * .2f; // Watch out for dividing by 0 if there is only 1 projectile.
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), StartPosition.X, StartPosition.Y, perturbedSpeed.X * 5, perturbedSpeed.Y * 5, type, damage, 0, Main.myPlayer);
                    }
                }
            }
            if (AI_Timer == 641)
            {
                for (int d = 0; d < 30; d++)
                {
                    Dust.NewDust(NPC.Center, 0, 0, DustID.BubbleBurst_Purple, 0f + Main.rand.Next(-20, 20), 0f + Main.rand.Next(-20, 20), 150, default(Color), 1.5f);
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float numberProjectiles = 12;
                    float adjustedRotation = MathHelper.ToRadians(30);
                    for (int i = 0; i < numberProjectiles; i++)
                    {
                        Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-adjustedRotation, adjustedRotation, i / (numberProjectiles - 1))) * .2f; // Watch out for dividing by 0 if there is only 1 projectile.
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), StartPosition.X, StartPosition.Y, perturbedSpeed.X * 5, perturbedSpeed.Y * 5, type, damage, 0, Main.myPlayer);
                    }
                }

            }
            Vector2 direction = Main.player[NPC.target].Center - NPC.Center;
            direction.Normalize();
            direction *= 6f;
            const int AmountOfProjectiles = 6;
            if (AI_Timer == 691)
            {
                SoundEngine.PlaySound(Fire, NPC.position);
            }
            if(AI_Timer == 701)
            {
                SoundEngine.PlaySound(SoundID.Item124, NPC.Center);
                for (int i = 0; i < AmountOfProjectiles; ++i)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, direction.RotatedByRandom(MathHelper.Pi / 4) * Main.rand.NextFloat(0.5f, 1f), ModContent.ProjectileType<AnubisBeam>(), NPC.damage / 3, 1, Main.myPlayer, 0, 0);
                }
                AI_State = (float)ActionState.SecondStage8;
                AI_Timer = 0;
            }
           
        }
        private void SecondStage8()
        {
            AI_Timer++;
            if (AI_Timer >= 180)
            {
                AI_State = (float)ActionState.SecondStage9;
                AI_Timer = 0;
            }
        }
        private void SecondStage9()
        {
            AI_Timer++;
            Vector2 direction = Main.player[NPC.target].Center - NPC.Center;
            direction.Normalize();
            direction *= 2f;
            const int AmountOfProjectiles = 6;
            if (AI_Timer == 51)
            {
                SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact, NPC.position);
            }
            if (AI_Timer == 61)
            {
                SoundEngine.PlaySound(SoundID.Item124, NPC.Center);
                for (int i = 0; i < AmountOfProjectiles; ++i)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, direction.RotatedByRandom(MathHelper.Pi / 4) * Main.rand.NextFloat(0.5f, 1f), ModContent.ProjectileType<AnubisDisc>(), NPC.damage / 3, 1, Main.myPlayer, 0, 0);
                }
               
            }
            float speed = 10f;
            int type = ModContent.ProjectileType<AnubisDisc>();
            int damage = NPC.damage;
            var entitySource = NPC.GetSource_FromAI();
            if (AI_Timer == 121)
            {
                SoundEngine.PlaySound(Pierce, NPC.position);
                for (int ir = 0; ir < 3; ir++)
                {
                    Vector2 positionNew = Vector2.Lerp(new Vector2(NPC.Center.X - 100, NPC.Center.Y + 300), new Vector2(NPC.Center.X - 800, NPC.Center.Y - 800), (float)ir / 7);
                    Vector2 velocity = new Vector2(speed, 0);

                    Projectile.NewProjectile(entitySource, positionNew, velocity, type, damage / 4, 0f, Main.myPlayer);


                }
                AI_State = (float)ActionState.SecondStage2;
                AI_Timer = 0;

            }

        }
        /*public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (InGuardingState)
            {
                switch (Main.rand.Next(3))
                {
                    /*case 0:
                        if(hit.DamageType == DamageClass.Magic || hit.DamageType == DamageClass.MagicSummonHybrid)
                        {
                            NPC.HealEffect(hit.Damage, true);
                            NPC.life += hit.Damage;
                        }
                        break;
                    case 1:
                        if (hit.DamageType == DamageClass.Melee || hit.DamageType == DamageClass.MeleeNoSpeed)
                        {
                            NPC.HealEffect(hit.Damage, true);
                            NPC.life += hit.Damage;
                        }
                        break;
                    case 2:
                        if (hit.DamageType == DamageClass.Summon || hit.DamageType == DamageClass.SummonMeleeSpeed)
                        {
                            NPC.HealEffect(hit.Damage, true);
                            NPC.life += hit.Damage;
                        }
                        break;
                    case 3:
                        if (hit.DamageType == DamageClass.Ranged)
                        {
                            NPC.HealEffect(hit.Damage, true);
                            NPC.life += hit.Damage;
                        }
                        break;
                    
                }
                NPC.HealEffect(hit.Damage, true);
                NPC.life += hit.Damage;
            }
        }
        */

      /*  public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.type == ModContent.ProjectileType<EmeraldWall>() || projectile.type == ModContent.ProjectileType<EmeraldWallSharp>() ||  projectile.type == ProjectileID.BouncyBoulder || projectile.type == ModContent.ProjectileType<AnubisBeam>() || projectile.type == ProjectileID.CultistBossLightningOrb || projectile.type == ProjectileID.CultistBossLightningOrbArc || projectile.type == ModContent.ProjectileType<AnubisDisc>())
            {
                return false;
            }
            else
            {
                return true;
            }
        }
      */
        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.DownedAnubis, -1);
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.WorldData);
            }
        }
        public bool SecondStage;
        public bool FirstSecondStageSwitch;
        public bool reversedAttack4 = false;
        public override void AI()
        {
            if (InGuardingState)
            {
                NPC.immortal = true;
            }
            else if (!InGuardingState)
            {
                NPC.immortal = false;
            }
            Player player = Main.player[NPC.target];
            if (player.dead)
            {
                NPC.EncourageDespawn(5);
                NPC.velocity.Y -= 0.04f;
                NPC.EncourageDespawn(10);
                return;
            }
            if (NPC.life <= (NPC.lifeMax / 2) && NPC.life != NPC.lifeMax)
            {
                SecondStage = true;

            }

            if (AI_State != (float)ActionState.Attack4)
            {
                reversedAttack4 = Main.rand.NextBool();
            }

            switch (AI_State)
            {
                case (float)ActionState.Notice:
                    Notice();
                    break;
                case (float)ActionState.Move:
                    Move();
                    break;
                case (float)ActionState.Attack:
                    Attack();
                    break;
                case (float)ActionState.Move2:
                    Move2();
                    break;
                case (float)ActionState.Attack2:
                    Attack2();
                    break;
                case (float)ActionState.Move3:
                    Move3();
                    break;
                case (float)ActionState.Attack3:
                    Attack3();
                    break;
                case (float)ActionState.Move4:
                    Move4();
                    break;
                case (float)ActionState.Attack4:
                    if (reversedAttack4) Attack4Alt();
                    else Attack4();
                    break;
                case (float)ActionState.Move5:
                    Move5();
                    break;
                case (float)ActionState.Attack5:
                    Attack5();
                    break;
                case (float)ActionState.Move6:
                    Move6();
                    break;
                case (float)ActionState.Attack6:
                    Attack6();
                    break;
                case (float)ActionState.Heal:
                    Heal();
                    break;
                case (float)ActionState.SecondStage1:
                    SecondStage1();
                    break;
                case (float)ActionState.SecondStage2:
                    SecondStage2();
                    break;
                case (float)ActionState.SecondStage3:
                    SecondStage3();
                    break;
                case (float)ActionState.SecondStage4:
                    SecondStage4();
                    break;
                case (float)ActionState.SecondStage5:
                    SecondStage5();
                    break;
                case (float)ActionState.SecondStage6:
                    SecondStage6();
                    break;
                case (float)ActionState.SecondStage7:
                    SecondStage7();
                    break;
                case (float)ActionState.SecondStage8:
                    SecondStage8();
                    break;
                case (float)ActionState.SecondStage9:
                    SecondStage9();
                    break;
            }
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;
            switch (AI_State)
            {
                case (float)ActionState.Notice:
                    if (AI_Timer < 10)
                    {
                        NPC.frame.Y = (int)Frame.Move1 * frameHeight;
                    }
                    else
                    {
                        NPC.frame.Y = (int)Frame.Move1 * frameHeight;
                    }
                    break;
                case (float)ActionState.Move:
                case (float)ActionState.Move2:
                case (float)ActionState.Move3:
                case (float)ActionState.Move4:
                case (float)ActionState.Move5:
                case (float)ActionState.Move6:
                case (float)ActionState.Heal:
                case (float)ActionState.SecondStage1:
                case (float)ActionState.SecondStage2:
                case (float)ActionState.SecondStage5:
                case (float)ActionState.SecondStage8:

                    NPC.frameCounter++;
                    if (NPC.frameCounter < 10)
                    {
                        NPC.frame.Y = (int)Frame.Move1 * frameHeight;
                    }
                    else if (NPC.frameCounter < 20)
                    {
                        NPC.frame.Y = (int)Frame.Move2 * frameHeight;
                    }
                    else if (NPC.frameCounter < 30)
                    {
                        NPC.frame.Y = (int)Frame.Move3 * frameHeight;
                    }
                    else
                    {
                        NPC.frameCounter = 0;
                    }
                    break;
                case (float)ActionState.Attack:
                case (float)ActionState.Attack2:
                case (float)ActionState.Attack3:
                case (float)ActionState.Attack4:
                case (float)ActionState.Attack5:
                case (float)ActionState.Attack6:
                case (float)ActionState.SecondStage3:
                case (float)ActionState.SecondStage4:
                case (float)ActionState.SecondStage6:
                case (float)ActionState.SecondStage7:
                case (float)ActionState.SecondStage9:

                    NPC.frameCounter++;
                    if (NPC.frameCounter < 10)
                    {
                        NPC.frame.Y = (int)Frame.Attack1 * frameHeight;
                    }
                    else if (NPC.frameCounter < 20)
                    {
                        NPC.frame.Y = (int)Frame.Attack2 * frameHeight;
                    }
                    else if (NPC.frameCounter < 30)
                    {
                        NPC.frame.Y = (int)Frame.Attack3 * frameHeight;
                    }
                    else if (NPC.frameCounter < 40)
                    {
                        NPC.frame.Y = (int)Frame.Attack4 * frameHeight;
                    }
                    else if (NPC.frameCounter < 50)
                    {
                        NPC.frame.Y = (int)Frame.Attack5 * frameHeight;
                    }
                   else if (NPC.frameCounter < 60)
                    {
                        NPC.frame.Y = (int)Frame.Attack6 * frameHeight;
                    }
                    else
                    {
                        NPC.frameCounter = 0;
                    }
                    break;
            }
        }
    }
}
