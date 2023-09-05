using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Conquest.Assets.Common;

namespace Conquest.Projectiles.Summoner;

internal class HealingSun : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 10;
        Projectile.height = 10;
        Projectile.friendly = false;
        Projectile.hostile = false;
        Projectile.timeLeft = 450;
        Projectile.light = 1f;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
    }
    public override void AI()
    {
        float rotationBonus = Projectile.velocity.Length();
        rotationBonus *= (Projectile.velocity.X > 0) ? 1 : -1;
        Projectile.rotation += MathHelper.ToRadians(rotationBonus);
        Projectile.velocity *= 0.94f;

        foreach (Player player in Main.player)
        {
            if (!player.active || player.dead || player.Distance(Projectile.Center) > 300) continue;
            if (player.Hitbox.Intersects(Projectile.Hitbox))
            {
                player.Heal(50);
                Projectile.Kill();
            }
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(player.Center) * 8, 0.11f);
        }

    }
}
