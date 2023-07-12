﻿
using Conquest.Buffs;
using Conquest.Items.Materials;
using Conquest.Projectiles.Summoner;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Conquest.Items.Weapons.Summon
{
    internal class JellyUmbrella : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Jelly Umbrella");
            // Tooltip.SetDefault("Summons a spirit jellyfish to fight for you\nThe spirit jellyfish has a chance to inflict electrified");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.knockBack = 3f;
            Item.mana = 10;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.sellPrice(gold: 3);
            Item.rare = 4;
            Item.UseSound = SoundID.Item44;

            Item.noMelee = true;
            Item.DamageType = DamageClass.Summon;
            Item.buffType = ModContent.BuffType<JellyfishBuff>();
            Item.shoot = ModContent.ProjectileType<Jellyfish>();
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);

            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
            projectile.originalDamage = Item.damage;

            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Umbrella, 1)
          .AddIngredient(ModContent.ItemType<JellyfishTentacle>(), 3)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }

    }
}

