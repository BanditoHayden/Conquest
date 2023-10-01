using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using static Terraria.ModLoader.PlayerDrawLayer;


namespace Conquest.Projectiles.Ranged
{
    public class CowboyGun2 : ModProjectile
    {
        int timer;
        int fired;

        float rotationRate;

        public override void SetDefaults()
        {
            Projectile.width = 46; // The width of projectile hitbox
            Projectile.height = 20; // The height of projectile hitbox

            Projectile.aiStyle = 0;
            Projectile.DamageType = DamageClass.Ranged; // What type of damage does this projectile affect?
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = false; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = true; // Can the projectile collide with tiles?
            Projectile.timeLeft = 120; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)

            Projectile.penetrate = 999;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);

            // If the projectile hits the left or right side of the tile, reverse the X velocity
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
            {
                Projectile.velocity.X = -oldVelocity.X * 0.9f;
            }

            // If the projectile hits the top or bottom side of the tile, reverse the Y velocity
            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
            {
                Projectile.velocity.Y = -oldVelocity.Y * 0.9f;
            }
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (MathF.Abs(Projectile.position.X - target.position.X) < target.width * 0.6f)
            {
                Projectile.velocity.X *= -0.9f;
            }
            if (MathF.Abs(Projectile.position.Y - target.position.Y) < target.height * 0.6f)
            {
                Projectile.velocity.X *= 0.9f;
            }
        }

        public float EaseLerp(float x)
        {
            return ((-1f / 3600f) * (x - 60f) * (x - 60f)) + 1;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (Projectile.ai[0] == 0) Projectile.rotation = Main.rand.NextFloat(2 * MathF.PI);

            rotationRate = ((1f / 169f) * (Projectile.ai[0] - 60) * (Projectile.ai[0] - 60));
            float rotationValue1 = Projectile.rotation + (2 * MathHelper.ToRadians(rotationRate));

            NPC rTarget = null;
            foreach (NPC npc in Main.npc)
            {
                if (!npc.friendly && npc.active && npc.Distance(Projectile.position) < 600 && !npc.dontTakeDamage && npc.type != NPCID.TargetDummy)
                {
                    rTarget = npc;
                    break;
                }
            }

            if (rTarget != null)
            {
                float rotationValue2 = Projectile.DirectionTo(rTarget.Center).ToRotation();
                Projectile.rotation = MathHelper.Lerp(rotationValue1, rotationValue2, EaseLerp(Projectile.ai[0]));
            }
            else Projectile.rotation = rotationValue1;


            if (Projectile.ai[0] == 60)
            {
                NPC target = null;
                foreach (NPC npc in Main.npc)
                {
                    if (!npc.friendly && npc.active && npc.Distance(Projectile.position) < 600 && !npc.dontTakeDamage && npc.type != NPCID.TargetDummy)
                    {
                        target = npc;
                        break;
                    }
                }
                if (target != null)
                {
                    Vector2 position = Projectile.Center;
                    float speed = 20f;
                    Vector2 direction = Projectile.Center.DirectionTo(target.Center);
                    Vector2 velocity = (direction * speed) + target.velocity;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, velocity, ProjectileID.Bullet, Projectile.damage, 0f, owner.whoAmI, fired = 1);
                    SoundEngine.PlaySound(SoundID.Item11, Projectile.Center);
                }
            }

            Projectile.ai[0]++;

            Projectile.velocity.Y += 0.13f;

            /*
            SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
            if (foundTarget)
            {
                Projectile.rotation = (targetCenter - Projectile.position).ToRotation();
                Projectile.velocity = Vector2.Zero;
                Vector2 position = Projectile.Center;
                float speed = 20f;
                Vector2 direction = Vector2.Normalize(targetCenter - Projectile.Center);
                Vector2 velocity = direction * speed;
                if (fired != 1)
                {
                    timer++;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, velocity, ProjectileID.Bullet, Projectile.damage, 0f, owner.whoAmI, fired = 1);
                    SoundEngine.PlaySound(SoundID.Item11, Projectile.Center);
                }

                if (timer >= 30)
                {
                    Projectile.Kill();
                }
                Projectile.ai[0]++;


            }
            else
            {

            }
            */
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SteampunkSteam);
            }
        }
        private void SearchForTargets(Player owner, out bool foundTarget, out float distanceFromTarget, out NPC target)
        {
            // Starting search distance
            distanceFromTarget = 700f;
            target = null;
            foundTarget = false;

            // This code is required if your minion weapon has the targeting feature
            if (owner.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[owner.MinionAttackTargetNPC];
                float between = Vector2.Distance(npc.Center, Projectile.Center);

                // Reasonable distance away so it doesn't target across multiple screens
                if (between < 2000f)
                {
                    distanceFromTarget = between;
                    target = npc;
                    foundTarget = true;
                }
            }

            if (!foundTarget)
            {
                // This code is required either way, used for finding a target
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];

                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, Projectile.Center);
                        bool closest = Vector2.Distance(Projectile.Center, target.Center) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                        bool closeThroughWall = between < 100f;

                        if ((closest && inRange || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            target = npc;
                            foundTarget = true;
                        }
                    }
                }
            }



        }
    }
}


