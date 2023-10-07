using Terraria.GameContent.Creative;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;


namespace Conquest.Projectiles.Melee
{
    public class Tornado : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 26;
            Projectile.aiStyle = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 120;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = true;

        }


        public static void ProjRotateAroundSomething(Projectile proj, double dist, float rotateAmount, Vector2 rotationPos)
        {
            double deg = proj.ai[1];
            double rad = deg * (Math.PI / 180);
            proj.ai[1] += rotateAmount;

            proj.position.X = rotationPos.X - (int)(Math.Cos(rad) * dist) - proj.width / 2;
            proj.position.Y = rotationPos.Y - (int)(Math.Sin(rad) * dist) - proj.height / 2;
            proj.netUpdate = true;
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 10)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
            Projectile.ai[0] += 1f;
              if (Projectile.ai[0] >= 1f)
            {
                Projectile.velocity *= 1.00f;
            }
            if (Projectile.ai[0] >= 60f)
            {
                Projectile.velocity = Vector2.Zero;
            }
          
            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation += MathHelper.Pi;

            }
        }
        
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.velocity = Vector2.Zero;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SteampunkSteam);
            }
        }
    }
}
