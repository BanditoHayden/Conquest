using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent;
using Terraria.DataStructures;
using Terraria.Audio;
using System.Collections.Generic;
using Conquest.Projectiles.Ranged;

namespace Conquest.Items.Weapons.Ranged;

public class GammaRay : ModItem
{
    public override void SetStaticDefaults()
    {
        /*Tooltip.SetDefault("Shoots an energized ray of gel\n" +
            "Charges fire rate while striking enemies\n" +
            "After enough charge, right click to fire a burst");*/
    }

    public override void SetDefaults()
    {
        //Item.rare = ModContent.RarityType<ShadowRarity>();

        Item.width = 46;
        Item.height = 14;
        Item.scale = 1.5f;

        Item.damage = 156;
        Item.DamageType = DamageClass.Ranged;
        Item.crit = 10;
        Item.knockBack = 0.01f;
        Item.useAmmo = AmmoID.Gel;

        Item.useTime = 30;
        Item.useAnimation = 30;

        Item.noMelee = true;
        Item.autoReuse = true;

        Item.useStyle = ItemUseStyleID.Shoot;
        Item.shoot = ModContent.ProjectileType<GammaRayProjectile>();
        Item.shootSpeed = 120;

        Item.UseSound = SoundID.DD2_ExplosiveTrapExplode;

        Item.value = Item.sellPrice(platinum: 0, gold: 25);
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        float percent = (-10f * Item.useTime) + 300f;
        if (Item.useTime == 1) percent = 2f;
        percent *= 10000;
        int whole = (int)percent;
        percent = whole / 10000;
        string text = (Item.useTime <= 20 ? "[c/2bd8a2:Burst Charged! ]" : "Burst Charge: ") + percent + "%";
        TooltipLine item = new TooltipLine(Mod, "Tooltip3", text);
        tooltips.Add(item);
    }

    public override bool AltFunctionUse(Player player)
    {
        return Item.useAnimation <= 20;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        type = ModContent.ProjectileType<GammaRayProjectile>();
        if (player.altFunctionUse == 2)
        {
            damage = (int)(damage * (7 - (0.2f * Item.useTime)));
            Item.useTime = 30;
            Item.useAnimation = 30;
        }
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        SoundEngine.PlaySound(player.altFunctionUse == 2 ? SoundID.Item96 : SoundID.Item157, player.position);
        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * Item.width * Item.scale; //This gets the direction of the flame projectile, makes its length to 1 by normalizing it. It then multiplies it by 54 (the item width) to get the position of the tip of the flamethrower.
        muzzleOffset.Y -= 3 * Item.scale;

        return true;
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-11, -2);
    }
}

