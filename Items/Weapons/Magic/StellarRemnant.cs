using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Conquest.Projectiles.Magic;

namespace Conquest.Items.Weapons.Magic;

public class StellarRemnant : ModItem
{
    public override void SetStaticDefaults()
    {
        // DisplayName.SetDefault("Stellar Remnant");
        // Tooltip.SetDefault("Creates waves of dark energy" +
        //     "\nDrains the user's life" +
        //     "\nCannot be used under mana sickness");
    }

    public override void SetDefaults()
    {
        Item.width = 14;
        Item.height = 13;
        Item.damage = 122;
        Item.mana = 9;
        Item.DamageType = DamageClass.Magic;
        Item.knockBack = 4;
        Item.noUseGraphic = true;
        Item.noMelee = true;
        Item.useTime = 1;
        Item.useAnimation = 5;
        Item.autoReuse = true;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.shoot = ModContent.ProjectileType<StellarRemnantProjectile>();
        Item.shootSpeed = 7;
        Item.UseSound = SoundID.Item20;
        Item.rare = ItemRarityID.Purple;

        Item.value = Item.sellPrice(platinum: 0, gold: 25);
    }

    public override bool CanUseItem(Player player)
    {
        return !player.HasBuff(BuffID.ManaSickness);
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        player.statLife--;
        if (player.statLife <= 1) player.statLife = 1;
        velocity = velocity.RotatedBy(Main.rand.NextFloat(MathF.PI / -4, MathF.PI / 4));
    }
}

