
using Conquest.Projectiles.Hostile;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Conquest.NPCs 
{ 
    public class FloralThreat : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;
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
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Jungle,

                new FlavorTextBestiaryInfoElement("A mutated flower, it wanders aimlessy spreading its spores."),

            });
        }
        private enum ActionState
        {
            Asleep,
            Notice,
            Move,
            Spit,
        }
        private enum Frame
        {
            Asleep,
            Move1,
            Move2,
            Move3,
            Spit1,
            Spit2,
            Spit3,
            Spit4,
        }
        public override void SetDefaults()
        {
            NPC.width = 42;
            NPC.height = 45;
            NPC.damage = 30;
            NPC.defense = 5;
            NPC.lifeMax = 60;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath9;
            NPC.value = 60f;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.lavaImmune = false;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
        }
        public ref float AI_State => ref NPC.ai[0];
        public ref float AI_Timer => ref NPC.ai[1];
        private void FallAsleep()
        {
            NPC.TargetClosest(true);

            if (NPC.HasValidTarget && Main.player[NPC.target].Distance(NPC.Center) < 500f)
            {
                AI_State = (float)ActionState.Notice;
                AI_Timer = 0;
            }
        }

        private void Notice()
        {
            if (Main.player[NPC.target].Distance(NPC.Center) < 1250f)
            {
                AI_Timer++;

                if (AI_Timer >= 20)
                {
                    AI_State = (float)ActionState.Move;
                    AI_Timer = 0;
                }
            }
            else
            {
                NPC.TargetClosest(true);

                if (!NPC.HasValidTarget || Main.player[NPC.target].Distance(NPC.Center) > 500f)
                {
                    AI_State = (float)ActionState.Asleep;
                    AI_Timer = 0;
                }
            }
        }
        private void Move()
        {
            if (Main.player[NPC.target].Distance(NPC.Center) < 1250f)
            {
                Player target = Main.player[NPC.target];
                Vector2 ToPlayer = NPC.DirectionTo(target.Center) * 2;
                NPC.velocity = ToPlayer;
                NPC.velocity.Y = 0f;
                AI_Timer++;
            }
            if (Main.player[NPC.target].Distance(NPC.Center) < 350f && AI_Timer >= 30)
            {
                AI_State = (float)ActionState.Spit;
                AI_Timer = 0;
            }

        }

        private void Spit()
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
                float speed = 10f;
                int type = ModContent.ProjectileType<FloralSpit>();
                int damage = NPC.damage;

                if (AI_Timer >= 41)
                {
                    AI_Timer = 0;
                    Projectile.NewProjectile(source, position, direction * speed, type, damage, 0f, Main.myPlayer);
                    AI_State = (float)ActionState.Notice;
                }

            }

        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Floral1").Type, 1f);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Floral2").Type, 1f);
            }
        }
        public override void AI()
        {
            switch (AI_State)
            {
                case (float)ActionState.Asleep:
                    FallAsleep();
                    break;
                case (float)ActionState.Notice:
                    Notice();
                    break;
                case (float)ActionState.Move:
                    Move();
                    break;
                case (float)ActionState.Spit:
                    Spit();
                    break;
            }
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;

            switch (AI_State)
            {
                case (float)ActionState.Asleep:
                    NPC.frame.Y = (int)Frame.Asleep * frameHeight;
                    break;
                case (float)ActionState.Notice:
                    if (AI_Timer < 10)
                    {
                        NPC.frame.Y = (int)Frame.Move1 * frameHeight;
                    }
                    else
                    {
                        NPC.frame.Y = (int)Frame.Asleep * frameHeight;
                    }
                    break;
                case (float)ActionState.Move:
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
                case (float)ActionState.Spit:
                    NPC.frameCounter++;
                    if (NPC.frameCounter < 10)
                    {
                        NPC.frame.Y = (int)Frame.Spit1 * frameHeight;
                    }
                    else if (NPC.frameCounter < 20)
                    {
                        NPC.frame.Y = (int)Frame.Spit2 * frameHeight;
                    }
                    else if (NPC.frameCounter < 30)
                    {
                        NPC.frame.Y = (int)Frame.Spit3 * frameHeight;
                    }
                    else if (NPC.frameCounter < 40)
                    {
                        NPC.frame.Y = (int)Frame.Spit4 * frameHeight;
                    }

                    else
                    {
                        NPC.frameCounter = 0;
                    }
                    break;
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.Vine, 3));
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneJungle && NPC.CountNPCS(ModContent.NPCType<FloralThreat>()) != 3)
            {
                return 0.1f;
            }
            else
                return 0f;
        }

    }
}
