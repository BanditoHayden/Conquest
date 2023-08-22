
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

public class MarksmanSpawnBolt : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50;    //The length of old position to be recorded
        ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
    }
    int iAm;
    public override void SetDefaults()
    {
        Projectile.timeLeft = 75;
        iAm = 75;
        Projectile.width = 8;
        Projectile.height = 8;
        Projectile.aiStyle = 1;
        Projectile.friendly = false;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.penetrate = 3;
        Projectile.alpha = 255;
        Projectile.light = 0.5f;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.alpha = 255;
        AIType = ProjectileID.Bullet;
    }

    public override void AI()
    {
        Projectile.Center = Vector2.Lerp(Projectile.Center,
            Main.player[Projectile.owner].Center,
            1f - (Projectile.timeLeft * 1f / iAm));

        Projectile.rotation = Projectile.Center.DirectionTo(Main.player[Projectile.owner].Center).ToRotation();
    }

    public override bool PreDraw(ref Color lightColor)
    {
        default(Effects.RedTrail).Draw(Projectile);

        return false;
    }

}
