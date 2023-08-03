
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Conquest.Projectiles.Melee;

namespace Conquest.Items.Weapons.Melee
{
    public class OreClub : ModItem
    {
        public override void SetStaticDefaults()
        {

            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            // Common Properties

            Item.width = 68;
            Item.height = 68;
            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 120;
            Item.useTime = 120;
            Item.noUseGraphic = true;
            //Item.UseSound = SoundID.Item1;
            Item.noMelee = true;
            Item.autoReuse = false;
            Item.rare = ItemRarityID.Green;

            //   Item.channel = true;
            Item.value = Item.sellPrice(gold: 1);
            Item.noMelee = true;
            // Weapon Properties
            Item.damage = 34;
            Item.knockBack = 2f;
            Item.DamageType = DamageClass.Melee;
            // Projectile Properties
            Item.shoot = ModContent.ProjectileType<OC>();
            Item.shootSpeed = 5f;

        }
       
       
        public int attackType = 0;
        public int comboExpireTimer = 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Using the shoot function, we override the swing projectile to set ai[0] (which attack it is)
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Main.myPlayer, attackType);
           
            attackType = 0;

            comboExpireTimer = 0; // Every time the weapon is used, we reset this so the combo does not expire
            return false; // return false to prevent original projectile from being shot
        }
    } }
