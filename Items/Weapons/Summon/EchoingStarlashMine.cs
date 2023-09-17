using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Conquest.Buffs;

namespace Conquest.Projectiles.Summoner;

public class EchoingStarlashMine : ModProjectile
{

    int myDelay = 30;
    public override void SetDefaults()
    {
        Projectile.friendly = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.damage = 35;
        Projectile.DamageType = DamageClass.Summon;
        Projectile.width = 7;
        Projectile.height = 7;
        Projectile.timeLeft = 120;
        Projectile.penetrate = 999;
        Projectile.scale = 1.5f;
        myDelay = Main.rand.Next(20, 41);
    }

    public override void AI()
    {
        if (!Main.dedServ)
        {
            Dust d = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.IceTorch);
            d.noGravity = true;
            d.velocity = Vector2.Zero;
        }
        Projectile.rotation += MathHelper.ToRadians(2 * Projectile.velocity.X);
        Projectile.velocity *= 0.97f;


        foreach (Projectile p in Main.projectile)
        {
            if (p.active && p != Projectile && p.type == Projectile.type && p.Distance(Projectile.Center) < 300
             && Projectile.ai[0] % myDelay == 0)
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<EchoingStarlashField>(), 0, 0, Projectile.owner, ai1: p.whoAmI);
            }
        }

        Projectile.ai[0]++;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Projectile.velocity *= -0.1f;
    }

    public override void Kill(int timeLeft)
    {
        if (!Main.dedServ)
        {
            for (int i = 0; i < 4; i++)
            {
                Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.IceTorch).noGravity = true;
            }
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.velocity.X != oldVelocity.X) Projectile.velocity.X *= -1;
        if (Projectile.velocity.Y != oldVelocity.Y) Projectile.velocity.Y *= -1;
        return false;
    }
}

