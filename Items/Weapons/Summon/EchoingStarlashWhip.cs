using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Conquest.Buffs;

namespace Conquest.Projectiles.Summoner;

public class EchoingStarlashWhip : ModProjectile
{
	public override void SetStaticDefaults()
	{
		// This makes the projectile use whip collision detection and allows flasks to be applied to it.
		ProjectileID.Sets.IsAWhip[Type] = true;
	}

	public override void SetDefaults()
	{
		// This method quickly sets the whip's properties.
		Projectile.width = 7;
		Projectile.height = 39;
		Projectile.DefaultToWhip();
		Projectile.WhipSettings.Segments = 11;
		Projectile.WhipSettings.RangeMultiplier = 2.5f;
	}

	private float Timer
	{
		get => Projectile.ai[0];
		set => Projectile.ai[0] = value;
	}

	private float ChargeTime
	{
		get => Projectile.ai[1];
		set => Projectile.ai[1] = value;
	}

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damage)
    {
        Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
		Main.player[Projectile.owner].AddBuff(ModContent.BuffType<EchoingStarlashBuff>(), 180);
    }

    // This method draws a line between all points of the whip, in case there's empty space between the sprites.
    private void DrawLine(List<Vector2> list)
	{
		Texture2D texture = TextureAssets.FishingLine.Value;
		Rectangle frame = texture.Frame();
		Vector2 origin = new Vector2(frame.Width / 2, 2);

		Vector2 pos = list[0];
		for (int i = 0; i < list.Count - 1; i++)
		{
			Vector2 element = list[i];
			Vector2 diff = list[i + 1] - element;

			float rotation = diff.ToRotation() - MathHelper.PiOver2;
			Color color = Lighting.GetColor(element.ToTileCoordinates(), new Color(10, 20, 40));
			//color.A = 128;
			Vector2 scale = new Vector2(1, (diff.Length() + 2) / frame.Height);

			Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);
				
			pos += diff;
		}
	}

	public override void AI()
	{
		Vector2 point = Projectile.WhipPointsForCollision[Projectile.WhipPointsForCollision.Count - 1];
        Lighting.AddLight(point, /*new Vector3(0.007f, 0.780f, 1f)*/TorchID.UltraBright);

        Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
        if (MathF.Abs((Timer / timeToFlyOut) - 0.5f) < float.Epsilon) // apex reached, shoot projectile
        {
			Vector2 endPoint = Projectile.WhipPointsForCollision[Projectile.WhipPointsForCollision.Count - 1];
        }

    }

	public override bool PreDraw(ref Color lightColor)
	{
		List<Vector2> list = new List<Vector2>();
		Projectile.FillWhipControlPoints(Projectile, list);

		//Main.DrawWhip_WhipBland(Projectile, list);
		// The code below is for custom drawing.
		// If you don't want that, you can remove it all and instead call one of vanilla's DrawWhip methods, like above.
		// However, you must adhere to how they draw if you do.

		List<Vector2> pointsList = new List<Vector2>(list);
		list.RemoveAt(pointsList.Count - 1);
		DrawLine(list);

		SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

		Main.instance.LoadProjectile(Type);
		Texture2D texture = TextureAssets.Projectile[Type].Value;

		Vector2 pos = list[0];

		for (int i = 0; i < list.Count - 1; i++)
		{
			// These two values are set to suit this projectile's sprite, but won't necessarily work for your own.
			// You can change them if they don't!
			Rectangle frame = new Rectangle(0, 0, 14, 78);
			Vector2 origin = new Vector2(frame.Width / 2, 12);
			float scale = 1;

			// These statements determine what part of the spritesheet to draw for the current segment.
			// They can also be changed to suit your sprite.
			if (i == list.Count - 2)
			{
				frame.Y = 64;
				frame.Height = 14;

				// For a more impactful look, this scales the tip of the whip up when fully extended, and down when curled up.
				Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
				float t = Timer / timeToFlyOut;
				scale = MathHelper.Lerp(0.5f, 1.5f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));
			}
			else/* if (i > 0)*/
			{
				frame.Y = 34;
				frame.Height = 16;
			}

			Vector2 element = list[i];
			Vector2 diff = list[i + 1] - element;

			float rotation = diff.ToRotation() - MathHelper.PiOver2; // This projectile's sprite faces down, so PiOver2 is used to correct rotation.
			Color color = Lighting.GetColor(element.ToTileCoordinates());

			Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);

			pos += diff;
		}
		return false;
	}
}