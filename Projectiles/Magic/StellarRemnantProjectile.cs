using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Conquest.Projectiles.Magic;

public class StellarRemnantProjectile : ModProjectile
{
    public override void SetStaticDefaults()
    {
        //DisplayName.SetDefault("Stellar Remnant");
        Main.projFrames[Projectile.type] = 4;
    }

    public override void SetDefaults()
    {
        Projectile.friendly = true; // Can the projectile deal damage to enemies?
        Projectile.DamageType = DamageClass.Magic; // Is the projectile shoot by a ranged weapon?
        Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
        Projectile.tileCollide = false; // Can the projectile collide with tiles?
        Projectile.damage = 99;
        Projectile.knockBack = 3;
        Projectile.aiStyle = -1;
        Projectile.width = 16;
        Projectile.height = 15;
        Projectile.timeLeft = Main.rand.Next(300, 350);
    }

    float rotation = 0;

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileID.BlackBolt, Projectile.damage, Projectile.knockBack);
        p.timeLeft = 0;
        p.DamageType = DamageClass.Magic;
        return base.OnTileCollide(oldVelocity);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damage)
    {
        Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileID.BlackBolt, Projectile.damage, Projectile.knockBack).timeLeft = 1;
        base.OnHitNPC(target, hit, damage);
    }

    float vel;
    bool tooCloseForComfort = false;
    public override void AI()
    {
        if (Projectile.ai[0] == 0)
        {
            Projectile.velocity.X *= -4;
            Projectile.velocity.Y *= 3;
            if (Main.rand.NextBool()) Projectile.velocity.Y *= -1;
        }
        if (Projectile.ai[0] < 30 && !tooCloseForComfort)
        {
            Vector2 toTarget = Projectile.Center.DirectionTo(Main.MouseWorld) * 22;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, toTarget, 0.07f);
        }

        if (Vector2.Distance(Projectile.Center, Main.MouseWorld) <= 4) tooCloseForComfort = true;

        foreach (NPC npc in Main.npc)
        {
            if (!npc.active) continue;
            if (npc.friendly) continue;
            if (npc.life <= 0) continue;
            if (npc.dontTakeDamage) continue;
            if (npc.type == NPCID.TargetDummy) continue;
            if (npc.Distance(Projectile.position) > 400) continue;
            Vector2 toTarget = Projectile.Center.DirectionTo(npc.Center) * 22;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, toTarget, 0.09f);
            break;
        }
        Vector2 offset = new Vector2(8, 0);
        if (Main.rand.NextBool())
            Dust.NewDustDirect(Projectile.Center + (offset.RotatedBy(rotation)), 1, 1, DustID.ShadowbeamStaff).noGravity = true;
        if (Main.rand.NextBool())
            Dust.NewDustDirect(Projectile.Center + (offset.RotatedBy(rotation + MathF.PI)), 1, 1, DustID.ShadowbeamStaff).noGravity = true;
        if (Main.rand.NextBool())
            Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.Obsidian).noGravity = true;
        rotation += MathF.PI / 22.5f;
        if (++Projectile.frameCounter >= 4)
        {
            Projectile.frameCounter = 0;
            // Or more compactly Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
            if (++Projectile.frame >= Main.projFrames[Projectile.type])
                Projectile.frame = 0;
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
}

