using System;
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
using Microsoft.CodeAnalysis;

namespace Conquest.Items.Weapons.Magic
{
    public class Staff : ModItem
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
            Item.width = 36;
            Item.height = 36;
            Item.value = Item.sellPrice(silver: 1);
            Item.noMelee = true;
            Item.rare = 6;
            // Use Properties
            Item.useTime = 15;
            Item.useAnimation = 15;
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
            Item.shoot = ModContent.ProjectileType<StaffProjectile>();
            Item.shootSpeed = 12f;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 55f;

            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        }
    }
    public class StaffProjectile : ModProjectile
    {
        private ref float timer => ref Projectile.ai[0];
        private ref float otherTimer => ref Projectile.ai[1];
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.AmethystBolt}";
        public override void SetDefaults()
        {
            Projectile.ignoreWater = true;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.aiStyle = -1;
            Projectile.Opacity = 0;
            Projectile.timeLeft = 360;
            base.SetDefaults();
        }
        public override void AI()
        {
            timer++;
            ++otherTimer;
            if (otherTimer >= 20)
            {
                otherTimer = 1;
            }
            var pos1 = Projectile.Center + Vector2.One.RotatedBy(timer / 10) * 10;
            var pos2 = Projectile.Center + Vector2.One.RotatedBy(timer / 10 + MathHelper.Pi) * 10;
            Dust a = Dust.NewDustPerfect(pos1, DustID.GemSapphire, Vector2.Zero, (int)(255 - timer * 17), Color.LightBlue);
            Dust b = Dust.NewDustPerfect(pos2, DustID.GemSapphire, Vector2.Zero, (int)(255 - timer * 17), Color.LightBlue);
            a.noGravity = true;
            b.noGravity = true;


            //i was gonna make a lurch effect on this but couldnt get it to look good so i didnt
            base.AI();
        }
    }
}
