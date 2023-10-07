using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Conquest.Projectiles.Summoner;

namespace Conquest.Buffs;

public class EchoingStarlashBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
    }
    public override void Update(Player player, ref int buffIndex)
    {
        player.GetModPlayer<StarlashPlayer>().buffed = true;
    }
}

public class StarlashPlayer : ModPlayer
{
    public bool buffed = true;

    public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (hit.DamageType == DamageClass.Summon && !ProjectileID.Sets.IsAWhip[proj.type] && buffed)
        {
            Projectile.NewProjectileDirect(proj.GetSource_FromThis(),
                proj.Center, Main.rand.NextVector2Circular(15, 15),
                ModContent.ProjectileType<EchoingStarlashMine>(), 50, 0, proj.owner);
        }
    }

    public override void ResetEffects()
    {
        buffed = false;
    }
}

