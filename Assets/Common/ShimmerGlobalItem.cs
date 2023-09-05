using Conquest.Items.Weapons.Melee;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Conquest.Assets.Systems
{
    public class ShimmerRecipes : ModSystem
    {
        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(ItemID.Gladius)
                  .AddCustomShimmerResult(ModContent.ItemType<EmperorSword>())
                  .AddIngredient(ItemID.Gladius)
                  .AddCondition(Condition.DownedEowOrBoc)
                  .AddCondition(Language.GetOrRegister("Shimmered"), () => NPC.downedBoss2)
                  .Register();
            Recipe recipe2 = Recipe.Create(ItemID.Muramasa)
                  .AddIngredient(ItemID.Muramasa)
                 .AddCustomShimmerResult(ModContent.ItemType<Chikage>())
                 .AddCondition(Language.GetOrRegister("Shimmered"), () => NPC.downedBoss3)
                 .AddCondition(Condition.DownedSkeletron)
                 .Register();
            Recipe recipe3 = Recipe.Create(ItemID.TheRottedFork)
                .AddIngredient(ItemID.TheRottedFork)
               .AddCustomShimmerResult(ModContent.ItemType<FreshFork>())
               .AddCondition(Language.GetOrRegister("Shimmered"), () => NPC.downedBoss2)
               .AddCondition(Condition.DownedEowOrBoc)
               .Register();
        }
    }
    public class GladiusGlobal : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.Gladius;

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.Add(new(Mod, "", "Can be shimmered after killing any evil boss"));
        }
    }
    public class MuramasaGlobal : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.Muramasa;

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.Add(new(Mod, "", "Can be shimmered after killing Skeletron"));
        }
    }
    public class RottedForkGlobal : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.TheRottedFork;

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.Add(new(Mod, "", "Can be shimmered after killing any evil boss"));
        }
    }


}
