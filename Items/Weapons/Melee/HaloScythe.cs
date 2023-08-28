using System;
using Conquest.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Conquest.Items.Weapons.Melee
{
	public class HaloScythe : ModItem
	{
        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 62;
            Item.value = Item.sellPrice(gold: 2);
            Item.rare = ItemRarityID.Green;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.autoReuse = true;
            Item.damage = 62;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<HaloScytheProj>();
            Item.reuseDelay = 3;
        }
    }
}

