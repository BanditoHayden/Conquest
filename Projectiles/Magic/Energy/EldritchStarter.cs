
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

namespace Conquest.Projectiles.Magic.Energy;

public class EldritchStarter : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50;    //The length of old position to be recorded
        ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
    }
    public override void SetDefaults()
    {
        Projectile.width = 8;
        Projectile.height = 8;
        Projectile.aiStyle = 0;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.penetrate = 3;
        Projectile.timeLeft = 120;
        Projectile.alpha = 255;
        Projectile.light = 0.5f;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = true;
        Projectile.extraUpdates = 2;
        Projectile.alpha = 255;
        AIType = ProjectileID.Bullet;
    }

    public override void AI()
    {
        if (Projectile.ai[0]++ % 4 == 0)
        {
            Vector2 testPosition = new Vector2(0, 2);
            while (Collision.CanHitLine(Projectile.Center, 1, 1, Projectile.Center + testPosition, 1, 1))
            {
                testPosition.Y += 2;
            }

            Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(),
                Projectile.Center + testPosition,
                (Projectile.Center + testPosition).DirectionTo(Projectile.Center + Projectile.velocity) * 10f,
                ModContent.ProjectileType<EldritchWave>(), Projectile.damage, Projectile.knockBack);
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        default(Effects.RoyalBlueTrail).Draw(Projectile);

        return false;
    }

}
