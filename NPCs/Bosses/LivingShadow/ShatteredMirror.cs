using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Conquest.NPCs.Bosses.LivingShadow;

public class ShatteredMirror : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 1;
        ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
    }

    public override void SetDefaults()
    {
        Item.width = 16;
        Item.height = 16;
        Item.maxStack = 1;
        Item.value = 100;
        Item.rare = ItemRarityID.Purple;
        Item.useAnimation = 30;
        Item.useTime = 30;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.consumable = false;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        if (Main.dayTime) tooltips.Insert(4, new TooltipLine(Mod, "Tooltip0", "Broken by extraterrestrial powers, this mirror now awakens a powerful being"));
        else tooltips.Insert(4, new TooltipLine(Mod, "Tooltip0", "Are you ready for what will be released?"));
    }

    public override bool CanUseItem(Player player)
    {
        return !NPC.AnyNPCs(ModContent.NPCType<LivingShadow>());
    }

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI == Main.myPlayer)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath51, player.position);

            int type = ModContent.NPCType<LivingShadow>();

            if (Main.netMode != NetmodeID.MultiplayerClient) NPC.SpawnOnPlayer(player.whoAmI, type);
            else NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
        }

        return true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient<Items.Consumable.EightTrigramsMirror>()
            .AddIngredient(ItemID.FragmentNebula, 2)
            .AddIngredient(ItemID.FragmentSolar, 2)
            .AddIngredient(ItemID.FragmentStardust, 2)
            .AddIngredient(ItemID.FragmentVortex, 2)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
    }
}