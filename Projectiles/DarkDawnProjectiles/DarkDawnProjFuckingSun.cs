using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Conquest.Items.Weapons.Magic;

namespace Conquest.Projectiles.DarkDawnProjectiles {
    public class DarkDawnProjFuckingSun : ModProjectile {
        SoundStyle sunFalling = new SoundStyle("Conquest/Assets/Sounds/SunFalling");
        SoundStyle sunSummoning = new SoundStyle("Conquest/Assets/Sounds/SunSummoning");
        private Color fireColor;
        private Vector2 oldVelocity;
        public override void SetStaticDefaults() {
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
			Main.projPet[Projectile.type] = true;
        }
        public override void SetDefaults() {
            Projectile.width = 288;
            Projectile.height = 288;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 240;
            Projectile.CritChance = 100;
        }
        public override bool MinionContactDamage() {
			return true;
		}
        public override void AI() {

            fireColor = new Color(255, 106, 0);

            for (int k = 0; k < 50; k++) {
                Vector2 eclipsePos = Main.rand.NextVector2CircularEdge(Projectile.width/2, Projectile.width/2);
                Dust eclipseDust = Dust.NewDustPerfect(Projectile.Center + eclipsePos, DustID.RainbowMk2, null, 255, fireColor, 2f);
                eclipseDust.noGravity = true;
                eclipseDust.fadeIn = 0f;
            }
            if(!Main.mouseLeft) {
                Projectile.timeLeft = 0;
            }
            for (int i = 0; i < Main.maxNPCs; i++) {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy()) {
                    if(Projectile.getRect().Intersects(npc.getRect())) {                    
                        Projectile.velocity = oldVelocity/10;
                    }
                    else {
                        Projectile.velocity = oldVelocity;
                    }
                }
            }
            if (Projectile.timeLeft <= 1) {
                for (int k = 0; k < 100; k++) {
                    Vector2 explodePos = Main.rand.NextVector2CircularEdge(Projectile.width/2, Projectile.width/2);
                    Dust explodeDust = Dust.NewDustPerfect(Projectile.Center + explodePos, DustID.RainbowMk2, explodePos/15, 255, fireColor, 2f);
                    explodeDust.noGravity = true;
                    explodeDust.fadeIn = 0f;
                }
            }
            
        }
        public override void OnSpawn(IEntitySource source) {
            SoundEngine.PlaySound(sunFalling　with {Volume = 3.0f});
            SoundEngine.PlaySound(sunSummoning);
            Projectile.velocity = Projectile.Center.DirectionTo(Main.MouseWorld)*10f;
            oldVelocity = Projectile.velocity;
            playerStuff.cameraShaking = true;
        }
        public override void OnKill(int timeLeft) {
            SoundEngine.PlaySound(SoundID.Item163);
            playerStuff.cameraShaking = false;
        }
    }
}

