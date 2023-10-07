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

namespace Conquest.Projectiles.Magic
{
    public class Brain : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCHit13, Projectile.position);
            for (int i = 0; i < 50; i++)
            {
                Vector2 outerdustring = Main.rand.NextVector2CircularEdge(1f, 1f);
                Dust dusty = Dust.NewDustPerfect(Projectile.position, DustID.LifeDrain, outerdustring * 5, Scale: 1f);
                dusty.noGravity = true;
            }
        }
    }
}
