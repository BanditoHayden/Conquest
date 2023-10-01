using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Conquest.Assets.Common;

namespace Conquest.Projectiles.Summoner
{
    internal class EmeraldWall : ModProjectile
    {
        public bool imploding = false;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 50;
        }

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;
        }

        private int timer = 0;
        private int killTimer = 0;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            player.GetModPlayer<MyPlayer>().emerald = true;

            timer++;

            Vector2 offset = new Vector2(100, 0).RotatedBy(MathHelper.ToRadians(5 * timer));

            Projectile.rotation = Projectile.velocity.ToRotation();
            
            if (timer >= 300 || player.GetModPlayer<MyPlayer>().emeraldBoom && !imploding)
            {
                imploding = true;
                SoundEngine.PlaySound(SoundID.Item8, Projectile.position);
            }

            if (imploding)
            {
                ImplodingBehaviour();
            }
            else
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(player.Center + offset) * 18, 0.33f);
            }
        }

        public void ImplodingBehaviour()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(player.Center) * 18, 0.11f);
            if (Projectile.Distance(player.Center) < 15)
            {
                Projectile.Kill();
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (timer > 5)
            {
                Player player = Main.player[Projectile.owner];
                NPC victim = FindClosestNPC();
                if (victim != null)
                {
                    SoundEngine.PlaySound(SoundID.Shatter);
                    for (int i = 0; i < 7; i++)
                    {
                        Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(Main.rand.Next(-20, 20), Main.rand.Next(-20, 20)), ModContent.ProjectileType<EmeraldWallSharp>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        p.ai[1] = victim.whoAmI;
                    }
                    player.GetModPlayer<MyPlayer>().emerald = false;
                    player.GetModPlayer<MyPlayer>().emeraldCD += 40 * (4 - player.ownedProjectileCounts[Type]);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            default(Effects.EmeraldTrailSmall).Draw(Projectile);
            return false;
        }

        public NPC? FindClosestNPC(float maxDetectDistance = 675)
        {
            NPC closestNPC = null;

            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs(max always 200)
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];

                if (target.CanBeChasedBy())
                {
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }
    }
}
