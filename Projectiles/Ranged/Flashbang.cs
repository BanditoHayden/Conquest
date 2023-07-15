﻿using Microsoft.Xna.Framework.Graphics;
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

namespace Conquest.Projectiles.Ranged
{
    internal class Flashbang : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("闪光震撼弹");
        }
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Default;
            Projectile.penetrate = 1;
            Projectile.aiStyle = ProjAIStyleID.GroundProjectile;
            Projectile.timeLeft = 300;
            Projectile.alpha = 0;
            Projectile.light = 0;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        private int boomCounter = 0;
        public override void AI()
        {
            if (++boomCounter >= 90)
            {
                boomCounter = 0;
                Player player = Main.player[Projectile.owner];
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 0, ModContent.ProjectileType<FlashbangLight>(), 1, 0, Projectile.owner, 0, 0);
                Projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 0, ModContent.ProjectileType<FlashbangLight>(), 1, 0, Projectile.owner, 0, 0);
            Projectile.Kill();
        }
    }
}
