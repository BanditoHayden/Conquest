using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Conquest.Projectiles.Magic.Energy;
using Conquest.Buffs;
using Mono.Cecil;

namespace Conquest.Items.Weapons.Magic.Energy;

public class Omniwrench : EnergyWeapon
{
    SoundStyle RetroBlast = new SoundStyle($"{nameof(Conquest)}/Assets/Sounds/Retro_Blast")
    {
        Volume = 0.9f,
        PitchVariance = 0.2f,
        MaxInstances = 3,
    };
        

	public override void SetDefaults()
    {
		Item.width = 31; 
        Item.height = 31;
        Item.staff[Type] = true;
        Item.value = 1000;
        Item.noMelee = true;
        Item.rare = ItemRarityID.Pink;
        Item.mana = 27;
        Item.useTime = 1;
        Item.useAnimation = 30;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.noUseGraphic = false;
        Item.damage = 100;
        Item.knockBack = 6f;
        Item.DamageType = DamageClass.Magic;
		Item.shoot = ModContent.ProjectileType<LightningBolt>();
		Item.shootSpeed = 8;
        Item.ArmorPenetration = 999;
        Item.autoReuse = true;
        Item.noMelee = true;
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-8, 0);
    }

    int shotsFired = 0;

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (shotsFired == 0) SoundEngine.PlaySound(SoundID.Item43, position);
        if (shotsFired % 3 == 0 && shotsFired < 20)
        {
            Vector2 oldPos = position;
            type = ModContent.ProjectileType<LightningSpawnBolt>();
            float ringWidth = Main.rand.NextFloat(50f, 70f);
            position += Main.rand.NextVector2CircularEdge(ringWidth, ringWidth);
        }
        else if (shotsFired % 2 == 0) type = ProjectileID.None;
        else
        {
            float energy = player.GetModPlayer<EnergyPlayer>().energyPower;
            for (int i = 0; i < (int)energy - 1; i++)
            {
                Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), position, velocity, type, damage, knockback, player.whoAmI);
            }
        }
        shotsFired++;
        shotsFired = shotsFired % Item.useAnimation;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.RainbowRod)
            .AddIngredient(ItemID.MartianConduitPlating, 40)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}