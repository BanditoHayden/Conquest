
using Conquest.Assets.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Conquest.Projectiles.Melee
{
    public class UnusedAeroWind : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50;    //The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 900;
            Projectile.alpha = 255;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
            AIType = ProjectileID.Bullet;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.98f;
            if (Projectile.velocity.LengthSquared() < 0.1f) Projectile.Kill();

            Dust d = Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), 1, 1, DustID.FireworksRGB, Scale: 0.5f, newColor: Color.White);
            d.noGravity = true;
            d.velocity = Projectile.velocity / 2f;
        }


    }
}
