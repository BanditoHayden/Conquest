using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.DataStructures;
using Terraria.Audio;
using Conquest.Assets.Common;
using Conquest.Projectiles.Ranged;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.UI.Chat;

namespace Conquest.Items.Weapons.Ranged;

public class TimestreamRevolver : ModItem
{
    Effect GoldenFX;
    static void SetEffectParameters(Effect effect)
    {
        effect.Parameters["uTime"].SetValue((float)(Main.timeForVisualEffects * 0.032f));
    }
    static bool ShaderTooltip(DrawableTooltipLine line, Effect shader)
    {
        Vector2 textPos = new Vector2(line.X, line.Y);
        for (float i = 0; i < 1; i += 0.25f)
        {
            Vector2 borderOffset = (i * MathF.Tau).ToRotationVector2() * 2;
            ChatManager.DrawColorCodedString(Main.spriteBatch, line.Font, line.Text, textPos + borderOffset, Color.Black, line.Rotation, line.Origin, line.BaseScale);
        }
        SetEffectParameters(shader);
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, shader, Main.UIScaleMatrix);
        ChatManager.DrawColorCodedString(Main.spriteBatch, line.Font, line.Text, textPos, Color.Red, line.Rotation, line.Origin, line.BaseScale);
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
        return false;
    }
    public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
    {
        if (GoldenFX == null)
            GoldenFX = ModContent.Request<Effect>("Conquest/Assets/Shaders/Gradient", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
        if (line.Index == 0)
        {
            return ShaderTooltip(line, GoldenFX);
        }
        return true;
    }

    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 1;
    }

    SoundStyle Reload = new SoundStyle($"{nameof(Conquest)}/Assets/Sounds/TRevolverReload")
    {
        Volume = 0.9f,
        PitchVariance = 0.1f,
        MaxInstances = 3,
    };
    SoundStyle Fire = new SoundStyle($"{nameof(Conquest)}/Assets/Sounds/TRevolver")
    {
        Volume = 0.9f,
        PitchVariance = 0.2f,
        MaxInstances = 7,
    };
    SoundStyle Rewind = new SoundStyle($"{nameof(Conquest)}/Assets/Sounds/TRevolverRewind")
    {
        Volume = 0.9f,
        PitchVariance = 0.1f,
        MaxInstances = 3,
    };

    int bullets = 6, bulletsMax = 6;
    int reloadCooldown = 0;
    public override void SetDefaults()
    {
        // Common Properties
        Item.width = 27;
        Item.height = 15;
        Item.rare = ModContent.RarityType<ArtifactRarity>();
        Item.value = Item.buyPrice(gold: 33);
        Item.noMelee = true;
        // Use Properties
        Item.useTime = 14;
        Item.useAnimation = 14;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.noUseGraphic = false;
        Item.autoReuse = true;

        // Weapon Properties
        Item.damage = 44;
        Item.knockBack = 0.5f;
        Item.DamageType = DamageClass.Ranged;
        // Projectile Properties
        Item.shoot = ModContent.ProjectileType<TRevolverShot>();
        Item.shootSpeed = 10f;
    }
    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-3, 0);
    }

    public override bool CanUseItem(Player player)
    {
        return reloadCooldown <= 0;
    }

    public override bool AltFunctionUse(Player player)
    {
        return reloadCooldown <= 0;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (player.altFunctionUse == 2)
        {
            if (player.ownedProjectileCounts[Item.shoot] > 0) SoundEngine.PlaySound(Rewind, position);

            type = ProjectileID.None;
            if (velocity.X < 0)
            {
                velocity = new Vector2(-velocity.Length(), 0).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(20f, 50f)));
            }
            else
            {
                velocity = new Vector2(velocity.Length(), 0).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-20f, 50f)));
            }
        }
        else
        {
            SoundEngine.PlaySound(Fire, position);
        }
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        bullets--;
        if (bullets < 1)
        {
            reloadCooldown = 45;
        }
        return base.Shoot(player, source, position, velocity, type, damage, knockback);
    }

    public override void UpdateInventory(Player player)
    {
        Item.SetNameOverride("Timestream Revolver - " + bullets + "/" + bulletsMax);

        if (player.HeldItem == Item)
        {
            if (Keybinds.Reload.JustPressed && bullets < bulletsMax) reloadCooldown = 45;

            if (reloadCooldown == 45)
            {
                SoundEngine.PlaySound(Reload, player.position);
            }
            reloadCooldown--;
            if (reloadCooldown == 0)
            {
                bullets = bulletsMax;
                reloadCooldown = -1;
            }
        }
        else if (reloadCooldown > 0) reloadCooldown = 45;
    }
}
