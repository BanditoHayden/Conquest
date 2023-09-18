using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Conquest.Projectiles.Summoner;

namespace Conquest.Items.Weapons.Summon;

public class EchoingStarlash : ModItem
{
	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
	}

	public override void SetDefaults()
	{
		// This method quickly sets the whip's properties.
		// Mouse over to see its parameters.
		Item.DefaultToWhip(ModContent.ProjectileType<EchoingStarlashWhip>(), 144, 4f, 5);
		Item.shootSpeed = 5;
		Item.rare = ItemRarityID.Purple;
		Item.width = 19;
		Item.height = 18;
		Item.useTime = 50;
		Item.autoReuse = true;
		Item.DamageType = DamageClass.SummonMeleeSpeed;
		Item.damage = 144;
		Item.channel = true;
		Item.scale = 2;
	}
}