using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.DataStructures;

using Conquest.Dusts;
using Conquest.Assets.Common;

namespace Conquest.Projectiles.Ranged
{
    public class DynastyProj : ModProjectile
    {
        public Vector2 initialCenter;

        // This field is used as a counter for the wave motion
        public int sineTimer;

        // This field "offsets" the progress along the wave
        public float waveOffset;
        private float waveDirection = 1f;

        public Color drawColor = Color.Black;


        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            Main.projFrames[Type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;

            Projectile.aiStyle = 0;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 9;
            Projectile.timeLeft = 300;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            initialCenter = Projectile.Center;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return drawColor * 0.64f;
        }

        public override void AI()
        {
            Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();

            float velocityLength = Projectile.velocity.Length();

            if (velocityLength > 0)
            {
                // How long one wave should be, measured in pixels
                // A stride of 300 pixels is 18.75 tiles
                float waveStride = 300 * waveDirection;

                float waveProgress = sineTimer * velocityLength / waveStride + waveOffset;  // 1 for each full sine wave
                float radians = waveProgress * MathHelper.TwoPi;  // Convert the wave progress into an angle for MathF.Sin()
                float sine = MathF.Sin(radians) * Projectile.direction;

                // Using the calculated sine value, generate an offset used to position the projectile on the wave
                // The offset should be perpendicular to the velocity direction, hence the RotatedBy call
                Vector2 offset = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2 * -1);

                // How wide the wave should be, times two
                // An amplitude of 32 pixels is 2 tiles, meaning the total wave width is 64 pixels, or 4 tiles
                float waveAmplitude = 32;

                // Having the projectiles spawn offset from the player might not be ideal.  To fix that, let's reduce the amplitude when the projectile is freshly spawned
                if (sineTimer < 20)
                {
                    // Up to 1/3rd of a second (20/60 = 1/3), make the amplitude grow to the intended size
                    float factor = 1f - sineTimer / 20f;
                    waveAmplitude *= 1f - factor * factor;
                }

                // Get the offset used to adjust the projectile's position
                offset *= sine * waveAmplitude;

                // Update the position manually since ShouldUpdatePosition returns false
                initialCenter += Projectile.velocity;
                Projectile.Center = initialCenter + offset;

                // Update the rotation used to draw the projectile
                // This projectile should act as if it were moving along the sine wave.
                // The rotation can be calculated using the cosine value, which is the slope of the sine wave, and then stretching/squishing the slope based on the amplitude and wave frequency.
                // The slope needs to be inverted due to negative slopes being "upwards" in Terraria's world space.
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
                // Failsafe for when the velocity is 0
                Projectile.frame = 1;
                Projectile.rotation = 0;
            }

            // Spawn dusts
            //Dust.NewDustPerfect(Projectile.Center, DustID.SteampunkSteam, Velocity: Vector2.Zero, newColor: drawColor, Scale: 0.7f);

            sineTimer++;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 25; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GhostDust>(), speed * 5, Scale: 0.8f, newColor: drawColor);
                d.noGravity = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (drawColor == Color.DarkGoldenrod)
            {
                default(Effects.GoldTrail).Draw(Projectile);
            }
            else
            {
                default(Effects.BlackTrail).Draw(Projectile);
            }

            return false;
        }
    }
}

