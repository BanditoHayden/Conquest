using Conquest.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Conquest.Items.Weapons.Magic
{
    public class Duality : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            // Tooltip.SetDefault("Projectiles fired from this weapon always crit");
        }
        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 30;
            Item.height = 30;
            Item.value = Item.sellPrice(gold: 1);
            Item.noMelee = true;
            Item.rare = ItemRarityID.White;
            // Use Properties
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.RaiseLamp;
            Item.noUseGraphic = false;
            Item.UseSound = SoundID.Item43;
            Item.autoReuse = true;
            // Weapon Properties
            Item.damage = 54;
            Item.knockBack = 3f;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 15;
            // Projectile Properties
            Item.shoot = ModContent.ProjectileType<SineWave>();
            Item.shootSpeed = 10f;
        }







        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Color[] colors = new Color[] { Color.Black, Color.FloralWhite };
            int projectileCount = 2;

            for (int i = 0; i < projectileCount; i++)
            {
                // Be wary of dividing by zero when projectileCount is 1
                float waveOffset = i / (float)(projectileCount - 1);

                Projectile projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, 0, waveOffset * (1f - 1f / projectileCount), i);
            }

            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
           .AddIngredient(ItemID.LightShard, 1)
            .AddIngredient(ItemID.DarkShard, 1)
           .AddTile(TileID.MythrilAnvil)
           .Register();
        }

    }
}
