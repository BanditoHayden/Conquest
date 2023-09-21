using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Conquest.Projectiles.Magic;
using Conquest.Assets.Common;

namespace Conquest.Items.Weapons.Magic
{
    public class Staff3 : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            // Tooltip.SetDefault("Projectiles fire at your cursor, Critical hits heal you");
            Item.staff[Item.type] = true;
        }
        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 48;
            Item.height = 48;
            Item.value = Item.sellPrice(silver: 1);
            Item.noMelee = true;
            Item.rare = 6;
            // Use Properties
            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = false;
            Item.UseSound = SoundID.Item43;
            Item.autoReuse = true;
            // Weapon Properties
            Item.damage = 52;
            Item.knockBack = 3f;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 9;
            // Projectile Properties
            Item.shoot = ModContent.ProjectileType<Staff3Projectile>();
            Item.shootSpeed = 12f;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int num = 3 + Main.rand.Next(0, 3);

            for (float i = 0; i < num; i++)
            {
                position = new Vector2(Main.MouseWorld.X + Main.rand.Next(-400, 400), Main.screenPosition.Y + Main.screenHeight * 9/10);
                velocity = position.DirectionTo(Main.MouseWorld + Vector2.UnitX * Main.rand.Next(-100, 100)) * Item.shootSpeed;
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            }

            return false;

        }
    }
    public class Staff3Projectile : ModProjectile
    {
        private ref float timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50;    //The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.ignoreWater = true;
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.Opacity = 0;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            base.SetDefaults();
        }
        float gravity = 0.2f;
        public override void AI()
        {
            timer++;
            //Dust a = Dust.NewDustPerfect(Projectile.Center, DustID.Lava, Vector2.Zero, (int)(255 - timer * 17));
            //a.noGravity = true;
            Lighting.AddLight(Projectile.Center, Color.OrangeRed.ToVector3());
            base.AI();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.OrangeRed;
            default(Effects.FireTrail).Draw(Projectile);
            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 70);
            base.OnHitNPC(target, hit, damageDone);
        }
    }
}
