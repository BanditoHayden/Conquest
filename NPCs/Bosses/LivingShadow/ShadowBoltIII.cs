using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Conquest.NPCs.Bosses.LivingShadow;

public class ShadowBoltIII : ModNPC
{
    public override void SetStaticDefaults()
    {
        //DisplayName.SetDefault("Shadow Bolt");
    }

    public override void SetDefaults()
    {
        NPC.width = 6;
        NPC.height = 6;
        NPC.lifeMax = 1;
        NPC.defense = 0;
        NPC.damage = 67;
        NPC.npcSlots = 0f;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.ShowNameOnHover = false;
        NPC.dontTakeDamage = true;
        NPC.buffImmune[BuffID.Confused] = true;
        NPCID.Sets.TrailCacheLength[NPC.type] = 5;
        NPCID.Sets.TrailingMode[NPC.type] = 0;
    }

    float velocity = 1;
    Vector2 vel;
    public override void AI()
    {
        if (NPC.FindBuffIndex(BuffID.Confused) != -1) NPC.DelBuff(NPC.FindBuffIndex(BuffID.Confused));
        if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
        {
            NPC.TargetClosest();
        }
        Player player = Main.player[NPC.target];

        if (player.HeldItem.damage != 0) NPC.damage = player.HeldItem.damage < 100 ? player.HeldItem.damage * 2 : player.HeldItem.damage;
        else NPC.damage = 67;

        if (!Main.dayTime) NPC.damage = 9999;

        if (NPC.ai[0] == 0)
        {
            vel = Main.rand.NextVector2CircularEdge(1, 1);
        }
        NPC.velocity = vel * velocity;
        if (Main.getGoodWorld) NPC.velocity *= 1.4f;
        velocity += 0.1f;
        if (velocity > 20) velocity = 20;

        Dust.NewDustDirect(NPC.Center, 4, 4, DustID.Wraith).noGravity = true;
        NPC.velocity.Y += 0.25f;
        NPC.velocity /= 1.001f;

        if (NPC.collideX) NPC.velocity.X *= -0.9f;
        if (NPC.collideY) NPC.velocity.Y *= -0.9f;

        if (NPC.ai[0] > 500) NPC.life--;

        NPC.ai[0]++;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        NPC.rotation = (float)Math.Atan2((double)NPC.velocity.Y, (double)NPC.velocity.X);
        Main.instance.LoadProjectile(NPC.type);
        Texture2D texture = TextureAssets.Npc[NPC.type].Value;

        // Redraw the projectile with the color not influenced by light
        Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
        for (int k = 0; k < NPC.oldPos.Length; k++)
        {
            Vector2 drawPos = (NPC.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, NPC.gfxOffY);
            Color color = NPC.GetAlpha(drawColor) * ((NPC.oldPos.Length - k) / (float)NPC.oldPos.Length);
            Main.EntitySpriteDraw(texture, drawPos, null, color, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
        }

        return true;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit)
    {
        NPC.ai[0] = 301;
    }

    public override void OnKill()
    {
        int id = Projectile.NewProjectile(NPC.GetSource_FromThis(),
                                          NPC.Center,
                                          Vector2.Zero,
                                          ProjectileID.BlackBolt,
                                          67, 0);
        Main.projectile[id].friendly = false;
        Main.projectile[id].hostile = true;

    }
}

