
using Conquest.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Conquest.Projectiles.Summoner.NaniteMinionStuff
{
    public class NaniteMinion : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.Nanites}";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Jelly Minion");
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
        }
        public sealed override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.tileCollide = false;

            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;
        }
        public enum ActionState
        {
            idle,

        }
        private enum atkType
        {
            LaserTriangle,
        }
        private uint AI_State_uint
        {
            get => BitConverter.SingleToUInt32Bits(Projectile.ai[1]);
            set => Projectile.ai[1] = BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
        }

        private ActionState AI_State
        {
            get => (ActionState)AI_State_uint;
            set => AI_State_uint = (uint)value;
        }
        private uint AttackType_uint
        {
            get => BitConverter.SingleToUInt32Bits(Projectile.ai[2]);
            set => Projectile.ai[2] = BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
        }

        private atkType AttackType
        {
            get => (atkType)AttackType_uint;
            set => AI_State_uint = (uint)value;
        }

        private ref float timer => ref Projectile.ai[0];

        public override bool MinionContactDamage()
        {
            return false;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (!CheckActive(owner))
            {
                return;
            }
            Projectile.Center = owner.Center; //set the projectile to


            SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);

            Visuals();

            timer++;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(ModContent.BuffType<Electrified2>(), 60);
            }
        }
        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<TheSwarm>());

                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<TheSwarm>()))
            {
                Projectile.timeLeft = 2;
            }

            return true;
        }

        private void SearchForTargets(Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter)
        {
            distanceFromTarget = 700f;
            targetCenter = Projectile.position;
            foundTarget = false;

            if (owner.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[owner.MinionAttackTargetNPC];
                float between = Vector2.Distance(npc.Center, Projectile.Center);

                if (between < 2000f)
                {
                    distanceFromTarget = between;
                    targetCenter = npc.Center;
                    foundTarget = true;
                }
            }

            if (!foundTarget)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];

                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, Projectile.Center);
                        bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
                        bool closeThroughWall = between < 100f;

                        if ((closest && inRange || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }

            Projectile.friendly = foundTarget;
        }

        private void Visuals()
        {
            Projectile.rotation = Projectile.velocity.X * 0.05f;

            int frameSpeed = 5;

            Projectile.frameCounter++;

            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }

            // Some visuals here
        }
        public override bool PreDraw(ref Color lightColor)
        {
            float prismMaxDrawTime = 200;
            Color color1 = new(116, 251, 253);
            Color color2 = new(99, 208, 223);
            Color color3 = new(77, 164, 169);
            Main.instance.LoadProjectile(ProjectileID.LastPrism);
            Texture2D prism = TextureAssets.Projectile[ProjectileID.LastPrism].Value;
            Vector2 position = Main.MouseWorld - Main.screenPosition;


            if (AttackType == atkType.LaserTriangle)
            {
                if (timer < prismMaxDrawTime)
                {
                    float percent = timer / prismMaxDrawTime;
                    Rectangle slice = new Rectangle(0, 0, prism.Width, (int)(prism.Height / 5 * percent));
                    Main.spriteBatch.Draw(prism, position, slice, color1, Projectile.rotation, prism.source(1, 5), 1, SpriteEffects.None, 1);
                    Dust.NewDust(position + Main.screenPosition - Vector2.UnitX * prism.Width / 2, prism.Width, 1, DustID.Electric, 0, 0, 0, default, 0.5f);
                }
                if (timer >= prismMaxDrawTime)
                {
                    Main.spriteBatch.Draw(prism, position, prism.GetFrame((int)(timer % 5), 5, 2), color1, Projectile.rotation, prism.source(1, 5), 1, SpriteEffects.None, 1);
                }
            }
            return false;
        }
    }
    public static class PeePeePooPoo
    {
        /// <summary>
        /// returns the center of the vector
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="XQuotientOffset">dont make this 0 or the earth will explode</param>
        /// <param name="YQuotientOffset">this too</param>
        /// <returns></returns>
        public static Vector2 source(this Texture2D texture, float XQuotientOffset = 1, float YQuotientOffset = 1)
        {
            return new Vector2(texture.Width / XQuotientOffset / 2, texture.Height / YQuotientOffset / 2);
        }
        public static Rectangle GetFrame(this Texture2D texture, int frame, int maxFrames, int spacing)
        {
            int frameHeight = (texture.Height - (maxFrames * spacing))/maxFrames;
            return new Rectangle(0, frame * frameHeight + frame * spacing, texture.Width, frameHeight);
        }
    }
}
