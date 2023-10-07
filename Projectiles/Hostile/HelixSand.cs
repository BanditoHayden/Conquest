using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;

namespace Conquest.Projectiles.Hostile
{
    public class HelixSand : ModProjectile
    {
        public int sineTimer;
        public Vector2 initialCenter;
        public float waveOffset;
        public override void SetDefaults()
        {
            Projectile.tileCollide = true;
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.hostile = true;
            Projectile.scale = 2;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 240;
            Projectile.aiStyle = 1;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            initialCenter = Projectile.Center;
        }

        public override void AI()
        {
            Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();

            float velocityLength = Projectile.velocity.Length();

            if (velocityLength > 0)
            {
                float waveStride = 300f;

                float waveProgress = sineTimer * velocityLength / waveStride + waveOffset;  // 1 for each full sine wave
                float radians = waveProgress * MathHelper.TwoPi;  // Convert the wave progress into an angle for MathF.Sin()
                float sine = MathF.Sin(radians) * Projectile.direction;

                Vector2 offset = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2 * -1);

                float waveAmplitude = 32;

                if (sineTimer < 20)
                {
                    float factor = 1f - sineTimer / 20f;
                    waveAmplitude *= 1f - factor * factor;
                }

                offset *= sine * waveAmplitude;

                initialCenter += Projectile.velocity;
                Projectile.Center = initialCenter + offset;
                float cosine = MathF.Cos(radians) * Projectile.direction;
                Projectile.rotation = Projectile.velocity.ToRotation() + MathF.Atan(-1 * cosine * waveAmplitude * velocityLength / waveStride);

                // Update the frame used to draw the projectile
                const float sineOf60Degrees = 0.866025404f;
                if (sine > sineOf60Degrees)
                {
                    Projectile.frame = Projectile.direction == 1 ? 0 : 2;
                }
                else if (sine < -sineOf60Degrees)
                {
                    Projectile.frame = Projectile.direction == 1 ? 2 : 0;
                }
                else
                {
                    Projectile.frame = 1;
                }
            }
            else
            {
                Projectile.frame = 1;
                Projectile.rotation = 0;
            }
            sineTimer++;
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
