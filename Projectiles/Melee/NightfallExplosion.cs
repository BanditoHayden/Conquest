using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Conquest.Projectiles.Melee;

public class NightfallExplosion : ModProjectile
{
    public override void SetStaticDefaults()
    {
        //DisplayName.SetDefault("Nightfall");
        Main.projFrames[Projectile.type] = 5;
    }

    public override void SetDefaults()
    {
        Projectile.friendly = true; // Can the projectile deal damage to enemies?
        Projectile.DamageType = DamageClass.Melee; // Is the projectile shoot by a ranged weapon?
        Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
        Projectile.tileCollide = false; // Can the projectile collide with tiles?
        Projectile.damage = 90;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.knockBack = 7;
        Projectile.width = 25;
        Projectile.height = 25;
        Projectile.timeLeft = 60;
        Projectile.penetrate = 999;
        Projectile.scale = 1.5f;
        Projectile.localNPCHitCooldown = -1;
        Projectile.usesLocalNPCImmunity = true;
    }

    public override void AI()
    {
        Dust.NewDustDirect(Projectile.Center, Projectile.width, Projectile.height, DustID.GoldFlame).noGravity = true;
        if (++Projectile.frameCounter >= 2)
        {
            Projectile.frameCounter = 0;
            // Or more compactly Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
            if (++Projectile.frame >= Main.projFrames[Projectile.type])
                Projectile.Kill();
        }
        Projectile.ai[0]++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        // SpriteEffects helps to flip texture horizontally and vertically
        SpriteEffects spriteEffects = SpriteEffects.None;
        if (Projectile.spriteDirection == -1)
            spriteEffects = SpriteEffects.FlipHorizontally;

        // Getting texture of projectile
        Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

        // Calculating frameHeight and current Y pos dependence of frame
        // If texture without animation frameHeight is always texture.Height and startY is always 0
        int frameHeight = texture.Height / Main.projFrames[Projectile.type];
        int startY = frameHeight * Projectile.frame;

        // Get this frame on texture
        Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

        // Alternatively, you can skip defining frameHeight and startY and use this:
        // Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);

        Vector2 origin = sourceRectangle.Size() / 2f;

        // If image isn't centered or symmetrical you can specify origin of the sprite
        // (0,0) for the upper-left corner
        float offsetX = 20f;
        origin.X = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX);

        // If sprite is vertical
        // float offsetY = 20f;
        // origin.Y = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Height - offsetY : offsetY);


        // Applying lighting and draw current frame
        Color drawColor = Projectile.GetAlpha(lightColor);
        Main.EntitySpriteDraw(texture,
            Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
            sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

        // It's important to return false, otherwise we also draw the original texture.
        return false;
    }

    public override void ModifyDamageHitbox(ref Rectangle hitbox)
    {
        // By using ModifyDamageHitbox, we can allow the flames to damage enemies in a larger area than normal without colliding with tiles.
        // Here we adjust the damage hitbox. We adjust the normal 6x6 hitbox and make it 66x66 while moving it left and up to keep it centered.
        int size = 80;
        hitbox.X -= size;
        hitbox.Y -= size;
        hitbox.Width += size * 2;
        hitbox.Height += size * 2;
    }
}

