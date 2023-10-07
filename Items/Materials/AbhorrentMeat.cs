using Terraria.Net;
using Terraria.GameContent.NetModules;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Conquest.Assets.Common;
using System.Collections.Generic;

namespace Conquest.Items.Materials
{
    public class AbhorrentMeat : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.shoot = 10;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.width = 18;
            Item.height = 22;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(copper: 33); // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on platinum/gold/silver/copper arguments provided to it.
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            ArenaSpawnInfo info = new ArenaSpawnInfo(new List<int[]>(), 0);// <- initialize new spawn info
            info.Enemies.Add(new int[]{ NPCID.Zombie, NPCID.Zombie, NPCID.Zombie, NPCID.Zombie});// <- add wave 1
            info.Enemies.Add(new int[] { NPCID.Skeleton, NPCID.Skeleton, NPCID.Skeleton, NPCID.Skeleton });// <- add wave 2
            ModContent.GetInstance<ArenaSystem>().ActivateArena(player.Center, info, new Vector2[]{ player.Center, player.Center + Vector2.UnitX * 150, player.Center - Vector2.UnitX * 150 }, 50 * 16, 50 * 16);
            return false;
        }
    }
}
