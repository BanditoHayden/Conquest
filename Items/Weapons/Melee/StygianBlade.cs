using Conquest.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Conquest.Items.Materials;
using Terraria.DataStructures;
using Conquest.Buffs;

namespace Conquest.Items.Weapons.Melee
{
    public class StygianBlade : ModItem
    {
        public int attackType = 0; // keeps track of which attack it is
        public int comboExpireTimer = 0; // we want the attack pattern to reset if the weapon is not used for certain period of time
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 80;
            Item.height = 80;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 1);
            Item.noMelee = false;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noUseGraphic = false;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.damage = 125;
            Item.knockBack = 2f;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<StygianBladeProj>();
            Item.shootSpeed = 1;
            Item.reuseDelay = 6;
        }
        int Counter;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float adjustedItemScale = player.GetAdjustedItemScale(Item); // Get the melee scale of the player and item.
            Counter++;
            if (Counter == 1)
            {
                attackType = 0;
            }
            if (Counter == 2)
            {
                attackType = 1;
                //Counter = 0;
            }
            if (Counter == 3)
            {
                attackType = 2;
                Counter = 0;
            }
            comboExpireTimer = 0;

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Main.myPlayer, attackType);

            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
           .AddIngredient(ModContent.ItemType<AbhorrentMeat>(), 2)
           .AddIngredient(ItemID.HallowedBar, 9)
           .AddTile(TileID.Anvils)
           .Register();
        }
    }
}
