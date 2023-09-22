
using Conquest.Assets.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Conquest.NPCs.Bosses.Anubis
{
    public class AnubisChaser : ModProjectile
    {
        public override string Texture => $"{nameof(Conquest)}/NPCs/Bosses/Anubis/AnubisBeam";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 400;    //The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;               //The width of projectile hitbox
            Projectile.height = 22;              //The height of projectile hitbox
            Projectile.aiStyle = 1;             //The ai style of the projectile, please reference the source code of Terraria
            Projectile.hostile = true;         //Can the projectile deal damage to the player?
                                               //projectile.minion = true;           //Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = 5;           //How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 500;          //The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 255;           
            Projectile.light = 0.5f;
            Projectile.extraUpdates = 1;
            Projectile.ignoreWater = true;          
            Projectile.tileCollide = false;                      
            AIType = ProjectileID.Bullet;           
        }
        public override bool PreDraw(ref Color lightColor)
        {
            default(Effects.PurpleTrail).Draw(Projectile);

            return false;
        }

        public override void AI()
        {
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.Center.DirectionTo(Main.player[Projectile.owner].Center) * 4f, 0.025f);
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void Kill(int timeLeft)
        {
            if (!Main.dedServ)
            {
                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
                {
                    Dust d = Dust.NewDustDirect(Projectile.oldPos[i], 1, 1, DustID.PurpleTorch, Scale: 1.25f);
                    d.velocity = Vector2.Zero;
                    d.noGravity = true;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X) Projectile.velocity.X *= -1;
            if (Projectile.velocity.Y != oldVelocity.Y) Projectile.velocity.Y *= -1;
            if (Projectile.penetrate-- < 1) Projectile.Kill();
            return false;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
    }
}
