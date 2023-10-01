using Conquest.Assets.GUI;
using Conquest.Buffs;
using Conquest.Items.Weapons.Ranged;
using Conquest.Projectiles.Magic;
using Conquest.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Conquest.Assets.Common
{
    internal class MyProjectile : GlobalProjectile
    {
        public static float killsCount;
        public bool manaOnKill = false;
        public bool bombOnKill = false;
        public override bool InstancePerEntity => true;
        public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
        {
            if (T1.p6On == true && projectile.DamageType == DamageClass.Magic)
            {
                projectile.GetGlobalProjectile<MyProjectile>().bombOnKill = true;
            }
            return base.OnTileCollide(projectile, oldVelocity);
        }
        public override void AI(Projectile projectile)
        {
            if (T1.p3On == true && projectile.DamageType == DamageClass.Magic)
            {
                projectile.GetGlobalProjectile<MyProjectile>().manaOnKill = true;
            }
        }
        
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            Player player = Main.player[projectile.owner];
            if (T3.p27On == true && projectile.DamageType == DamageClass.Ranged && projectile.type != ProjectileID.MolotovCocktail && projectile.type != ProjectileID.MolotovFire && projectile.type != ProjectileID.MolotovFire2 && projectile.type != ProjectileID.MolotovFire3)
            {
                if (Main.rand.NextFloat() >= 0.9f)
                {
                    if (Main.rand.NextFloat() >= 0.67)
                    {
                        Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, projectile.velocity, ProjectileID.MolotovCocktail, projectile.damage / 10, 0, player.whoAmI, 0, 0);
                    }
                    else if (Main.rand.NextFloat() >= 0.50)
                    {
                        Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, projectile.velocity, ModContent.ProjectileType<Fireinthehole>(), projectile.damage / 4, 0, player.whoAmI, 0, 0);
                    }
                    else
                    {
                        Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, projectile.velocity, ModContent.ProjectileType<Flashbang>(), 1, 0, player.whoAmI, 0, 0);
                    }
                }
            }
        }
        public override void OnKill(Projectile projectile, int timeLeft)
        {
            Player player = Main.player[projectile.owner];
            if (projectile.GetGlobalProjectile<MyProjectile>().manaOnKill == true)
            {
                if (Main.rand.NextBool(10))
                {
                    player.statMana += (int)(projectile.damage / 20 * player.manaCost);
                }
            }
            if (projectile.GetGlobalProjectile<MyProjectile>().bombOnKill == true)
            {
                Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.position, projectile.velocity * 0, ModContent.ProjectileType<ManaBomb>(), projectile.damage / 2, 0, projectile.owner, 0, 0);

            }
        }
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Main.myPlayer];
            var entitySource = Projectile.GetSource_None();

            MyPlayer modPlayer = player.GetModPlayer<MyPlayer>();
            if (player.HeldItem.type == ModContent.ItemType<Breakneck>())
            {
                if (!target.active)
                {
                    killsCount += 1;
                    Breakneck.fireRateTimer = 0;
                }
            }
            if (modPlayer.Polyute && projectile.friendly && Main.rand.NextBool(10))
            {
                Projectile.NewProjectile(entitySource, target.Center.X, target.Center.Y - 100, Main.rand.Next(20, 21) * .25f, Main.rand.Next(20, 21) * .25f, ProjectileID.ChlorophyteBullet, player.HeldItem.damage, 0f, projectile.owner);

            }
            if (modPlayer.ElectroCrystal && projectile.friendly && Main.rand.NextBool(10))
            {
                target.AddBuff(ModContent.BuffType<Electrified2>(), 120);
            }
        }
    }
}
