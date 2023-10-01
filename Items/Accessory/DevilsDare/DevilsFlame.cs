using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

using Terraria.DataStructures;
using Terraria.Audio;

namespace Conquest.Items.Accessory.DevilsDare
{
    internal class DevilsFlame : ModProjectile
    {
		SoundStyle Spawn = new SoundStyle($"{nameof(Conquest)}/Items/Accessory/DevilsDare/Devils_Flame")
        {
            Volume = 1.2f,
            PitchVariance = 0.2f,
            MaxInstances = 3,
        };
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.alpha = 0;
            Projectile.light = 0;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
			Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
        }
		public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.Item8);
        }
		public override void OnKill(int timeLeft)
        {
			SoundEngine.PlaySound(SoundID.Item8);
		}
		 public override void AI()
        {
			Player player = Main.player[Projectile.owner];
			if (player.GetModPlayer<DevilsDareEffects>().DevilsVisuals)
				Projectile.timeLeft = 2;
			if (player.direction == -1)
			{
				Projectile.position.X = (player.Center.X+Projectile.position.X+3)/2;
				Projectile.rotation = -Math.Min(Math.Max((player.Center.X-Projectile.position.X+3)/20,-1),1);
			}
			else
			{
				Projectile.position.X = (player.Center.X+Projectile.position.X-11)/2;
				Projectile.rotation = -Math.Min(Math.Max((player.Center.X-Projectile.position.X-11)/20,-1),1);
			}
			Projectile.position.Y = (player.position.Y-16+Projectile.position.Y)/2;
			Lighting.AddLight(Projectile.position, (new Vector3(259,73,73)) * 0.0025f * Main.essScale);
			Projectile.spriteDirection = player.direction;
			
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
				if (++Projectile.frame >= 3)
				{
					Projectile.frame = 0;
				}
            }
        }
	}
}