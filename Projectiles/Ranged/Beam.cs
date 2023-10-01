using Conquest.Assets.Common;
using Conquest.Items.Weapons.Ranged;
using Conquest.Projectiles.HeldProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace Conquest.Projectiles.Ranged
{
    public class Beam : ModProjectile
    {
        private const float moving = 60f;

        public float setting
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }


        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.hide = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            var spriteBatch = Main.spriteBatch;

            laser(spriteBatch, texture, new Vector2(Main.player[Projectile.owner].Center.X, Main.player[Projectile.owner].Center.Y - 13f),
                    Projectile.velocity, 10, Projectile.damage, -1.57f, 1f, 2000f, Color.White, (int)moving);
            return false;
        }



        public void laser(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, Vector2 t, float state, int dmg, float rotation = 0f, float scale = 1f, float maximun = 1800f, Color col = default(Color), int transitional = 50)
        {
            float rotati = t.ToRotation() + rotation;

            for (float i = transitional; i <= setting; i += state)
            {
                Color c = Color.White;
                var origin = start + i * t;
                spriteBatch.Draw(texture, origin - Main.screenPosition,
                    new Rectangle(0, 26, 28, 26), i < transitional ? Color.Transparent : c, rotati,
                    new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);
            }

            spriteBatch.Draw(texture, start + t * (transitional - state) - Main.screenPosition,
                new Rectangle(0, 0, 28, 26), Color.White, rotati, new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);

            spriteBatch.Draw(texture, start + (setting + state) * t - Main.screenPosition,
                new Rectangle(0, 52, 28, 26), Color.White, rotati, new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 unit = Projectile.velocity;
            float point = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), player.Center,
                player.Center + unit * setting, 22, ref point);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 5;
        }

        public override void OnKill(int timeLeft)
        {
            Main.LocalPlayer.GetModPlayer<MyPlayer>().OutbreakShake = 0;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Update(player);
            laserloc(player);

            if (Main.LocalPlayer.GetModPlayer<MyPlayer>().BeamActive == 0)
            {
                Projectile.Kill();
            }
            else
            {
                Projectile.timeLeft = 2;
            }
            Projectile.position = player.Center + Projectile.velocity * moving;

            dusts(player);
            lights();
        }

        private void dusts(Player player)
        {
            Vector2 t = Projectile.velocity * -1;
            Vector2 position = player.Center + Projectile.velocity * setting;

            for (int i = 0; i < 2; ++i)
            {
                float rotation = Projectile.velocity.ToRotation() + (Main.rand.Next(2) == 1 ? -1.0f : 1.0f) * 1.57f;
                float randomize = (float)(Main.rand.NextDouble() * 0.8f + 1.0f);
                Vector2 dustVel = new Vector2((float)Math.Cos(rotation) * randomize, (float)Math.Sin(rotation) * randomize);
                Dust dust = Main.dust[Dust.NewDust(position, 0, 0, DustID.YellowStarDust, dustVel.X, dustVel.Y)];
                dust.noGravity = true;
                dust.scale = 3f;
            }
        }

        private void laserloc(Player player)
        {
            for (setting = moving; setting <= 2200f; setting += 5f)
            {
                var start = player.Center + Projectile.velocity * setting;
                if (!Collision.CanHit(player.Center, 1, 1, start, 1, 1))
                {
                    setting -= 5f;
                    break;
                }
            }
        }

        private void Update(Player player)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 difference = Main.MouseWorld - player.Center;
                difference.Normalize();
                Projectile.velocity = difference;
                Projectile.direction = Main.MouseWorld.X > player.position.X ? 1 : -1;
                Projectile.netUpdate = true;
            }
            int direct = Projectile.direction;
            player.ChangeDir(direct);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * direct, Projectile.velocity.X * direct);
        }

        private void lights()
        {
            DelegateMethods.v3_1 = new Vector3(0.8f, 0.8f, 1f);
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * (setting - moving), 26, DelegateMethods.CastLight);
        }

        public override bool ShouldUpdatePosition() => false;
        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 t = Projectile.velocity;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + t * setting, (Projectile.width + 16) * Projectile.scale, DelegateMethods.CutTiles);
        }
    }
}
