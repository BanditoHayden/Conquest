using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Conquest.Projectiles.Hostile
{
    public class SandExplosion : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.tileCollide = true;
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.hostile = true;
            Projectile.scale = 2;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 240;
            Projectile.aiStyle = 1;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.Kill();
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_GoblinBomb, Projectile.Center);
            for (int i = 0; i < 12; i++)
            {
                Vector2 velocity = new(Main.rand.NextFloat(1.6f, 6f), Main.rand.NextFloat(1.6f, 9f));
                velocity *= Main.rand.NextBool() ? -1f : 1f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<SandFall>(), Projectile.damage / 3, Projectile.knockBack / 3f, Projectile.owner);
            }


        }
    }
}
