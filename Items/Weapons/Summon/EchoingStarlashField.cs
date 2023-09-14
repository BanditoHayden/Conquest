using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Conquest.Assets.Common;
using Conquest.Buffs;

namespace Conquest.Projectiles.Summoner;

public class EchoingStarlashField : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileID.Sets.TrailCacheLength[Type] = 30;
    }

    public override void SetDefaults()
    {
        Projectile.friendly = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.damage = 0;
        Projectile.DamageType = DamageClass.Summon;
        Projectile.width = 2;
        Projectile.height = 2;
        Projectile.timeLeft = 300;
        Projectile.penetrate = 999;
        Projectile.scale = 1f;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        default(Effects.RoyalBlueTrail).Draw(Projectile);
        return false;
    }

    public override void AI()
    {
        Projectile.rotation = Projectile.velocity.ToRotation();

        foreach (NPC npc in Main.npc)
        {
            if (npc.active && !npc.dontTakeDamage && !npc.friendly)
            {
                if (npc.Hitbox.Intersects(Projectile.Hitbox))
                {
                    npc.AddBuff(ModContent.BuffType<StardustLightning>(), 60);
                }
            }
        }

        Projectile target = Main.projectile[(int)Projectile.ai[1]];

        if (Projectile.Hitbox.Intersects(target.Hitbox)) Projectile.Kill();

        Projectile.velocity = Vector2.Lerp(
            Projectile.velocity, Projectile.Center.DirectionTo(target.Center) * 8, 0.14f);
    }
}

