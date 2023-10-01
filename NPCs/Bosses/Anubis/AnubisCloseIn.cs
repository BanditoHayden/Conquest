
using Conquest.Assets.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace Conquest.NPCs.Bosses.Anubis;

public class AnubisTarget : ModProjectile
{
    public override string Texture => $"{nameof(Conquest)}/NPCs/Bosses/Anubis/AnubisBeam";

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50;    //The length of old position to be recorded
        ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
    }

    public override void SetDefaults()
    {
        Projectile.width = 22;               //The width of projectile hitbox
        Projectile.height = 22;              //The height of projectile hitbox
        Projectile.hostile = false;         //Can the projectile deal damage to the player?
                                           //projectile.minion = true;           //Is the projectile shoot by a ranged weapon?
        Projectile.penetrate = 2;           //How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
        Projectile.alpha = 255;           
        Projectile.light = 0.5f;
        Projectile.timeLeft = 480;
        Projectile.extraUpdates = 1;
        Projectile.ignoreWater = true;          
        Projectile.tileCollide = false;                                 
    }
    public override bool PreDraw(ref Color lightColor)
    {
        default(Effects.PurpleTrail).Draw(Projectile);

        return false;
    }

    float angle;
    Vector2 effectiveVelocity, oldPosition;

    public override void AI()
    {
        Player player = Main.player[Projectile.owner];

        Vector2 target = player.Center + new Vector2(40, 0).RotatedBy(angle);
        angle += MathHelper.ToRadians(3f);

        Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.Center.DirectionTo(target) * 12f, 0.11f);

        Projectile.ai[0]++;
    }

    public override void OnKill(int timeLeft)
    {
        if (!Main.dedServ)
        {
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.oldPos[i], 1, 1, DustID.PurpleTorch, Scale: 1.25f);
                d.velocity = Vector2.Zero;
                d.noGravity = true;
            }
        }
    }

    public override bool? CanCutTiles()
    {
        return false;
    }
}