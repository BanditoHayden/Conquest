using Conquest.Assets.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace Conquest.NPCs.Bosses.Anubis
{
    public class AnubisDisc : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50;    //The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;               
            Projectile.height = 22;           
            Projectile.aiStyle = 1;          
            Projectile.hostile = true;         
                                             
            Projectile.penetrate = 1;           
            Projectile.timeLeft = 900;        
            Projectile.alpha = 255;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
            AIType = ProjectileID.Bullet;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            default(Effects.WhiteTrail).Draw(Projectile);

            return true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            return false;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            var settings = new ParticleOrchestraSettings
            {
                PositionInWorld = Projectile.Center,
                MovementVector = Projectile.velocity
            };
            ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.RainbowRodHit, settings);
        }
    }
}
