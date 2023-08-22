using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Conquest.Items.Weapons.Magic.Energy;

public abstract class EnergyWeapon : ModItem
{
    public float damageScale = 1f;
    public float knockbackScale = 1f;
    public float velocityScale = 1f;

    int baseDamage;
    float baseKnockback, baseVelocity;

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        float energy = player.GetModPlayer<EnergyPlayer>().energyPower;
        damage = (int)MathF.Round(damage * energy * damageScale);
        knockback *= energy * knockbackScale;
        velocity *= energy * velocityScale;
    }
}

