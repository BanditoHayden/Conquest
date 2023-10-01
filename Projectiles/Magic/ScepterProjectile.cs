
using Conquest.Assets.Common;
using Conquest.Items.Dev;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace Conquest.Projectiles.Magic
{
    public class ScepterProjectile : ModProjectile
    {
        public override void SetDefaults()
        {

            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 60;
        }
        public override void AI()
        {

            var settings = new ParticleOrchestraSettings
            {
                PositionInWorld = Projectile.Center,
                MovementVector = Projectile.velocity,
                UniqueInfoPiece = ModContent.ItemType<ItemTransfer1>(),
            };
            if (Projectile.timeLeft == 60)
            {
                ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.ItemTransfer, settings);
            }
        }
        public override void OnKill(int timeLeft)
        {
            Vector2 perturbedSpeed = new Vector2(0, -6).RotatedByRandom(MathHelper.ToRadians(360));
            switch (Main.rand.Next(3))
            {
                case 0:
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), new Vector2(Projectile.Center.X, Projectile.Center.Y), new Vector2(perturbedSpeed.X, perturbedSpeed.Y), ModContent.ProjectileType<Rune2>(), Projectile.damage, 0, Projectile.owner);
                    break;
                case 1:
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), new Vector2(Projectile.Center.X, Projectile.Center.Y), new Vector2(perturbedSpeed.X, perturbedSpeed.Y), ModContent.ProjectileType<Rune1>(), Projectile.damage, 0, Projectile.owner);
                    break;
                case 2:
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), new Vector2(Projectile.Center.X, Projectile.Center.Y), new Vector2(perturbedSpeed.X, perturbedSpeed.Y), ModContent.ProjectileType<Rune3>(), Projectile.damage * 2, 0, Projectile.owner);
                    break;
            }
        }
    }
    public class Rune1 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;    //The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 180;
            Projectile.light = 0.5f;
        }
        int timer;
        public override void AI()
        {
            timer++;
            if (timer <= 59)
            {
                Projectile.velocity = Vector2.Zero;
            }
            if (timer >= 60)
            {
                Projectile.alpha = 255;
                float maxDetectRadius = 500f;
                float projSpeed = 18f;
                Projectile.rotation = Projectile.velocity.ToRotation();
                NPC closestNPC = FindClosestNPC(maxDetectRadius);
                if (closestNPC == null)
                    return;

                Projectile.velocity = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
            }

        }
        public override void OnSpawn(IEntitySource source)
        {
            var settings = new ParticleOrchestraSettings
            {
                PositionInWorld = Projectile.Center,
                MovementVector = Projectile.velocity,
            };
            ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.ShimmerTownNPCSend, settings);
        }

        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];

                if (target.CanBeChasedBy())
                {
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            default(Effects.WhiteTrail).Draw(Projectile);

            return true;
        }
    }
    public class Rune2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;    //The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 240;
            Projectile.light = 0.5f;
        }
        int timer;
        public override void AI()
        {
            timer++;
            if (timer <= 59)
            {
                Projectile.velocity = Vector2.Zero;
            }
            if (timer >= 60)
            {
                Projectile.alpha = 255;
                float maxDetectRadius = 500f;
                float projSpeed = 9f;
                Projectile.rotation = Projectile.velocity.ToRotation();
                NPC closestNPC = FindClosestNPC(maxDetectRadius);
                if (closestNPC == null)
                    return;

                Projectile.velocity = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
            }

        }
        public override void OnSpawn(IEntitySource source)
        {
            var settings = new ParticleOrchestraSettings
            {
                PositionInWorld = Projectile.Center,
                MovementVector = Projectile.velocity,
            };
            ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.ShimmerTownNPCSend, settings);
        }

        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];

                if (target.CanBeChasedBy())
                {
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            default(Effects.GoldTrail).Draw(Projectile);

            return true;
        }
    }
    public class Rune3 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40;    //The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.penetrate = 5;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 360;
            Projectile.light = 0.5f;
        }
        int timer;
        public override void AI()
        {
            timer++;
            if (timer <= 59)
            {
                Projectile.velocity = Vector2.Zero;
            }
            if (timer >= 60)
            {
                Projectile.alpha = 255;
                float maxDetectRadius = 500f;
                float projSpeed = 3f;
                Projectile.rotation = Projectile.velocity.ToRotation();
                NPC closestNPC = FindClosestNPC(maxDetectRadius);
                if (closestNPC == null)
                    return;

                Projectile.velocity = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
            }

        }
        public override void OnSpawn(IEntitySource source)
        {
            var settings = new ParticleOrchestraSettings
            {
                PositionInWorld = Projectile.Center,
                MovementVector = Projectile.velocity,
            };
            ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.ShimmerTownNPCSend, settings);
        }

        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];

                if (target.CanBeChasedBy())
                {
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            default(Effects.RoyalBlueTrail).Draw(Projectile);

            return true;
        }
    }
}
