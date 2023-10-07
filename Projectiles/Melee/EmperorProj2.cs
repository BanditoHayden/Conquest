using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Conquest.Projectiles.Melee
{
    public class EmperorProj2 : ModProjectile
    {
        public override void SetDefaults()
        {

            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 5;
        }
        public override void AI()
        {
            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3)
            {
                Projectile.tileCollide = false;

                Projectile.alpha = 255;
                Projectile.position = Projectile.Center;
                Projectile.Center = Projectile.position;
            }

            return;
        }
        public override void OnKill(int timeLeft)
        {

            for (int i = 0; i < 50; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Gold, speed * 5, Scale: 1.5f);
                d.noGravity = true;
            }

        }
    }
}
