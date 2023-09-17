using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Conquest.Buffs;

namespace Conquest.Projectiles.Summoner
{
    public class Sus : ModProjectile
    {
        public override void SetStaticDefaults()
        {

            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

            Main.projPet[Projectile.type] = true;

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public sealed override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 9;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minionSlots = 0.5f;
            Projectile.penetrate = -1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool MinionContactDamage()
        {
            return true;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (!CheckActive(owner))
            {
                return;
            }

            GeneralBehavior(owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
            SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
            BoidsMovement();
            Movement(foundTarget, distanceFromTarget, targetCenter, distanceToIdlePosition, vectorToIdlePosition);
            Visuals();
        }

        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<SusBuff>());

                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<SusBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            return true;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1);
            for (int i = 0; i < 3; i++)
            {
                Dust.NewDustDirect(Projectile.TopLeft, Projectile.width, Projectile.height, DustID.Blood).velocity = Main.rand.NextVector2Circular(4, 4);
            }
        }

        private void BoidsMovement()
        {
            Vector2 separation = Vector2.Zero;
            Vector2 alignment = Vector2.Zero;
            Vector2 cohesion = Vector2.Zero;
            int count = 0;

            foreach (Projectile p in Main.projectile)
            {
                if (p.hostile && p.Hitbox.Intersects(Projectile.Hitbox))
                {
                    Projectile.Kill();
                    p.penetrate--;
                }

                if (Projectile.whoAmI != p.whoAmI && p.type == Projectile.type && p.active && p.Center.Distance(Projectile.Center) < 128)
                {
                    count++;

                    // avoid colliding with nearby boids
                    separation += 4f * (Projectile.Center - p.Center).SafeNormalize(Vector2.Zero) / (Projectile.Center - p.Center).Length();

                    // face the same way as nearby boids
                    alignment += 1 / 128f * (p.velocity - Projectile.velocity);

                    // move toward the center of groups of boids
                    cohesion += 1 / 128f * (p.Center - Projectile.Center);
                }
                else if (Projectile.whoAmI != p.whoAmI && p.active && (p.DamageType != DamageClass.Summon || p.hostile) && (Projectile.Center - p.Center).Length() < 128)
                {
                    // avoid projectiles
                    separation += 8f * (Projectile.Center - p.Center).SafeNormalize(Vector2.Zero) / (Projectile.Center - p.Center).Length();
                }
            }

            if (!Main.LocalPlayer.dead && (Projectile.Center - Main.LocalPlayer.Center).Length() > 200)
            {
                // return to player
                separation -= 4f * (Projectile.Center - Main.LocalPlayer.Center).SafeNormalize(Vector2.Zero) / (Projectile.Center - Main.LocalPlayer.Center).Length();
            }

            if (count > 0)
            {
                alignment /= count;
                cohesion /= count;
            }

            Projectile.velocity += separation + alignment + cohesion;
        }

        private void GeneralBehavior(Player owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition)
        {
            Vector2 idlePosition = new Vector2(-48, 0);

            int minionsOfSameType = 0;
            foreach (Projectile p in Main.projectile)
            {
                if (p.owner == Projectile.owner && p.type == Projectile.type)
                {
                    minionsOfSameType++;
                }
            }
            if (minionsOfSameType > 0)
            {
                idlePosition = idlePosition.RotatedBy(2 * MathF.PI / minionsOfSameType * (Projectile.minionPos + 1));
            }
            idlePosition += owner.Center;

            vectorToIdlePosition = idlePosition - Projectile.Center;
            distanceToIdlePosition = vectorToIdlePosition.Length();

            if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 2000f)
            {
                Projectile.position = idlePosition;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }

            float overlapVelocity = 0.04f;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile other = Main.projectile[i];

                if (i != Projectile.whoAmI && other.active && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
                {
                    if (Projectile.position.X < other.position.X)
                    {
                        Projectile.velocity.X -= overlapVelocity;
                    }
                    else
                    {
                        Projectile.velocity.X += overlapVelocity;
                    }

                    if (Projectile.position.Y < other.position.Y)
                    {
                        Projectile.velocity.Y -= overlapVelocity;
                    }
                    else
                    {
                        Projectile.velocity.Y += overlapVelocity;
                    }
                }
            }
        }

        private void SearchForTargets(Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter)
        {
            // Starting search distance
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

        private void Movement(bool foundTarget, float distanceFromTarget, Vector2 targetCenter, float distanceToIdlePosition, Vector2 vectorToIdlePosition)
        {
            float speed = 8f;
            float inertia = 20f;

            if (foundTarget)
            {
                if (distanceFromTarget > 40f)
                {
                    Vector2 direction = targetCenter - Projectile.Center;
                    direction.Normalize();
                    direction *= speed;

                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
                }
            }
            else
            {
                if (distanceToIdlePosition > 600f)
                {
                    speed = 12f;
                    inertia = 60f;
                }
                else
                {
                    speed = 4f;
                    inertia = 80f;
                }

                if (distanceToIdlePosition > 20f)
                {

                    vectorToIdlePosition.Normalize();
                    vectorToIdlePosition *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
                }
                else if (Projectile.velocity == Vector2.Zero)
                {
                    Projectile.velocity.X = -0.15f;
                    Projectile.velocity.Y = -0.05f;
                }
            }
        }

        private void Visuals()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
    }
}
