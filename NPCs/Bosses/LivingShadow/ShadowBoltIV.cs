using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Conquest.NPCs.Bosses.LivingShadow;

public class ShadowBoltIV : ModNPC
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
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.ShowNameOnHover = false;
        NPC.dontTakeDamage = true;
        NPC.npcSlots = 0f;
        NPC.buffImmune[BuffID.Confused] = true;
        NPCID.Sets.TrailCacheLength[NPC.type] = 5;
        NPCID.Sets.TrailingMode[NPC.type] = 0;
    }

    bool did = false;

    bool oneOldEnough = false;

    Vector2 vel = Vector2.Zero;

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
            if (Main.getGoodWorld && NPC.ai[1] != 6) NPC.NewNPCDirect(NPC.GetSource_FromThis(),
                                                                      NPC.Center,
                                                                      NPC.type,
                                                                      ai1: 6);
        }

        foreach (NPC npc in Main.npc)
        {
            if (npc.type == NPC.type && npc.ai[0] >= 300)
            {
                int choice = Main.rand.Next(0, 100);
                if (choice < 25) oneOldEnough = true;
            }
        }

        if (oneOldEnough)
        {
            vel = NPC.Center.DirectionTo(player.Center) * 16;
            if (!did) NPC.velocity = vel;
        }
        else
        {
            NPC.velocity /= 1.08f;
        }

        did = oneOldEnough;

        if (NPC.ai[0] > Main.rand.Next(350, 450)) NPC.life--;


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

