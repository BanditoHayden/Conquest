using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Conquest.Items.Accessory;

public class EnergyDiagnostic : ModItem
{

    public override void SetDefaults()
    {
        Item.width = 16;
        Item.height = 19;

        Item.value = Item.sellPrice(0, 1, 0, 0);
        Item.rare = ItemRarityID.Blue;

        Item.accessory = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Wire, 2)
            .AddRecipeGroup("IronBar", 1)
            .AddIngredient(ItemID.SoulofFright)
            .Register();
        CreateRecipe()
            .AddIngredient(ItemID.Wire, 2)
            .AddRecipeGroup("IronBar", 1)
            .AddIngredient(ItemID.SoulofSight)
            .Register();
        CreateRecipe()
            .AddIngredient(ItemID.Wire, 2)
            .AddRecipeGroup("IronBar", 1)
            .AddIngredient(ItemID.SoulofMight)
            .Register();
    }

    public override void UpdateInventory(Player player)
    {
        player.GetModPlayer<InformationPlayer>().accEnergyTracker = true;
    }

    public override void UpdateEquip(Player player)
    {
        player.GetModPlayer<InformationPlayer>().accEnergyTracker = true;
    }
}

public class InformationPlayer : ModPlayer
{
    public bool accEnergyTracker = false;

    public override void ResetEffects()
    {
        accEnergyTracker = false;
    }
}

