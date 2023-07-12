﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Conquest.Projectiles.Melee;

namespace Conquest.Items.Weapons.Melee
{
    public class AnubisHammer : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 30;
            Item.height = 30;
            Item.value = Item.buyPrice(gold: 1);
            Item.noMelee = true;
            // Use Properties
            Item.rare = 6;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = false;
            Item.noUseGraphic = true;
            // Weapon Properties
            Item.damage = 90;
            Item.knockBack = 3f;
            Item.DamageType = DamageClass.Melee;
            // Projectile Properties
            Item.shootSpeed = 15f;
            Item.shoot = ModContent.ProjectileType<AnubisHammerProj>();
        }


        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 4;
        }
    }
}
