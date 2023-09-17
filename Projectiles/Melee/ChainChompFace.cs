using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using static Terraria.ModLoader.ModTexturedType;

namespace Conquest.Projectiles.Melee
{
	public class ChainChompFace : ModProjectile
	{
		
		SoundStyle ChompYelp = new SoundStyle($"{nameof(Conquest)}/Assets/Sounds/Chain_Chomp_Yelp")
        {
            Volume = 0.9f,
            PitchVariance = 0.1f,
            MaxInstances = 2,
        };

		public override void SetDefaults()
		{
			Projectile.DamageType = DamageClass.Melee;
			Projectile.netImportant = true;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
			Projectile.aiStyle = 15;
		}
		bool hit = false, thrown = false;

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!hit && thrown) SoundEngine.PlaySound(ChompYelp, Projectile.position);
			hit = true;
            return base.OnTileCollide(oldVelocity);
        }

        public override void AI()
		{
			var player = Main.player[Projectile.owner];

			if (!player.channel && !thrown)
            {
				thrown = true;
				SoundEngine.PlaySound(SoundID.Item1);
            }

			// If owner player dies, remove the flail.
			if (player.dead) {
				Projectile.Kill();
				return;
			}
			else
			{
				base.AI();
			}
		}
		public override bool PreDrawExtras()
		{
			return false;
		}
		
		public override bool PreDraw(ref Color lightColor)
		{
			var player = Main.player[Projectile.owner];
			Projectile.spriteDirection = player.direction;
			Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
			Texture2D chainTexture = ModContent.Request<Texture2D>("Conquest/Projectiles/Melee/ChainChompChain").Value;
			Projectile.rotation = 3.14159f-(float)Math.Atan2((double)(Projectile.position.X-player.position.X), (double)(Projectile.position.Y-player.position.Y));
			Vector2 mountedCenter = player.MountedCenter;
			var drawPosition = Projectile.Center;
			Vector2 remainingVectorToPlayer = mountedCenter - drawPosition;
			float rotation = remainingVectorToPlayer.ToRotation() - MathHelper.PiOver2;
			//Main.EntitySpriteDraw(Texture, drawPosition - Main.screenPosition, null, lightColor, rotation, projectileTexture.Size() * 0.5f, 1f, (Projectile.spriteDirection > 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
			

			if (Projectile.alpha == 0) {
				int direction = -1;

				if (Projectile.Center.X < mountedCenter.X)
					direction = 1;

				player.itemRotation = (float)Math.Atan2(remainingVectorToPlayer.Y * direction, remainingVectorToPlayer.X * direction);
			}

			// This while loop draws the chain texture from the projectile to the player, looping to draw the chain texture along the path
			while (true) {
				float length = remainingVectorToPlayer.Length();

				// Once the remaining length is small enough, we terminate the loop
				if (length < 17f || float.IsNaN(length))
					break;

				// drawPosition is advanced along the vector back to the player by 12 pixels
				// 12 comes from the height of ExampleFlailProjectileChain.png and the spacing that we desired between links
				drawPosition += remainingVectorToPlayer * 16 / length;
				remainingVectorToPlayer = mountedCenter - drawPosition;

				// Finally, we draw the texture at the coordinates using the lighting information of the tile coordinates of the chain section
				Color color = Lighting.GetColor((int)drawPosition.X / 16, (int)(drawPosition.Y / 16f));
				Main.EntitySpriteDraw(chainTexture, drawPosition - Main.screenPosition, null, color, rotation, chainTexture.Size() * 0.5f, 1f, (Projectile.spriteDirection > 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
			}

			
			
			
			return true;
		}
		
	}
}