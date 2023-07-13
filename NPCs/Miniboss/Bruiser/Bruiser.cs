﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Conquest.Assets.Systems;
using Terraria.GameContent.ItemDropRules;
using Conquest.Items.Weapons.Melee;

namespace Conquest.NPCs.Miniboss.Bruiser
{
    public class Bruiser : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 18;
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

                new FlavorTextBestiaryInfoElement("A mindless beast, trapped in the ancient pyramid"),

            });
        }
        private enum ActionState
        {
            Notice,
            Move,
            Attack,
        }
        private enum Frame
        {
            Asleep,
            Move1,
            Move2,
            Move3,
            Move4, 
            Move5,
            Move6,
            Move7,
            Attack1,
            Attack2,
            Attack3,
            Attack4,
            Attack5,
            Attack6,
            Attack7,
            Attack8,
            Attack9,
            Attack10,
            Attack11,
        }
        public override void SetDefaults()
        {
            NPC.width = 62;
            NPC.height = 116 / 2;
            NPC.damage = 90;
            NPC.defense = 5;
            NPC.lifeMax = 6600;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath9;
            NPC.value = 60f;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = 0;
            NPC.lavaImmune = false;
            NPC.noGravity = false;

            NPC.scale = 1.5f;
            NPC.noTileCollide = false;
        }
        public ref float AI_State => ref NPC.ai[0];
        public ref float AI_Timer => ref NPC.ai[1];
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
                    AI_State = (float)ActionState.Move;
                    AI_Timer = 0;
                }
            }
        }
        private void Move()
        {
            if (Main.player[NPC.target].Distance(NPC.Center) < 1250f)
            {
                Player target = Main.player[NPC.target];
                Vector2 ToPlayer = NPC.DirectionTo(target.Center) * 4;
                NPC.velocity = ToPlayer;
                NPC.velocity.Y = 0f;
                AI_Timer++;
            }
            if (Main.player[NPC.target].Distance(NPC.Center) < 600f && AI_Timer >= 90)
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
                int type = ModContent.ProjectileType<BruiserBeam>();
                int damage = NPC.damage;

                if (AI_Timer >= 70)
                {
                    AI_Timer = 0;
                    Projectile.NewProjectile(source, position, direction * speed, type, damage, 0f, Main.myPlayer);
                    AI_State = (float)ActionState.Notice;
                }

            }

        }
        public override void AI()
        {

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
                    else if (NPC.frameCounter < 40)
                    {
                        NPC.frame.Y = (int)Frame.Move4 * frameHeight;
                    }
                    else if (NPC.frameCounter < 50)
                    {
                        NPC.frame.Y = (int)Frame.Move5 * frameHeight;
                    }
                    else if (NPC.frameCounter < 60)
                    {
                        NPC.frame.Y = (int)Frame.Move6 * frameHeight;
                    }
                    else if (NPC.frameCounter < 70)
                    {
                        NPC.frame.Y = (int)Frame.Move7 * frameHeight;
                    }
                    else
                    {
                        NPC.frameCounter = 0;
                    }
                    break;
                case (float)ActionState.Attack:
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
                    else if (NPC.frameCounter < 70)
                    {
                        NPC.frame.Y = (int)Frame.Attack7 * frameHeight;
                    }
                    else if (NPC.frameCounter < 80)
                    {
                        NPC.frame.Y = (int)Frame.Attack8 * frameHeight;
                    }
                    else if (NPC.frameCounter < 90)
                    {
                        NPC.frame.Y = (int)Frame.Attack9 * frameHeight;
                    }
                    else if (NPC.frameCounter < 100)
                    {
                        NPC.frame.Y = (int)Frame.Attack10 * frameHeight;
                    }
                    else if (NPC.frameCounter < 110)
                    {
                        NPC.frame.Y = (int)Frame.Attack4 * frameHeight;
                    }
                    else
                    {
                        NPC.frameCounter = 0;
                    }
                    break;
            }
        }
        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.DownedBruiser, -1);
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.WorldData);
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MountainBreaker>(), 4));

        }
    }
    
    public class BruiserBeam : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.tileCollide = true;
            Projectile.width = 21;
            Projectile.height = 5;
            Projectile.hostile = true;
            Projectile.scale = 2;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 240;
            Projectile.aiStyle = 1;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

    }

}
