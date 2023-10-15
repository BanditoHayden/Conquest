using Conquest.Assets.Common;
using Conquest.Projectiles;
using Conquest.Projectiles.Melee;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Conquest.Items.Weapons.Melee
{
    public class AeroScimitar : ModItem
    {


        public override void SetDefaults()
        {

            // Common Properties
            Item.width = 60;
            Item.height = 60;
            // Use Properties
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.UseSound = SoundID.Item1;
            Item.reuseDelay = 2;

            Item.autoReuse = false;
            Item.rare = 2;
            Item.value = Item.buyPrice(gold: 5);
            // Weapon Properties
            Item.damage = 14;
            Item.knockBack = 2f;
            Item.DamageType = DamageClass.Melee;
            Item.shoot = ModContent.ProjectileType<Tornado>();
            Item.shootSpeed = 8f;
            // Projectile Properties
        }
    }
}