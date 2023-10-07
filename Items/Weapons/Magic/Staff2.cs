﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Conquest.Projectiles.Magic;
using Conquest.Assets.Common;

namespace Conquest.Items.Weapons.Magic
{
    public class Staff2 : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            // Tooltip.SetDefault("Projectiles fire at your cursor, Critical hits heal you");
            Item.staff[Item.type] = true;
        }
        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 40;
            Item.height = 40;
            Item.value = Item.sellPrice(silver: 1);
            Item.noMelee = true;
            Item.rare = 6;
            // Use Properties
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = false;
            Item.UseSound = SoundID.Item43;
            Item.autoReuse = true;
            // Weapon Properties
            Item.damage = 30;
            Item.knockBack = 3f;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 9;
            // Projectile Properties
            Item.shoot = ModContent.ProjectileType<Staff2Projectile>();
            Item.shootSpeed = 12f;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 60f;

            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
            float rot = MathHelper.PiOver4 / 4;
            int num = 4;

            for (float i = 0; i < num; i++)
            {
                Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.Lerp(-rot, rot, i / (float)(num - 1))); // Watch out for dividing by 0 if there is only 1 projectile.
                int a = Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI, 0, 1);
            }
            return false;

        }
    }
    public class Staff2Projectile : ModProjectile
    {
        private ref float timer => ref Projectile.ai[0];
        private ref float MergeScale => ref Projectile.ai[1];
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.AmethystBolt}";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50;    //The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.ignoreWater = true;
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.penetrate = 5;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.aiStyle = -1;
            Projectile.Opacity = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.timeLeft = 420;
            Projectile.scale = 1;
            base.SetDefaults();
        }
        float gravity = 0.2f;

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale = MergeScale;
            Projectile.Resize((int)(Projectile.width * MergeScale), (int)(Projectile.height * MergeScale));
            Projectile.damage = (int)(Projectile.damage * MergeScale);
            base.OnSpawn(source);
        }
        public override void AI()
        {
            Projectile.scale = MergeScale;
            timer++;
            Dust a = Dust.NewDustPerfect(Projectile.Center, DustID.GemAmethyst, Vector2.Zero, MergeScale == 1 ? (int)(255 - timer * 17) : 0, default, Projectile.scale);
            a.noGravity = true;

            Projectile.velocity.Y += gravity;

            if (timer > 30 && timer < 400) //Bug?
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (i != Projectile.whoAmI)
                    {
                        Projectile other = Main.projectile[i];
                        if (other.Hitbox.Intersects(Projectile.Hitbox) && other.type == Type && other.ai[0] > 30 && other.ai[0] < 400 && Projectile.scale + other.scale < 5)
                        {

                            int b = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, Type, Projectile.damage, Projectile.knockBack, Projectile.owner, 0, Projectile.scale + other.scale);
                            //Main.projectile[b].timeLeft = (int)(Projectile.timeLeft + Main.projectile[b].ai[1] * 30);
                            Projectile.Kill();
                            other.Kill();
                        }
                    }
                }
                base.AI();
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            return Projectile.penetrate <= 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
