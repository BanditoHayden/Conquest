using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Conquest.Projectiles.Melee;

public class NightfallProjectile : ModProjectile
{
    // Define the range of the Spear Projectile. These are overrideable properties, in case you'll want to make a class inheriting from this one.
    protected virtual float HoldoutRangeMin => 96f;
    protected virtual float HoldoutRangeMax => 450f;

    public override void SetStaticDefaults()
    {
        //DisplayName.SetDefault("Nightfall");
    }

    float rotCoeff = 1;
    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ProjectileID.Spear); // Clone the default values for a vanilla spear. Spear specific values set for width, height, aiStyle, friendly, penetrate, tileCollide, scale, hide, ownerHitCheck, and melee.
        Projectile.width = 56;
        Projectile.height = 56;
        Projectile.scale = 3f;
        rotCoeff = Main.rand.NextFloat(1f, 3f);
        rotCoeff *= Main.rand.NextBool() ? 1 : -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
    }

    Vector2 effVel;

    public override bool PreAI()
    {
        Player player = Main.player[Projectile.owner]; // Since we access the owner player instance so much, it's useful to create a helper local variable for this
        int duration = player.itemAnimationMax; // Define the duration the projectile will exist in frames

        player.heldProj = Projectile.whoAmI; // Update the player's held projectile id

        // Reset projectile time left if necessary
        if (Projectile.timeLeft > duration)
        {
            Projectile.timeLeft = duration;
        }

        Projectile.velocity = Vector2.Normalize(Projectile.velocity); // Velocity isn't used in this spear implementation, but we use the field to store the spear's attack direction.

        float halfDuration = duration * 0.5f;
        float progress;

        // Here 'progress' is set to a value that goes from 0.0 to 1.0 and back during the item use animation.
        if (Projectile.timeLeft < halfDuration)
        {
            progress = Projectile.timeLeft / halfDuration;
        }
        else
        {
            progress = (duration - Projectile.timeLeft) / halfDuration;
        }

        Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(progress * rotCoeff * Projectile.spriteDirection));

        // Move the projectile from the HoldoutRangeMin to the HoldoutRangeMax and back, using SmoothStep for easing the movement
        Projectile.Center = player.MountedCenter + Vector2.SmoothStep(Projectile.velocity * HoldoutRangeMin, Projectile.velocity * HoldoutRangeMax, progress);

        // Apply proper rotation to the sprite.
        if (Projectile.spriteDirection == -1)
        {
            // If sprite is facing left, rotate 45 degrees
            Projectile.rotation += MathHelper.ToRadians(45f);
        }
        else
        {
            // If sprite is facing right, rotate 135 degrees
            Projectile.rotation += MathHelper.ToRadians(135f);
        }

        effVel = Projectile.velocity - effVel;

        // Avoid spawning dusts on dedicated servers
        if (!Main.dedServ)
        {
            // These dusts are added later, for the 'ExampleMod' effect
            if (Main.rand.NextBool(3))
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GoldFlame, Projectile.velocity.X * 2f, Projectile.velocity.Y * 2f, Alpha: 128, Scale: 1.2f);
            }

            if (Main.rand.NextBool(4))
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GoldFlame, Alpha: 128, Scale: 0.3f);
            }
        }
        return false; // Don't execute vanilla AI.
    }

    bool hit = false;
    public override void OnHitNPC(NPC target, NPC.HitInfo h, int damage)
    {
        if (!hit)
        {
            Projectile.NewProjectile(Main.player[Projectile.owner].GetSource_FromThis(), target.Center + Main.rand.NextVector2Circular(target.width / 1.5f, target.height / 1.5f), Vector2.Zero, ModContent.ProjectileType<NightfallExplosion>(), damage, h.Knockback);
            SoundEngine.PlaySound(SoundID.Item14, target.position);
            hit = true;
        }
        base.OnHitNPC(target, h, damage);
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.FinalDamage *= 1 + (effVel.Length() / 300);
    }
}