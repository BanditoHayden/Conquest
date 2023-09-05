using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Conquest.Projectiles.Melee
{
    public class SwarmYoyo : ModProjectile
    {
        public override void SetStaticDefaults()
        {
         //   ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 2.5f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 450f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 22;
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = -1;

        }
        public override void SetDefaults()
        {
            Projectile.extraUpdates = 0;
            Projectile.width = 18;
            Projectile.height = 20;
            Projectile.aiStyle = 99;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.scale = 1f;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];

            
            if (Main.rand.NextBool(3))
            {
                for (int i = 0; i < 3; i++)
                {
                    SpawnAWasp();
                }
            }
            SpawnAWasp();
        }

        public void SpawnAWasp()
        {
            Vector2 perturbedSpeed = new Vector2(0, -6).RotatedByRandom(MathHelper.ToRadians(360));
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), new Vector2(Projectile.Center.X, Projectile.Center.Y), new Vector2(perturbedSpeed.X, perturbedSpeed.Y), ProjectileID.Wasp, (2 * Projectile.damage) / 3, 0, Projectile.owner);
        }
    }
}
