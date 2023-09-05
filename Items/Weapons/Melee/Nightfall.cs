using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Conquest.Projectiles.Melee;

namespace Conquest.Items.Weapons.Melee;

public class Nightfall : ModItem
{

    public override void SetStaticDefaults()
    {
        //Tooltip.SetDefault("Benefits more from attack speed bonuses");
        ItemID.Sets.SkipsInitialUseSound[Item.type] = true; // This skips use animation-tied sound playback, so that we're able to make it be tied to use time instead in the UseItem() hook.
        ItemID.Sets.Spears[Item.type] = true; // This allows the game to recognize our new item as a spear.
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
    }
    public override void SetDefaults()
    {
        // Common Properties
        Item.rare = ItemRarityID.Purple;
        Item.value = Item.sellPrice(platinum: 0, gold: 25);

        // Use Properties
        Item.useStyle = ItemUseStyleID.Shoot; // How you use the item (swinging, holding out, etc.)
        Item.useAnimation = 20; // The length of the item's use animation in ticks (60 ticks == 1 second.)
        Item.useTime = 20; // The length of the item's use time in ticks (60 ticks == 1 second.)
        Item.UseSound = SoundID.Item71; // The sound that this item plays when used.
        Item.autoReuse = true; // Allows the player to hold click to automatically use the item again. Most spears don't autoReuse, but it's possible when used in conjunction with CanUseItem()

        // Weapon Properties
        Item.damage = 240;
        Item.knockBack = 6.5f;
        Item.noUseGraphic = true; // When true, the item's sprite will not be visible while the item is in use. This is true because the spear projectile is what's shown so we do not want to show the spear sprite as well.
        Item.DamageType = DamageClass.Melee;
        Item.noMelee = true; // Allows the item's animation to do damage. This is important because the spear is actually a projectile instead of an item. This prevents the melee hitbox of this item.

        Item.width = 24;
        Item.height = 24;

        // Projectile Properties
        Item.shootSpeed = 3.7f; // The speed of the projectile measured in pixels per frame.
        Item.shoot = ModContent.ProjectileType<NightfallProjectile>(); // The projectile that is fired from this weapon
    }

    public override float UseTimeMultiplier(Player player)
    {
        return 0.5f;
    }

    public override bool CanUseItem(Player player)
    {
        // Ensures no more than one spear can be thrown out, use this when using autoReuse
        return player.ownedProjectileCounts[Item.shoot] < 1;
    }

    public override bool? UseItem(Player player)
    {
        // Because we're skipping sound playback on use animation start, we have to play it ourselves whenever the item is actually used.
        if (!Main.dedServ && Item.UseSound.HasValue)
        {
            SoundEngine.PlaySound(Item.UseSound.Value, player.Center);
        }

        return null;
    }
}