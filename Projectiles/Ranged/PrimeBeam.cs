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
using Terraria.Audio;
using Conquest.Buffs;
using Conquest.Assets.Common;

namespace Conquest.Projectiles.Ranged
{
    public class PrimeBeam : ModProjectile
    {
        int timer;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50; 
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 42;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.alpha = 255;
            Projectile.aiStyle = 1;
            Projectile.light = 0.3f;
            Projectile.extraUpdates = 1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            target.AddBuff(ModContent.BuffType<Electrified>(), 30);
        }

        public override void OnSpawn(IEntitySource source)
        {
            timer = 9;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            default(Effects.OutbreakTrail).Draw(Projectile);

            return true;
        }

        public override void AI()
        {
            Projectile.velocity *= 1.001f;
            Projectile.aiStyle = 0;
            if (timer > 0)
            {
                timer--;
            }

            if (timer == 0)
            {
                Projectile.alpha -= 255;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Vector2 speed = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(360));
                Dust dust1 = Dust.NewDustPerfect(Projectile.Center, DustID.YellowTorch, speed / 3, Scale: 2f);
                dust1.noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.Item94, Projectile.position);
        }
    }
}