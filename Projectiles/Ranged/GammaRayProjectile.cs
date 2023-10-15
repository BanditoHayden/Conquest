using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.Collections.Generic;
using Conquest.Buffs;
using Conquest.Items.Weapons.Ranged;

public class GammaRayProjectile : ModProjectile
{
    List<NPC> hit = new List<NPC>();

    float rotation;
    float scaleMult = 2;

    public override void SetDefaults()
    {
        Projectile.damage = 156;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.width = 6;
        Projectile.height = 6;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.tileCollide = true;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 300;
        Projectile.penetrate = 999;
        Projectile.extraUpdates = 499;
        rotation = Main.player[Projectile.owner].itemRotation;

        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = -1;
    }

    bool hasHit = false;

    public override void AI()
    {
        scaleMult = hasHit ? 2f : 1f;
        bool foundTarget = false;
        if (Projectile.localAI[0] == 0)
        {
            for (int i = 0; i < 3; i++)
            {
                Dust.NewDustDirect(Projectile.position, 1, 1, DustID.Vortex).velocity =
                    Projectile.velocity.RotatedBy((MathF.PI / 2 * (Main.rand.NextBool() ? 1 : -1)) + Main.rand.NextFloat(-0.39f, 0.39f)) * 8;
            }
        }

        if (Projectile.ai[0] > 5)
        {
            if ((int)Projectile.ai[0] % Main.rand.Next(4, 9) == 0)
            {
                foreach (NPC npc in Main.npc)
                {
                    if (!npc.active) continue;
                    if (npc.life <= 0) continue;
                    if (npc.friendly) continue;
                    if (npc.Center.Distance(Projectile.Center) > 600) continue;
                    if (npc.type == NPCID.TargetDummy) continue;
                    if (hit.Contains(npc)) continue;

                    foundTarget = true;
                    hit.Add(npc);
                    Vector2 newVel = Projectile.DirectionTo(npc.position) * Projectile.velocity.Length();
                    Projectile.velocity = newVel;
                    for (int i = 0; i < 3; i++)
                    {
                        float biggerScreeny = MathF.Max(Main.screenWidth, Main.screenHeight);
                        Vector2 pos = new Vector2(Projectile.position.X, Projectile.position.Y - Projectile.height / 4)
                                                     - Projectile.velocity * (i / 4f * Main.rand.NextFloat(0.9f, 1.1f));
                    }
                    break;
                }

                if (!foundTarget)
                {
                    Projectile.velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-MathF.PI / 6, MathF.PI / 6));
                }
            }
        }
        

        Projectile.localAI[0] += 1f; //The timer
        if (Projectile.localAI[0] > 1f) //The amount of ticks it takes for it to load
        {
            for (int num562 = 0; num562 < 5; num562++) //Adjust the thickness of the ray; too high a number may cause lag
            {
                Projectile.alpha = 255;
                //Dust.newDust(Vector2 position, Size.X, Size.Y, int Dust ID, Velocity.X, Velocity.Y, int alpha (transparency), Color, float scale)
                //Size determines where the dust will spawn (at random of course)
                //Dust ID's can be found in this handy page http://tconfig.wikia.com/wiki/List_of_Dusts
                if (!foundTarget)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        Dust dust = Dust.NewDustDirect(new Vector2(Projectile.position.X, Projectile.position.Y - Projectile.height / 4)
                                             + Projectile.velocity * (i / 4f * Main.rand.NextFloat(0.9f, 1.1f)),
                                               Projectile.width, Projectile.height,
                        DustID.Vortex, Scale: (Projectile.damage / 156f) * scaleMult);
                        dust.noGravity = true;
                        dust.velocity *= 0.2f;
                    }
                }
            }
        }
    }

    public override void OnKill(int timeLeft)
    {
        Main.player[Projectile.owner].HeldItem.useTime = Main.player[Projectile.owner].HeldItem.useAnimation;
        base.OnKill(timeLeft);
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        if (Main.player[Projectile.owner].HeldItem.useTime > 10
         && Main.player[Projectile.owner].HeldItem.type == ModContent.ItemType<GammaRay>())
        {
            Main.player[Projectile.owner].HeldItem.useTime--;
            Main.player[Projectile.owner].HeldItem.useAnimation--;
        }
        if (Projectile.damage / 156f > 1.5f) target.AddBuff(ModContent.BuffType<VortexFlames>(), 800);
        else target.AddBuff(ModContent.BuffType<VortexFlames>(), 100);
    }

    public override void ModifyDamageHitbox(ref Rectangle hitbox)
    {
        // By using ModifyDamageHitbox, we can allow the flames to damage enemies in a larger area than normal without colliding with tiles.
        // Here we adjust the damage hitbox. We adjust the normal 6x6 hitbox and make it 66x66 while moving it left and up to keep it centered.
        int size = 50;
        hitbox.X -= size;
        hitbox.Y -= size;
        hitbox.Width += size * 2;
        hitbox.Height += size * 2;
    }

namespace Conquest.Projectiles.Ranged
{
public class GammaRayProjectile : ModProjectile
{
    List<NPC> hit = new List<NPC>();

    float rotation;
    float scaleMult = 2;

    public override void SetDefaults()
    {
        Projectile.damage = 156;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.width = 6;
        Projectile.height = 6;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.tileCollide = true;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 300;
        Projectile.penetrate = 999;
        Projectile.extraUpdates = 499;
        rotation = Main.player[Projectile.owner].itemRotation;

        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = -1;
    }

    bool hasHit = false;

    public override void AI()
    {
        scaleMult = hasHit ? 2f : 1f;
        bool foundTarget = false;
        if (Projectile.localAI[0] == 0)
        {
            for (int i = 0; i < 3; i++)
            {
                Dust.NewDustDirect(Projectile.position, 1, 1, DustID.Vortex).velocity =
                    Projectile.velocity.RotatedBy((MathF.PI / 2 * (Main.rand.NextBool() ? 1 : -1)) + Main.rand.NextFloat(-0.39f, 0.39f)) * 8;
            }
        }

        if (Projectile.ai[0] > 5)
        {
            if ((int)Projectile.ai[0] % Main.rand.Next(4, 9) == 0)
            {
                foreach (NPC npc in Main.npc)
                {
                    if (!npc.active) continue;
                    if (npc.life <= 0) continue;
                    if (npc.friendly) continue;
                    if (npc.Center.Distance(Projectile.Center) > 600) continue;
                    if (npc.type == NPCID.TargetDummy) continue;
                    if (hit.Contains(npc)) continue;

                    foundTarget = true;
                    hit.Add(npc);
                    Vector2 newVel = Projectile.DirectionTo(npc.position) * Projectile.velocity.Length();
                    Projectile.velocity = newVel;
                    for (int i = 0; i < 3; i++)
                    {
                        float biggerScreeny = MathF.Max(Main.screenWidth, Main.screenHeight);
                        Vector2 pos = new Vector2(Projectile.position.X, Projectile.position.Y - Projectile.height / 4)
                                                     - Projectile.velocity * (i / 4f * Main.rand.NextFloat(0.9f, 1.1f));
                    }
                    break;
                }

                if (!foundTarget)
                {
                    Projectile.velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-MathF.PI / 6, MathF.PI / 6));
                }
            }
        }
        

        Projectile.localAI[0] += 1f; //The timer
        if (Projectile.localAI[0] > 1f) //The amount of ticks it takes for it to load
        {
            for (int num562 = 0; num562 < 5; num562++) //Adjust the thickness of the ray; too high a number may cause lag
            {
                Projectile.alpha = 255;
                //Dust.newDust(Vector2 position, Size.X, Size.Y, int Dust ID, Velocity.X, Velocity.Y, int alpha (transparency), Color, float scale)
                //Size determines where the dust will spawn (at random of course)
                //Dust ID's can be found in this handy page http://tconfig.wikia.com/wiki/List_of_Dusts
                if (!foundTarget)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        Dust dust = Dust.NewDustDirect(new Vector2(Projectile.position.X, Projectile.position.Y - Projectile.height / 4)
                                             + Projectile.velocity * (i / 4f * Main.rand.NextFloat(0.9f, 1.1f)),
                                               Projectile.width, Projectile.height,
                        DustID.Vortex, Scale: (Projectile.damage / 156f) * scaleMult);
                        dust.noGravity = true;
                        dust.velocity *= 0.2f;
                    }
                }
            }
        }
    }
    public override void OnKill(int timeLeft)
    {
        Main.player[Projectile.owner].HeldItem.useTime = Main.player[Projectile.owner].HeldItem.useAnimation;
        base.OnKill(timeLeft);
    }
    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        if (Main.player[Projectile.owner].HeldItem.useTime > 10
         && Main.player[Projectile.owner].HeldItem.type == ModContent.ItemType<GammaRay>())
        {
            Main.player[Projectile.owner].HeldItem.useTime--;
            Main.player[Projectile.owner].HeldItem.useAnimation--;
        }
        if (Projectile.damage / 156f > 1.5f) target.AddBuff(ModContent.BuffType<VortexFlames>(), 800);
        else target.AddBuff(ModContent.BuffType<VortexFlames>(), 100);
    }

    public override void ModifyDamageHitbox(ref Rectangle hitbox)
    {
        // By using ModifyDamageHitbox, we can allow the flames to damage enemies in a larger area than normal without colliding with tiles.
        // Here we adjust the damage hitbox. We adjust the normal 6x6 hitbox and make it 66x66 while moving it left and up to keep it centered.
        int size = 50;
        hitbox.X -= size;
        hitbox.Y -= size;
        hitbox.Width += size * 2;
        hitbox.Height += size * 2;
    }
}
}