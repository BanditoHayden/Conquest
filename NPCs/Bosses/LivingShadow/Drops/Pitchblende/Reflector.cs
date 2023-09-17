using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace Conquest.NPCs.Bosses.LivingShadow.Drops.Pitchblende;

public class Reflector : ModItem
{
    public override void SetStaticDefaults()
    {
        // DisplayName.SetDefault("Reflector");
        // Tooltip.SetDefault("Fires homing boomerangs" +
        //     "\nRight-clicking on an enemy will teleport you behind it and release a burst of boomerangs" +
        //     "\nBoomerangs pierce through enemy defence");
    }

    public override void SetDefaults()
    {
        Item.width = 19;
        Item.height = 19;
        Item.rare = ModContent.RarityType<Assets.Common.ArtifactRarity>();

        Item.damage = 193;
        Item.DamageType = DamageClass.Melee;
        Item.knockBack = 5;

        Item.useStyle = ItemUseStyleID.Swing;
        Item.noUseGraphic = true;
        Item.noMelee = true;

        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.autoReuse = true;

        Item.UseSound = SoundID.Item1;
        Item.shoot = ModContent.ProjectileType<ReflectorRang>();
        Item.shootSpeed = 66;

        Item.value = Item.sellPrice(platinum: 0, gold: 25);
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White;
    }

    public override bool AltFunctionUse(Player player)
    {
        return true;
    }

    public override void Update(ref float gravity, ref float maxFallSpeed)
    {
        gravity = 0.5f;
        maxFallSpeed = 0;
        Lighting.AddLight(Item.position, TorchID.White);
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        bool found = false;
        if (!player.immune && player.altFunctionUse == 2)
        {
            Dust.NewDustDirect(Main.MouseWorld, 5, 5, DustID.SilverFlame, Scale: 6f).noGravity = true;
            Dust.NewDustDirect(Main.MouseWorld, 5, 5, DustID.Wraith, Scale: 3f).noGravity = true;
            foreach (NPC npc in Main.npc)
            {
                if (npc.Center.Distance(Main.MouseWorld) < MathF.Max(40, npc.width))
                {
                    found = true;
                    for (int i = -64; i < 0; i += 8)
                    {
                        Vector2 toNPC = player.DirectionTo(npc.Center) * i;
                        toNPC = toNPC.RotatedBy(Main.rand.NextFloat(-MathF.PI / 16, MathF.PI / 16));
                        Projectile.NewProjectile(source, position, toNPC, type, damage, knockback, player.whoAmI);
                    }

                    Vector2 toPos = (player.DirectionTo(npc.Center) * player.Distance(npc.Center)) +
                                    (player.DirectionTo(npc.Center) * MathF.Max(100, npc.width * 0.85f));
                    bool inWall = false;
                    while (!Collision.CanHitLine(npc.Center, 1, 1, player.position + toPos, 1, 1))
                    {
                        toPos.Y--;
                        inWall = true;
                    }
                    if (inWall) toPos.Y -= 16;
                    player.Teleport(player.position + toPos, TeleportationStyleID.QueenSlimeHook);
                    player.direction *= -1;
                    player.AddImmuneTime(ImmunityCooldownID.General, 60);
                    break;
                }
            }
        }
        if (!found && player.altFunctionUse != 2)
        {
            Vector2 vel = velocity.RotatedBy(Main.rand.NextFloat(-MathF.PI / 7, MathF.PI / 7));
            vel.Normalize();
            vel *= 88;
            Projectile.NewProjectile(source, position, vel * Main.rand.NextFloat(0.85f, 1.15f), type, damage, knockback, player.whoAmI);
            for (int i = 0; i < 4; i++)
            {
                Vector2 v = velocity.RotatedBy(Main.rand.NextFloat(-MathF.PI / 7, MathF.PI / 7));
                v.Normalize();
                v *= 88;
                Projectile.NewProjectile(source, position, v * Main.rand.NextFloat(0.85f, 1.15f), type, damage, knockback, player.whoAmI);
            }
        }
        return false;
    }
}

