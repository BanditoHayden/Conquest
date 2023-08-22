using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
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
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.noUseGraphic = false;
        Item.damage = 26;
        Item.knockBack = 1f;
        Item.DamageType = DamageClass.Magic;
		Item.shoot = ModContent.ProjectileType<KaliberBolt>();
		Item.shootSpeed = 8;
		Item.autoReuse = true;
        Item.UseSound = RetroBlast;
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
