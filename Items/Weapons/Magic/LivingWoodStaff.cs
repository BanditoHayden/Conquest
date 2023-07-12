﻿
using Conquest.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Conquest.Items.Weapons.Magic
{
    public class LivingWoodStaff : ModItem
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
            Item.width = 30;
            Item.height = 30;
            Item.value = Item.sellPrice(silver: 1);
            Item.noMelee = true;
            Item.rare = 1;
            // Use Properties
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = false;
            Item.UseSound = SoundID.Item43;
            Item.autoReuse = true;
            // Weapon Properties
            Item.damage = 13;
            Item.knockBack = 3f;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 6;
            // Projectile Properties
            Item.shoot = ModContent.ProjectileType<Staff2>();
            Item.shootSpeed = 0f;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectileDirect(source, Main.MouseWorld, velocity, type, damage, knockback, player.whoAmI);
            return false;

        }
        public override void AddRecipes()
        {
            CreateRecipe()
           .AddIngredient(ItemID.Wood, 12)
           .AddTile(TileID.LivingLoom)
           .Register();

        }
    }
}
