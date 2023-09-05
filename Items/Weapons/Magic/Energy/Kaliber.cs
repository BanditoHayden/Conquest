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

public class Kaliber : EnergyWeapon
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
        Item.mana = 12;
        Item.useTime = 1;
        Item.useAnimation = 20;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.noUseGraphic = false;
        Item.damage = 78;
        Item.knockBack = 1f;
        Item.DamageType = DamageClass.Magic;
		Item.shoot = ModContent.ProjectileType<KaliberBolt>();
		Item.shootSpeed = 8;
		Item.autoReuse = true;
        Item.noMelee = true;
        Item.ArmorPenetration = 999;
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-4, 0);
    }

    int shotsFired = 0;
    public override void UpdateInventory(Player player)
    {
        float energy = player.GetModPlayer<EnergyPlayer>().energyPower;
        Item.useTime = 1;
        Item.useAnimation = (int)MathF.Round(21 - (3 * energy));
        if (Item.useAnimation < 10) Item.useAnimation = 12;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (shotsFired == 0) SoundEngine.PlaySound(SoundID.Item43, position);
        if (shotsFired % 3 == 0 && shotsFired < 10)
        {
            Vector2 oldPos = position;
            type = ModContent.ProjectileType<KaliberSpawnBolt>();
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
        }
        shotsFired++;
        shotsFired = shotsFired % Item.useAnimation;
    }
}

public class KaliberSpawning : GlobalNPC
{
    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        if (npc.type == NPCID.TheDestroyer)
        {
            npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<Kaliber>(), 5, 3));
        }
    }
}