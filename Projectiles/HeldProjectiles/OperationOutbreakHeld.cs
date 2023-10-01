using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Conquest.Projectiles.Ranged;
using Terraria.DataStructures;
using Conquest.Assets.Common.Sfx;
using Conquest.Buffs;
using Conquest.Assets.Common;

namespace Conquest.Projectiles.HeldProjectiles
{
    public class OperationOutbreakHeld : ModProjectile
    {
        int shootAccelerate;

        bool spam = false;

        bool laser = false;

        int spamCD;

        int overheat;

        public override void OnSpawn(IEntitySource source)
        {
            shootAccelerate = 30;
            SoundEngine.PlaySound(Audio.PrimeGun);
            for (int i = 0; i < 100; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                Dust dust1 = Dust.NewDustPerfect(Projectile.Center, DustID.YellowStarDust, speed * 8, Scale: 1f);
                dust1.noGravity = true;
            }
        }

        public override void SetStaticDefaults()
        {
            //hmm
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.width = 56;
            Projectile.height = 56;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.knockBack = 4;
            Projectile.ownerHitCheck = true;
            Projectile.scale = 1.2f;
        }

        public override void OnKill(int timeLeft)
        {
            Main.LocalPlayer.GetModPlayer<MyPlayer>().BeamActive = 0;
        }

        public override bool? CanDamage()
        {
            return false;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (shootAccelerate > 0)
            {
                shootAccelerate--;
            }

            if (spamCD > 0)
            {
                spamCD--;
            }

            if (shootAccelerate == 0)
            {
                Vector2 direction = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(6));
                SoundEngine.PlaySound(SoundID.Item91, player.position);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), new Vector2(Projectile.Center.X, Projectile.Center.Y - 5f), direction, ModContent.ProjectileType<PrimeBeam>(), 400, 3, Main.myPlayer);
                shootAccelerate = 1000;
            }
            if (shootAccelerate == 975)
            {
                Vector2 direction = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(6));
                SoundEngine.PlaySound(SoundID.Item91, player.position);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), new Vector2(Projectile.Center.X, Projectile.Center.Y - 5f), direction, ModContent.ProjectileType<PrimeBeam>(), 400, 3, Main.myPlayer);
            }
            if (shootAccelerate == 955)
            {
                Vector2 direction = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(6));
                SoundEngine.PlaySound(SoundID.Item91, player.position);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), new Vector2(Projectile.Center.X, Projectile.Center.Y - 5f), direction, ModContent.ProjectileType<PrimeBeam>(), 400, 3, Main.myPlayer);
            }
            if (shootAccelerate == 940)
            {
                Vector2 direction = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(6));
                SoundEngine.PlaySound(SoundID.Item91, player.position);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), new Vector2(Projectile.Center.X, Projectile.Center.Y - 5f), direction, ModContent.ProjectileType<PrimeBeam>(), 250, 3, Main.myPlayer);
            }
            if (shootAccelerate == 930)
            {
                Vector2 direction = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(6));
                SoundEngine.PlaySound(SoundID.Item91, player.position);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), new Vector2(Projectile.Center.X, Projectile.Center.Y - 5f), direction, ModContent.ProjectileType<PrimeBeam>(), 400, 3, Main.myPlayer);
            }
            if (shootAccelerate == 925)
            {
                Vector2 direction = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(6));
                SoundEngine.PlaySound(SoundID.Item91, player.position);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), new Vector2(Projectile.Center.X, Projectile.Center.Y - 5f), direction, ModContent.ProjectileType<PrimeBeam>(), 400, 3, Main.myPlayer);
                spam = true;
                spamCD = 5;
            }
            if (shootAccelerate == 825)
            {
                spam = false;
                laser = true;
                Main.LocalPlayer.GetModPlayer<MyPlayer>().BeamActive = 1;
                Main.LocalPlayer.GetModPlayer<MyPlayer>().OutbreakShake = 700;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), new Vector2(Projectile.Center.X, Projectile.Center.Y - 5f), Projectile.velocity, ModContent.ProjectileType<Beam>(), 800, 3, Main.myPlayer);
                SoundEngine.PlaySound(SoundID.Zombie104, player.position);
            }
            if (shootAccelerate == 605)
            {
                Main.LocalPlayer.GetModPlayer<MyPlayer>().BeamActive = 0;
                shootAccelerate = 240;
                overheat = 0;
                SoundEngine.PlaySound(Audio.Overheat);
                player.AddBuff(ModContent.BuffType<Overheat>(), 240);
                laser = false;
            }

            if (laser == true && spamCD == 0)
            {
                overheat++;
                spamCD = 5;
                //Projectile.NewProjectile(Projectile.GetSource_FromThis(), new Vector2(Projectile.Center.X, Projectile.Center.Y - 5f), Projectile.velocity, ModContent.ProjectileType<Air>(), 0, 0, Main.myPlayer);
            }

            if (spam == true && spamCD == 0)
            {
                Vector2 direction = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(6));
                SoundEngine.PlaySound(SoundID.Item91, player.position);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), new Vector2(Projectile.Center.X, Projectile.Center.Y - 5f), direction, ModContent.ProjectileType<PrimeBeam>(), 51, 3, Main.myPlayer);
                overheat++;
                spamCD = 5;
            }

            if (overheat > 17)
            {
                int dddust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke);
                Main.dust[dddust].scale = 1f;
                Main.dust[dddust].noGravity = false;
            }

            if (overheat > 45)
            {
                int dddust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
                Main.dust[dddust].scale = 2f;
                Main.dust[dddust].noGravity = false;
            }

            if (player.HasBuff(ModContent.BuffType<Overheat>()))
            {
                int dddust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke);
                Main.dust[dddust].scale = 2f;
                Main.dust[dddust].noGravity = false;
            }

            if (player.channel == false)
            {
                Projectile.Kill();
            }


            Vector2 rrp = player.RotatedRelativePoint(player.MountedCenter, true);

            bool stillInUse = player.channel && !player.noItems && !player.CCed;
            if (Projectile.owner == Main.myPlayer)
            {
                UpdatePlayerVisuals(player, rrp);

                UpdateAim(rrp, player.HeldItem.shootSpeed);

            }
            else if (!stillInUse)
                Projectile.timeLeft = 2;

        }
        private void UpdatePlayerVisuals(Player player, Vector2 playerhandpos)
        {
            Projectile.Center = playerhandpos;
            Projectile.spriteDirection = Projectile.direction;

            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 12;
            player.itemAnimation = 12;

            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();

        }
        private void UpdateAim(Vector2 source, float speed)
        {
            Player player = Main.player[Projectile.owner];
            var aim = Vector2.Normalize(Main.MouseWorld - source);
            if (aim.HasNaNs())
                aim = -Vector2.UnitY;
            Vector2 DirAndVel = new(Projectile.velocity.X * player.direction, Projectile.velocity.Y * player.direction);
            Projectile.rotation = DirAndVel.ToRotation();
            aim = Vector2.Normalize(Vector2.Lerp(Vector2.Normalize(Projectile.velocity), aim, 0.7f));
            aim *= speed;

            if (aim != Projectile.velocity)
                Projectile.netUpdate = true;
            Projectile.velocity = aim;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            var texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            var sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            Color drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, -20f),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            return false;
        }

    }
}
