using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace Conquest.Items.Weapons.Magic.BunnyBlaster
{
	public class BunnyBlaster	: ModItem
	{
		SoundStyle ByeBye = new SoundStyle($"{nameof(Conquest)}/Items/Weapons/Magic/BunnyBlaster/Bye_Bye")
        {
            Volume = 0.9f,
            PitchVariance = 0.2f,
            MaxInstances = 1,
        };
		public override void SetDefaults()
        {
			Item.width = 26; 
            Item.height = 28;
            Item.value = 100000;
            Item.noMelee = true;
            Item.rare = 6;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = false;
            Item.damage = 15;
            Item.mana = 4;
            Item.knockBack = 3f;
            Item.DamageType = DamageClass.Magic;
            Item.shootSpeed = 15.1f;
			Item.shoot = ModContent.ProjectileType<BunnyBomb>();
			Item.shootSpeed = 16;
			Item.autoReuse = true;
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			SoundEngine.PlaySound(ByeBye, player.position);
            return true;

        }
        public override void AddRecipes()
        {
            CreateRecipe()
           .AddIngredient(ItemID.Bunny, 1)
            .AddIngredient(ItemID.TopHat, 1)
           .AddTile(TileID.DemonAltar)
           .Register();
        }

    }
    public class BunnyBomb : ModProjectile
	{
		public override void SetDefaults() {
			Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 1;
			Projectile.aiStyle = 1;
			Projectile.timeLeft = 180;
		}
		public override void OnSpawn (IEntitySource source)
		{
			Projectile.spriteDirection = Main.player[Projectile.owner].direction;
		}
		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.NPCDeath1,Projectile.Center);
			Gore.NewGore(null, Projectile.Center, Projectile.velocity, 76);
			for (int i = 0; i < Main.rand.Next(6); i++)
			{
				Gore.NewGore(null, Projectile.Center, Projectile.velocity.RotatedByRandom(1f), 77);
			}
		}
	}
}