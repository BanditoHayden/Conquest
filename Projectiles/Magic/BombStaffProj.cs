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
using Conquest.Assets.Common;

namespace Conquest.Projectiles.Magic
{
    public class BombStaffProj : ModProjectile
    {

        private ref float boomCounter => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("炸弹");
            Main.projFrames[Projectile.type] = 2;

        }
        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.alpha = 0;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        

        public override void AI()
        {
            boomCounter++;
            if (++Projectile.frameCounter >= 30 && boomCounter < 90)
            {
                Projectile.frameCounter = 0;
                if (Projectile.frame == 0)
                {
                    Projectile.frame = 1;
                }
                else
                {
                    Projectile.frame = 0;
                }
            }
            if (++Projectile.frameCounter >= 15 && boomCounter >= 90 && boomCounter < 180)
            {
                Projectile.frameCounter = 0;
                if (Projectile.frame == 0)
                {
                    Projectile.frame = 1;
                }
                else
                {
                    Projectile.frame = 0;
                }
            }
            if (++Projectile.frameCounter >= 5 && boomCounter >= 180 && boomCounter < 270)
            {
                Projectile.frameCounter = 0;
                if (Projectile.frame == 0)
                {
                    Projectile.frame = 1;
                }
                else
                {
                    Projectile.frame = 0;
                }
            }
            if (boomCounter >= 270)
            {
                Projectile.frame = 1;
            }
            if (boomCounter >= 300)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 0, ModContent.ProjectileType<BoomX>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 0, ModContent.ProjectileType<BoomY>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                Projectile.Kill();
            }

        }
        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Main.myPlayer];
            MyPlayer modPlayer = player.GetModPlayer<MyPlayer>();
            player.GetModPlayer<MyPlayer>().ScreenShake = 12;
        }
        public override bool? CanDamage()
        {
            return false;
        }
    }
}
