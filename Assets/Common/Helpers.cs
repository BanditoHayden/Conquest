using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Conquest.Assets.Common
{
    public static class Helpers
    {
        public static void PlaceTile(int x, int y, int TileType, int style = 0, int direction = -1)
        {
            WorldGen.PlaceObject(x, y, TileType, true, style, 0, -1, direction);
        }
        public static MyPlayer GetWorldPlayer(this Player player) => player.GetModPlayer<MyPlayer>();
        // All credit goes to StormyTuna
        /// <summary>Gets the closest hostile NPC within the range of that position</summary>
        /// <param name="position">The position, should be the center of the search and usually the center of another entity</param>
        /// <param name="range">The range measured in units, 1 tile is 16 units</param>
        /// <param name="careAboutLineOfSight">Whether the function should check Collision.CanHit</param>
        /// <param name="careAboutCanBeChased">Whether the function should check npc.chaseable</param>
        /// <param name="excludedNPCs">The whoAmI fields of any NPCs that are excluded from the search</param>
        /// <returns>Returns the closest NPC. Returns null if no NPC is found</returns>
        /// 
        public static void TurnToExplosion(this Projectile proj, int width, int height)
        {
            proj.velocity = new Vector2(0f, 0f);
            proj.timeLeft = 3;
            proj.penetrate = -1;
            proj.tileCollide = false;
            proj.alpha = 255;
            proj.Resize(width, height);
            proj.netUpdate = true;
        }
        public static NPC GetClosestEnemy(Vector2 position, float range, bool careAboutLineOfSight, bool careAboutCanBeChased, List<int> excludedNPCs = null)
        {
            NPC closestNPC = null;
            float rangeSquared = range * range;
            if (excludedNPCs == null)
                excludedNPCs = new List<int>();

            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];

                if (!npc.active || npc.CountsAsACritter || npc.friendly || npc.immortal || excludedNPCs.Contains(npc.whoAmI))
                {
                    continue;
                }

                float distanceSquared = Vector2.DistanceSquared(position, npc.Center);
                bool canSee = careAboutLineOfSight ? Collision.CanHit(position, 1, 1, npc.position, npc.width, npc.height) : true;
                bool canBeChased = careAboutCanBeChased ? npc.chaseable : true;
                if (distanceSquared < rangeSquared && canSee && canBeChased)
                {
                    closestNPC = npc;
                    rangeSquared = distanceSquared;
                }
            }

            return closestNPC;
        }
        /// <summary>Gets a list of NPCs within the range of that position</summary>
        /// <param name="position">The position, should be the center of the search and usually the center of another entity</param>
        /// <param name="range">The range measured in units, 1 tile is 16 units</param>
        /// <param name="careAboutLineOfSight">Whether the function should check Collision.CanHit</param>
        /// <param name="careAboutCanBeChased">Whether the function should check npc.chaseable</param>
        /// <param name="excludedNPCs">The whoAmI fields of any NPCs that are excluded from the search</param>
        /// <returns>A list of NPCs within range of the position</returns>
        public static List<NPC> GetNearbyEnemies(Vector2 position, float range, bool careAboutLineOfSight, bool careAboutCanBeChased, List<int> excludedNPCs = null)
        {
            List<NPC> npcs = new List<NPC>();
            float rangeSquared = range * range;
            if (excludedNPCs == null)
                excludedNPCs = new List<int>();

            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];

                if (!npc.active || npc.CountsAsACritter || npc.friendly || npc.immortal || excludedNPCs.Contains(npc.whoAmI))
                {
                    continue;
                }

                float distanceSquared = Vector2.DistanceSquared(position, npc.Center);
                bool canSee = careAboutLineOfSight ? Collision.CanHit(position, 1, 1, npc.position, npc.width, npc.height) : true;
                bool canBeChased = careAboutCanBeChased ? npc.chaseable : true;
                if (distanceSquared <= rangeSquared && canSee && canBeChased)
                {
                    npcs.Add(npc);
                }
            }

            return npcs;
        }
        public abstract class Boomerang : ModProjectile
        {
            /// <summary>How quickly the boomerang will return to its owner. Default = 10f</summary>
            public float ReturnSpeed { get; set; } = 9f;

            /// <summary>How strong the boomerang will home in on its owner when returning. Default = 4f</summary>
            public float HomingOnOwnerStrength { get; set; } = 0.4f;

            /// <summary>How many frames the boomerang will travel away from the player for. Default = 30</summary>
            public int TravelOutFrames { get; set; } = 30;

            /// <summary>How many radians the boomerang will rotate per frame. Default = 0.4f</summary>
            public float Rotation { get; set; } = 0.4f;

            /// <summary>Whether or not the boomerang will display rotation. Default = true</summary>
            public bool DoRotation { get; set; } = true;

            /// <summary>Whether or not the boomerang will turn around when it reaches its max TravelOutFrames. Default = true</summary>
            public bool DoTurn { get; set; } = true;

            /// <summary>Whether or not the boomerang will bounce off of an enemy on hit. Default = true</summary>
            public bool BounceOnHit { get; set; } = true;

            public Player Owner { get => Main.player[Projectile.owner]; }

            public virtual void OnReachedApex() { }

            public override void AI()
            {
                // Funky sound
                if (Projectile.soundDelay == 0)
                {
                    Projectile.soundDelay = 8;
                    SoundEngine.PlaySound(SoundID.Item7, Projectile.position);
                }

                // Spinny :D
                if (DoRotation) Projectile.rotation += Rotation * Projectile.direction;

                // AI state 1 - travelling away from player 
                if (Projectile.ai[0] == 0f)
                {
                    // Increase our frame counter
                    Projectile.ai[1] += 1f;
                    // Check if our frame counter is high enough to turn around
                    if (Projectile.ai[1] >= (float)TravelOutFrames && DoTurn)
                    {
                        Projectile.ai[0] = 1f;
                        Projectile.ai[1] = 0f;
                        Projectile.netUpdate = true;
                        OnReachedApex();
                    }
                }
                else if (Projectile.ai[0] == 1f)
                {
                    // AI state 2 - travelling back to player
                    // Should travel through tiles
                    Projectile.tileCollide = false;

                    // See if our projectile is too far away
                    float xDif = Owner.Center.X - Projectile.Center.X;
                    float yDif = Owner.Center.Y - Projectile.Center.Y;
                    float dist = MathF.Sqrt(xDif * xDif + yDif * yDif);
                    if (dist > 3000f)
                    {
                        Projectile.Kill();
                    }

                    // Get our x and y velocity values
                    float mult = ReturnSpeed / dist;
                    float xVel = xDif * mult;
                    float yVel = yDif * mult;

                    // Increase or decrease our X velocity accordingly 
                    if (Projectile.velocity.X < xVel)
                    {
                        Projectile.velocity.X += HomingOnOwnerStrength;
                        if (Projectile.velocity.X < 0f && xVel > 0f)
                        {
                            Projectile.velocity.X += HomingOnOwnerStrength;
                        }
                    }
                    else if (Projectile.velocity.X > xVel)
                    {
                        Projectile.velocity.X -= HomingOnOwnerStrength;
                        if (Projectile.velocity.X > 0f && xVel < 0f)
                        {
                            Projectile.velocity.X -= HomingOnOwnerStrength;
                        }
                    }

                    // Same for our Y velocity
                    if (Projectile.velocity.Y < yVel)
                    {
                        Projectile.velocity.Y += HomingOnOwnerStrength;
                        if (Projectile.velocity.Y < 0f && yVel > 0f)
                        {
                            Projectile.velocity.Y += HomingOnOwnerStrength;
                        }
                    }
                    else if (Projectile.velocity.Y > yVel)
                    {
                        Projectile.velocity.Y -= HomingOnOwnerStrength;
                        if (Projectile.velocity.Y > 0f && yVel < 0f)
                        {
                            Projectile.velocity.Y -= HomingOnOwnerStrength;
                        }
                    }

                    // Catch our projectile
                    if (Main.myPlayer == Projectile.owner)
                    {
                        if (Projectile.getRect().Intersects(Main.player[Projectile.owner].getRect()))
                        {
                            Projectile.Kill();
                        }
                    }
                }
            }


            public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
            {
                if (Projectile.ai[0] == 0f)
                {
                    Projectile.ai[0] = 1f;
                    Projectile.ai[1] = 0f;
                    Projectile.netUpdate = true;
                    if (BounceOnHit) Projectile.velocity = -Projectile.velocity;
                }
            }
            public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
            {
                width = 20;
                height = 20;

                return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
            }

            public override bool OnTileCollide(Vector2 oldVelocity)
            {
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);

                if (Projectile.ai[0] == 0f)
                {
                    Projectile.ai[0] = 1f;
                    Projectile.ai[1] = 0f;
                    Projectile.netUpdate = true;
                    Projectile.velocity = -Projectile.velocity;
                }

                return false;
            }




        }
    }
}

