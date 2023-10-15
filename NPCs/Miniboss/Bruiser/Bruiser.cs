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
using Conquest.Assets.Systems;
using Terraria.GameContent.ItemDropRules;
using Conquest.Items.Weapons.Melee;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;

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
            NPC.defense = 15;
            NPC.lifeMax = 6600;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath9;
            NPC.value = 60f;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = 0;
            NPC.lavaImmune = false;
            NPC.noGravity = false;

            NPC.noTileCollide = false;
        }

          public override void HitEffect(NPC.HitInfo hit)
        {
            if (Main.netMode == NetmodeID.Server)
            {

                return;
            }

            if (NPC.life <= 0)
            {
                int headGoreType = Mod.Find<ModGore>("Bruiser_Head").Type;
                int armGoreType = Mod.Find<ModGore>("Bruiser_Arm").Type;
                int spikeGoreType = Mod.Find<ModGore>("Bruiser_Spike").Type;

                var entitySource = NPC.GetSource_Death();

                for (int i = 0; i < 1; i++)
                {
                    Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), headGoreType);
                    Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), armGoreType);
                    Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), spikeGoreType);
                }

                SoundEngine.PlaySound(SoundID.NPCDeath1, NPC.Center);
            }
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
                direction.Y -= 1.15f * MathF.Sqrt(MathF.Abs(targetPosition.X - position.X));
                direction.Normalize();
                float speed = 14f;
                int type = ModContent.ProjectileType<BruiserBeam>();
                int damage = NPC.damage;

                if (AI_Timer >= 50)
                {
                    AI_Timer = 0;
                    Projectile.NewProjectile(source, position, direction * speed, type, 40, 0f, NPC.whoAmI);
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
            NPC.spriteDirection = -NPC.direction;
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
        private const string ChainTexturePath = "Conquest/NPCs/Miniboss/Bruiser/BruiserBeamChain"; // The folder path to the flail chain sprite

        public Player stuck;
        Vector2 offset;

        public override void SetDefaults()
        {
            Projectile.tileCollide = true;
            Projectile.width = 21;
            Projectile.height = 5;
            Projectile.hostile = true;
            Projectile.scale = 1.2f;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 240;
        }
        public override void AI()
        {
            if (stuck == null)
            {
                Projectile.velocity.Y += 0.13f;
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            else
            {
                if (stuck.dead) Projectile.Kill();
                Projectile.Center = stuck.Center + offset;

                // disable player movement options
                stuck.controlLeft = false;
                stuck.controlRight = false;
                stuck.controlUp = false;
                stuck.controlDown = false;
                stuck.controlJump = false;

                // kill projectile if the bruiser takes enough damage
                if (lifeOnHit - Main.npc[Projectile.owner].life > 200) Projectile.Kill();

                stuck.velocity += stuck.Center.DirectionTo(Main.npc[Projectile.owner].Center) * 0.25f;
            }
        }

        int lifeOnHit;

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= 0;
            modifiers.FlatBonusDamage += 25 * Main.rand.NextFloat(0.85f, 1.15f);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            stuck = target;
            offset = target.Center.DirectionTo(Projectile.Center) * target.Center.Distance(Projectile.Center);
            lifeOnHit = Main.npc[Projectile.owner].life;
            Projectile.hostile = false;
            Projectile.timeLeft = 99999;
        }


        // Taken from ExampleMod with minor edits
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 npcPosition = Main.npc[Projectile.owner].Center;

            // This fixes a vanilla GetPlayerArmPosition bug causing the chain to draw incorrectly when stepping up slopes. The flail itself still draws incorrectly due to another similar bug. This should be removed once the vanilla bug is fixed.
            npcPosition.Y -= Main.npc[Projectile.owner].gfxOffY;

            Asset<Texture2D> chainTexture = ModContent.Request<Texture2D>(ChainTexturePath);

            // if animated chain sprites are ever a thing...
            Rectangle? chainSourceRectangle = null;

            // Drippler Crippler customizes sourceRectangle to cycle through sprite frames: sourceRectangle = asset.Frame(1, 6);
            float chainHeightAdjustment = 0f; // Use this to adjust the chain overlap. 

            Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (chainTexture.Size() / 2f);
            Vector2 chainDrawPosition = Projectile.Center;
            Vector2 vectorFromProjectileToPlayerArms = npcPosition.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
            Vector2 unitVectorFromProjectileToPlayerArms = vectorFromProjectileToPlayerArms.SafeNormalize(Vector2.Zero);
            float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : chainTexture.Height()) + chainHeightAdjustment;
            if (chainSegmentLength == 0)
            {
                chainSegmentLength = 10; // When the chain texture is being loaded, the height is 0 which would cause infinite loops.
            }
            float chainRotation = unitVectorFromProjectileToPlayerArms.ToRotation() + MathHelper.PiOver2;
            int chainCount = 0;
            float chainLengthRemainingToDraw = vectorFromProjectileToPlayerArms.Length() + chainSegmentLength / 2f;

            // This while loop draws the chain texture from the projectile to the player, looping to draw the chain texture along the path
            while (chainLengthRemainingToDraw > 0f)
            {
                // This code gets the lighting at the current tile coordinates
                Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

                // Flaming Mace and Drippler Crippler use code here to draw custom sprite frames with custom lighting.
                // Cycling through frames: sourceRectangle = asset.Frame(1, 6, 0, chainCount % 6);
                // This example shows how Flaming Mace works. It checks chainCount and changes chainTexture and draw color at different values

                // Here, we draw the chain texture at the coordinates
                Main.spriteBatch.Draw(chainTexture.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

                // chainDrawPosition is advanced along the vector back to the player by the chainSegmentLength
                chainDrawPosition += unitVectorFromProjectileToPlayerArms * chainSegmentLength;
                chainCount++;
                chainLengthRemainingToDraw -= chainSegmentLength;
            }
            return true;
        }

    }

}
