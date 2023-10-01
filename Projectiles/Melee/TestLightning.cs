using Conquest.Assets.Common;
using Conquest.Dusts;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using static Humanizer.In;


namespace Conquest.Projectiles.Melee
{
    public class TestLightning : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 60;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.scale = 0.7f;
            Projectile.penetrate = -1;
            Projectile.Resize(80, 50);

        }
        public Color drawColor = Color.Yellow;
       
        public override Color? GetAlpha(Color lightColor)
        {
            return drawColor;
        }
      
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] == 0f)
            {
              LightningStrike(target.whoAmI, target.Center, damageDone);
                Projectile.Kill();
               // MoonlordDeathDrama.RequestLight(((Projectile.timeLeft + 920) - 480f) / 120f, Projectile.Center);
            }
        }
        public override void OnKill(int timeLeft)
        {
            Player projOwner = Main.player[Projectile.owner];

            for (int i = 0; i < 50; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LightningDust>(), speed * 5, Scale: 1.5f);
                d.noGravity = true;
            }
        }
        int timer;
        public override void AI()
        {
            int damage = Projectile.damage;
            Projectile.rotation = Projectile.velocity.ToRotation();
            timer++;
            //MoonlordDeathDrama.RequestLight(((Projectile.timeLeft + 920) - 480f) / 120f, Projectile.Center);

            if (timer == 30)
            {
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.ai[0] == 0f)
            {
                LightningStrike(-1, Projectile.Center, Projectile.damage);
            }

            return base.OnTileCollide(oldVelocity);
        }












        // Credit to StormyTuna
        private void LightningStrike(int whoAmIToIgnore, Vector2 startPos, int damage)
        {
            List<NPC> closeNPCs = new();
            if (whoAmIToIgnore == -1)
            {
                closeNPCs = Helpers.GetNearbyEnemies(startPos, 16f * 16f, true, false);
            }
            else
            {
                closeNPCs = Helpers.GetNearbyEnemies(startPos, 16f * 16f, true, false, new List<int>() { whoAmIToIgnore });
            }

            int numLightning = (int)MathHelper.Clamp(closeNPCs.Count, 0f, 3f);
            for (int i = 0; i < numLightning; i++)
            {
                Main.LocalPlayer.ApplyDamageToNPC(closeNPCs[i], damage / 3, 0f, 0, false);
                LightningHelper.MakeDust(startPos, closeNPCs[i].Center);
            }

        }
        public class LightningHelper
        {
            public static void MakeDust(Vector2 source, Vector2 dest)
            {
                var dustPoints = CreateBolt(source, dest);
                foreach (var point in dustPoints)
                {
                    Dust d = Dust.NewDustPerfect(point, ModContent.DustType<LightningDust>(), Scale: 0.8f);
                    d.noGravity = true;
                    d.velocity = Vector2.Zero;
                }
            }

            public static List<Vector2> CreateBolt(Vector2 source, Vector2 dest)
            {
                var results = new List<Vector2>();
                Vector2 tangent = dest - source;
                Vector2 normal = Vector2.Normalize(new Vector2(tangent.Y, -tangent.X));
                float length = tangent.Length();

                List<float> positions = new List<float>();
                positions.Add(0);

                for (int i = 0; i < length; i++)
                    positions.Add(Main.rand.NextFloat(0f, 1f));

                positions.Sort();

                const float Sway = 1000;
                const float Jaggedness = 1 / Sway;

                Vector2 prevPoint = source;
                float prevDisplacement = 0f;
                for (int i = 1; i < positions.Count; i++)
                {
                    float pos = positions[i];

                    // used to prevent sharp angles by ensuring very close positions also have small perpendicular variation.
                    float scale = (length * Jaggedness) * (pos - positions[i - 1]);

                    // defines an envelope. Points near the middle of the bolt can be further from the central line.
                    float envelope = pos > 0.95f ? 20 * (1 - pos) : 1;

                    float displacement = Main.rand.NextFloat(-Sway, Sway);
                    displacement -= (displacement - prevDisplacement) * (1 - scale);
                    displacement *= envelope;

                    Vector2 point = source + pos * tangent + displacement * normal;
                    results.Add(point);
                    prevPoint = point;
                    prevDisplacement = displacement;
                }

                results.Add(prevPoint);

                return results;
            }



            




        }
    }
}
