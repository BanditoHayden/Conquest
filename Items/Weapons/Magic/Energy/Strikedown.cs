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

public class Strikedown : EnergyWeapon
{
    SoundStyle RetroBlast = new SoundStyle($"{nameof(Conquest)}/Assets/Sounds/Retro_Blast")
    {
        Volume = 0.9f,
        PitchVariance = 0.2f,
        MaxInstances = 3,
    };

	public override void SetDefaults()
    {
		Item.width = 26; 
        Item.height = 13;
        Item.value = 1000;
        Item.noMelee = true;
        Item.rare = ItemRarityID.Pink;
        Item.mana = 20;
        Item.useTime = 1;
        Item.useAnimation = 35;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.noUseGraphic = false;
        Item.damage = 50;
        Item.knockBack = 6.5f;
        Item.DamageType = DamageClass.Magic;
		Item.shoot = ModContent.ProjectileType<StrikedownBolt>();
		Item.shootSpeed = 8;
		Item.autoReuse = true;
        Item.ArmorPenetration = 999;
        knockbackScale = 2f;
        Item.noMelee = true;
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2, -1);
    }

    int shotsFired = 0;
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (shotsFired == 0) SoundEngine.PlaySound(SoundID.Item43, position);
        if (shotsFired % 3 == 0 && shotsFired < 15)
        {
            Vector2 oldPos = position;
            type = ModContent.ProjectileType<StrikedownSpawnBolt>();
            float ringWidth = Main.rand.NextFloat(200f, 220f);
            position += Main.rand.NextVector2CircularEdge(ringWidth, ringWidth);
        }
        else if (shotsFired < Item.useAnimation - 1)
        {
            type = ProjectileID.None;
        }
        else
        {
            SoundEngine.PlaySound(RetroBlast, position);
            float energy = player.GetModPlayer<EnergyPlayer>().energyPower;
            for (int i = 0; i < 5 + (int)energy; i++)
            {
                Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item),
                    position, velocity.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-15f, 15f))),
                    type, damage, knockback, player.whoAmI);
            }
        }
        shotsFired++;
        shotsFired = shotsFired % Item.useAnimation;
    }
}

public class StrikedownSpawning : GlobalNPC
{
    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        if (npc.type == NPCID.Retinazer || npc.type == NPCID.Spazmatism)
        {
            npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<Strikedown>(), 5, 3));
        }
    }
}