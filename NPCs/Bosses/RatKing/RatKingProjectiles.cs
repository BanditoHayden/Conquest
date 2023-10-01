using Conquest.Assets.Common;
using Conquest.Buffs;
using Conquest.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Conquest.NPCs.Bosses.RatKing
{
    internal class RatKingBeam : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50;    //The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;               //The width of projectile hitbox
            Projectile.height = 42;              //The height of projectile hitbox
            Projectile.aiStyle = 1;             //The ai style of the projectile, please reference the source code of Terraria
            Projectile.hostile = true;         //Can the projectile deal damage to the player?
                                               //projectile.minion = true;           //Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = 1;           //How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 900;          //The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 255;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
            AIType = ProjectileID.Bullet;
        }
        Vector2 AngleTowardsHoming(Vector2 currentVel, Vector2 currentPos, Vector2 target)
        {
            float maxTurnRadians = 0.1f;
            return currentVel.ToRotation().AngleTowards((target - currentPos).ToRotation(), maxTurnRadians).ToRotationVector2() * currentVel.Length();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player target = Main.player[(int)Projectile.ai[0]];
            int lineLength = 1000;
            Vector2 posOffset = AngleTowardsHoming(Projectile.velocity, Projectile.Center, target.Center);
            Vector2 velOffset = posOffset;
            for (int i = 0; i < lineLength; i++)
            {
                //draw dot at pos offset + projectile center
                velOffset = AngleTowardsHoming(velOffset, posOffset, target.Center);
                posOffset += velOffset;

            }
            default(Effects.WhiteTrail).Draw(Projectile);
            return base.PreDraw(ref lightColor);
        }
       
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
    }
    public class RatKingMoonSlash : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 138;
            Projectile.height = 112;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = 100;
            Projectile.timeLeft = 70;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;

        }

        public bool FadedIn
        {
            get => Projectile.localAI[0] == 1f;
            set => Projectile.localAI[0] = value ? 1f : 0f;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            
           
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation += MathHelper.Pi;

            }
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipVertically;
            }
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return true;
        }
    }
    public class RatKingMoonSlash2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 202;
            Projectile.height = 90;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = 100;
            Projectile.timeLeft = 70;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;

        }
        public override bool? CanCutTiles()
        {
            return false;
        }
        public bool FadedIn
        {
            get => Projectile.localAI[0] == 1f;
            set => Projectile.localAI[0] = value ? 1f : 0f;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];


            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation += MathHelper.Pi;

            }
        }
        
        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipVertically;
            }
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return true;
        }
    }
    public class CheeseRelease : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = 100;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = true;
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
            Projectile.velocity.Y = Projectile.velocity.Y + 0.1f; 
            if (Projectile.velocity.Y > 16f) 
            {
                Projectile.velocity.Y = 16f;
            }
        }
        public override void OnKill(int timeLeft)
        {
            var entitySource = Projectile.GetSource_FromThis();
            Projectile.NewProjectile(entitySource, Projectile.Center.X, Projectile.Center.Y, Main.rand.Next(0, 0), Main.rand.Next(0,0), ModContent.ProjectileType<BallOfLight>(), 30, 0f, Projectile.owner);
            SoundEngine.PlaySound(SoundID.DrumKick);
        }
    }
    public class BallOfLight : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = 100;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
            var entitySource = Projectile.GetSource_FromThis();
            Projectile.NewProjectile(entitySource, Projectile.Center.X, Projectile.Center.Y, Main.rand.Next(-10, 11) * .25f, Main.rand.Next(-10, -5) * .25f, ModContent.ProjectileType<Cheese>(), 60, 0f, Projectile.owner);
        }
    }
    public class Cheese : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 36;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = 100;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = true;
            AIType = ProjectileID.WoodenArrowHostile;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

        }
    }
    public class Cheese2 : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 36;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = 100;
            Projectile.timeLeft = 480;
            Projectile.tileCollide = false;
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50;    //The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }
        public override void AI()
        {
          
		}
        
        public override bool PreDraw(ref Color lightColor)
        {
            default(Effects.GoldTrail).Draw(Projectile);

            return true;
        }
    }




}
