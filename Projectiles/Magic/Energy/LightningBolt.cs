
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

public class LightningBolt : ModProjectile
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
        Projectile.timeLeft = 45;
        Projectile.alpha = 255;
        Projectile.light = 0.5f;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = true;
        Projectile.extraUpdates = 3;
        Projectile.alpha = 255;
        AIType = ProjectileID.Bullet;
    }

    float rotation = 0;
    public override void AI()
    {
        if (Projectile.ai[0]++ == 0)
        {
            rotation = Projectile.velocity.ToRotation();
        }
        if (Main.rand.NextBool(14))
        {
            float range = MathHelper.ToRadians(Main.rand.NextFloat(7f, 15f));
            range *= Main.rand.NextBool() ? 1 : -1;
            Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0).RotatedBy(rotation + range);
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        default(Effects.LightningTrail).Draw(Projectile);

        return false;
    }

}
