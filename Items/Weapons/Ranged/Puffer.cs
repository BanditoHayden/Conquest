using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.DataStructures;
using Terraria.Audio;
using Conquest.Assets.Common;
using Conquest.Projectiles.Ranged;
using System;
using System.Collections.Generic;

namespace Conquest.Items.Weapons.Ranged
{
    public class Puffer : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("AK-47u");
            Item.ResearchUnlockCount = 1;
        }

        SoundStyle Reload = new SoundStyle($"{nameof(Conquest)}/Assets/Sounds/AKReload")
        {
            Volume = 0.9f,
            PitchVariance = 0.1f,
            MaxInstances = 3,
        };

        float pufferCharge = 1f;
        float pufferChargeMax = 30f;
        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 24;
            Item.height = 19;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 25);
            Item.noMelee = false;
            // Use Properties
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = false;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;

            // Weapon Properties
            Item.damage = 12;
            Item.knockBack = 4f;
            Item.DamageType = DamageClass.Ranged;
            // Projectile Properties
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 20f;
            Item.useAmmo = AmmoID.Bullet;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3, 0);
        }

        public bool NearbyFloat(List<float> list, float value, float threshold = 2 * 3.14159f / 180)
        {
            foreach (float elem in list)
            {
                if (MathF.Abs(elem - value) < threshold) return true;
            }
            return false;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            List<float> rotations = new List<float>();
            rotations.Add(0);
            for (int i = 0; i < 6; i++)
            {
                float testValue = MathHelper.ToRadians(Main.rand.NextFloat(-20f, 20f));
                while (NearbyFloat(rotations, testValue))
                {
                    testValue = MathHelper.ToRadians(Main.rand.NextFloat(-20f, 20f));
                }
                rotations.Add(testValue);
                Projectile.NewProjectileDirect(player.GetSource_ItemUse_WithPotentialAmmo(Item, Item.useAmmo),
                    position, velocity.RotatedBy(testValue),
                    type, damage, knockback, player.whoAmI);
            }
            player.velocity += velocity * -1.5f;
        }
    }
}
