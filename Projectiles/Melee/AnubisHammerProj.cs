using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using static Conquest.Assets.Common.Helpers;
using Conquest.Assets.Common;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace Conquest.Projectiles.Melee
{
    public class AnubisHammerProj : Boomerang
    {
        List<float> goodList = new List<float>(new float[10]);
        public override void SetDefaults()
        {
            Projectile.ignoreWater = true;
            Projectile.width = 46;
            Projectile.height = 46;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.aiStyle = -1;

            ReturnSpeed = 60f;
            HomingOnOwnerStrength = 1.2f;
            TravelOutFrames = 30;
            DoTurn = true;
            DoRotation = false;

            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
        }

        float rotation = 0;

        public override bool PreDraw(ref Color lightColor)
        {

            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);

            Vector2 drawPos = (Projectile.position - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
            Main.EntitySpriteDraw(texture, drawPos, null, Projectile.GetAlpha(lightColor), rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            default(Effects.PurpleTrailRotationless).Draw(Projectile);
            rotation += Rotation;
            return false;
        }

        NPC farTarget = null;
        NPC target = null;
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.ai[0] == 0f)
            {
                farTarget = Helpers.GetClosestEnemy(Projectile.Center, 40f * 16f, true, true);
                target = Helpers.GetClosestEnemy(Projectile.Center, 20f * 16f, true, true);

                // If there's an npc near the boomerang, we want to move towards it
                if (target != null)
                {
                    DoTurn = false;
                    // Add to our velocity 
                    float maxVelocity = ReturnSpeed * Owner.GetAttackSpeed(DamageClass.Melee);
                    float homingStrength = 0.9f;
                    Vector2 toEnemy = target.Center - Projectile.Center;
                    toEnemy.Normalize();
                    toEnemy *= homingStrength;
                    Projectile.velocity += toEnemy;
                    if (Projectile.velocity.LengthSquared() > maxVelocity * maxVelocity)
                    {
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= maxVelocity;
                    }
                }

                DoTurn = true;

            }
            base.AI();
        }

        public override void OnReachedApex()
        {
            if (farTarget != null)
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(),
                    Projectile.position, Projectile.DirectionTo(farTarget.Center) * Projectile.velocity.Length(),
                    ModContent.ProjectileType<AnubisHammerShadow>(), Projectile.damage / 2, 0, Projectile.owner);
            }
        }
    }
}
