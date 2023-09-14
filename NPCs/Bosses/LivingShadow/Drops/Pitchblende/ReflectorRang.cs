using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Conquest.Assets.Common;
using Terraria.GameContent;

namespace Conquest.NPCs.Bosses.LivingShadow.Drops.Pitchblende;

public class ReflectorRang : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 10;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 19;
        Projectile.height = 19;
        Projectile.damage = 101;
        Projectile.scale = 1.5f;

        Projectile.friendly = true;
        Projectile.hostile = false;

        Projectile.tileCollide = true;
        Projectile.ignoreWater = false;
        Projectile.timeLeft = 350;
        Projectile.penetrate = 999;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 15;

        Projectile.ArmorPenetration = 35;

    }

    int hitCounter = 0;
    List<NPC> hit = new List<NPC>();

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.velocity.X != oldVelocity.X) Projectile.velocity.X *= -1;
        if (Projectile.velocity.Y != oldVelocity.Y) Projectile.velocity.Y *= -1;
        return false;
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
        rotation += thing * MathHelper.ToRadians(4 * MathF.Sqrt(Projectile.ai[0]));

        if (closeNPC != null && hitCounter <= 6)
        {
            default(Effects.WhiteTrailRotationless).Draw(Projectile);
        }
        else
        {
            default(Effects.BlackTrailRotationless).Draw(Projectile);
        }
        return false;
    }

    float thing = Main.rand.NextFloat(0.7f, 1.5f);
    NPC closeNPC = null;

    public void ChooseNPC()
    {
        foreach (NPC npc in Main.npc)
        {
            if (!npc.active) continue;
            if (npc.friendly) continue;
            if (npc.type == NPCID.TargetDummy) continue;
            if (npc.life <= 0) continue;
            if (npc.dontTakeDamage) continue;
            if (npc.Distance(Projectile.position) > 400) continue;
            if (hit.IndexOf(npc) == 0 || hit.IndexOf(npc) == 1) continue;
            closeNPC = npc;
            break;
        }
    }

    public void UnchooseNPC()
    {
        if (!closeNPC.active) closeNPC = null;
        if (closeNPC.friendly) closeNPC = null;
        if (closeNPC.type == NPCID.TargetDummy) closeNPC = null;
        if (closeNPC.life <= 0) closeNPC = null;
        if (closeNPC.dontTakeDamage) closeNPC = null;
        if (closeNPC.Distance(Projectile.position) > 400) closeNPC = null;
        if (hit.IndexOf(closeNPC) == 0 || hit.IndexOf(closeNPC) == 1) closeNPC = null;
    }

    int dustType = DustID.Wraith;
    float dustScale = 2f;

    public override void AI()
    {
        Lighting.AddLight(Projectile.position, new Vector3(0.4f, 0.4f, 0.8f));
        Vector2 offset = new Vector2(8, 0);

        if (Projectile.ai[0] > 250) Projectile.tileCollide = false;
        if (Projectile.ai[0] == 0 && Main.rand.NextBool()) thing *= -1;
        Vector2 toTarget = Projectile.position.DirectionTo(Main.player[Projectile.owner].position) * 4 * MathF.Sqrt(Projectile.ai[0]);

        if (closeNPC == null || hitCounter > 6)
        {
            ChooseNPC();
            if (closeNPC == null || Projectile.Distance(Main.player[Projectile.owner].position) > 12 || Projectile.ai[0] < 40) Projectile.velocity = Vector2.Lerp(Projectile.velocity, toTarget, 0.1f);
            else Projectile.position = Vector2.Lerp(Projectile.velocity, Main.player[Projectile.owner].position, 0.1f);
        }
        else
        {
            UnchooseNPC();
            if (closeNPC != null)
            {
                Vector2 toNPC = Projectile.position.DirectionTo(closeNPC.position) * 4 * MathF.Sqrt(Projectile.ai[0]);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, toNPC, 0.1f);
            }
        }

        if (Projectile.Distance(Main.player[Projectile.owner].position) < 24 && Projectile.ai[0] > 40)
        {
            Projectile.Kill();
        }

        Projectile.ai[0]++;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo h, int damage)
    {
        if (!target.active || target.life <= 0) closeNPC = null;
        if (hitCounter <= 6)
        {
            Projectile.velocity *= -1;
            Projectile.ArmorPenetration -= 5;
        }
        hitCounter++; 
        hit.Insert(0, target);
        base.OnHitNPC(target, h, damage);
    }
}

