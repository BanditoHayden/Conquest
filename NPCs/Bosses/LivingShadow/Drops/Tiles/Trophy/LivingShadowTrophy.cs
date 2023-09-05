using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Conquest.NPCs.Bosses.LivingShadow.Drops.Tiles.Trophy;

public class LivingShadowTrophy : ModItem
{
    public override void SetStaticDefaults()
    {
        //DisplayName.SetDefault("Living Shadow Trophy");

        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
    }

    public override void SetDefaults()
    {
        // Vanilla has many useful methods like these, use them! This substitutes setting Item.createTile and Item.placeStyle aswell as setting a few values that are common across all placeable items
        Item.DefaultToPlaceableTile(ModContent.TileType<LivingShadowTrophyTile>());

        Item.width = 24;
        Item.height = 23;
        Item.maxStack = 99;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.buyPrice(0, 1);
    }
}