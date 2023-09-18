
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

namespace Conquest.Projectiles.Ranged;

public class TRevolverShot : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2000;    //The length of old position to be recorded
        ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
    }

    List<NPC> NPCs = new List<NPC>();
    public override void SetDefaults()
    {
        Projectile.width = 8;
        Projectile.height = 8;
        Projectile.aiStyle = 1;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.penetrate = 999;
        Projectile.alpha = 255;
        Projectile.light = 0.5f;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = true;
        Projectile.extraUpdates = 20;
        Projectile.alpha = 255;
        AIType = ProjectileID.Bullet;

        Projectile.localNPCHitCooldown = 1;
        Projectile.usesLocalNPCImmunity = true;
    }
    bool tileCollided = false;
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile.velocity = Vector2.Zero;
        if (!tileCollided) NPCs = new List<NPC>();
        tileCollided = true;
        return recalled;
    }

    public override bool? CanHitNPC(NPC target)
    {
        return !NPCs.Contains(target);
    }

    Vector2 ogVel = new Vector2(0, 0);
    bool recalled = false;
    public override void AI()
    {
        if (Projectile.velocity.Length() > float.Epsilon) ogVel = Projectile.velocity;
        else if (!Main.dedServ)
        {
            Dust d = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.PurpleTorch);
            d.noGravity = true;
            d.velocity = Vector2.Zero;
        }
        if (Main.player[Projectile.owner].controlUseTile && !recalled)
        {
            recalled = true;
            Projectile.velocity = ogVel * -1;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (Projectile.velocity.Length() > float.Epsilon) default(Effects.PurpleTrailSmall).Draw(Projectile);

        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        NPCs.Add(target);
    }
}
