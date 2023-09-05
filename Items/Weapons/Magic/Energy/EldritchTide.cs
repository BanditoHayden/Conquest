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

public class EldritchTide : EnergyWeapon
{
	public override void SetDefaults()
    {
		Item.width = 31; 
        Item.height = 31;
        Item.staff[Type] = true;
        Item.value = 1000;
        Item.noMelee = true;
        Item.rare = ItemRarityID.Pink;
        Item.mana = 40;
        Item.useTime = 1;
        Item.useAnimation = 75;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.noUseGraphic = false;
        Item.damage = 143;
        Item.knockBack = 6f;
        Item.DamageType = DamageClass.Magic;
		Item.shoot = ModContent.ProjectileType<EldritchStarter>();
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
        if (shotsFired % 10 == 0) SoundEngine.PlaySound(SoundID.Item43, position);
        if (shotsFired % 3 == 0 && shotsFired < 20)
        {
            Vector2 oldPos = position;
            type = ModContent.ProjectileType<EldritchSpawnBolt>();
            float ringWidth = Main.rand.NextFloat(150f, 200f);
            position += Main.rand.NextVector2CircularEdge(ringWidth, ringWidth);
        }
        else if (shotsFired < Item.useAnimation - 1)
        {
            type = ProjectileID.None;
        }
        else
        {
            SoundEngine.PlaySound(SoundID.Item21, position);
            float energy = player.GetModPlayer<EnergyPlayer>().energyPower;
        }
                shotsFired++;
        shotsFired = shotsFired % Item.useAnimation;
    }
}