using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;


namespace Conquest.Items.Weapons.Melee
{
    public class LivingWoodSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Critical hits heal you");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 46;
            Item.height = 44;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(copper: 33);
            // Use Properties
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = false;
            // Weapon Properties
            Item.damage = 12;
            Item.knockBack = 3f;
            Item.DamageType = DamageClass.Melee;
            // Projectile Properties

        }
        
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Crit)
            {
                player.Heal(5);
            }
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
