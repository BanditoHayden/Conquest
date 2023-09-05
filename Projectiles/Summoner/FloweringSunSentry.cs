using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Conquest.Assets.Common;

namespace Conquest.Projectiles.Summoner;

public class FloweringSunSentry : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 15;
        Projectile.height = 24;
        Projectile.scale = 1f;
        Projectile.DamageType = DamageClass.Summon;
        Projectile.penetrate = -1;
        Projectile.sentry = true;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return false;
    }

    public override void AI()
    {
        Projectile.velocity.Y += 12;

        if (Projectile.ai[0] == 0) Projectile.ai[0] = (60 * 22);

        if (Projectile.ai[0]++ % (60 * 24) == 0)
        {
            SoundEngine.PlaySound(SoundID.Item29, Projectile.position);
            Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(),
                Projectile.position, Main.rand.NextVector2CircularEdge(15, 15),
                ModContent.ProjectileType<HealingSun>(), 0, 0, Projectile.owner);
        }
    }
}

