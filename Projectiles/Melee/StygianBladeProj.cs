using Conquest.Assets.Common;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Mono.Cecil;

namespace Conquest.Projectiles.Melee
{
    public class StygianBladeProj : ModProjectile
    {
        private float SWINGRANGE = 1f * (float)Math.PI; // The angle a swing attack covers (300 deg)
        private const float FIRSTHALFSWING = 0.5f; // How much of the swing happens before it reaches the target angle (in relation to swingRange
        private const float WINDUP = 0.15f; // How far back the player's hand goes when winding their attack (in relation to swingRange)
        private const float UNWIND = 0.5f; // When should the sword start disappearing

        private enum AttackType // Which attack is being performed
        {
            // Swings are normal sword swings that can be slightly aimed
            // Swings goes through the full cycle of animations
            Swing,
            Reverse,
            Stab,
        }

        private enum AttackStage
        {
            Prepare,
            Execute,
            Unwind
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Main.myPlayer];
            MyPlayer modPlayer = player.GetModPlayer<MyPlayer>();
            if (CurrentAttack == AttackType.Swing) {
                player.GetModPlayer<MyPlayer>().ScreenShake = 3;
            }
            if (CurrentAttack == AttackType.Stab)
            {
                player.GetModPlayer<MyPlayer>().ScreenShake = 3;
                damageDone = (int)(damageDone * 1.75f);
            } else
            {
                damageDone = (int)(damageDone * 0.85f);
            }
            target.AddBuff(BuffID.OnFire, 60);

        }
        // These properties wrap the usual ai and localAI arrays for cleaner and easier to understand code.
        private AttackType CurrentAttack
        {
            get => (AttackType)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }

        private AttackStage CurrentStage
        {
            get => (AttackStage)Projectile.localAI[0];
            set
            {
                Projectile.localAI[0] = (float)value;
                Timer = 0; // reset the timer when the projectile switches states
            }
        }
        private Vector2 secretVariable;
        private ref float InitialAngle => ref Projectile.ai[1]; // Angle aimed in (with constraints)
        private ref float Timer => ref Projectile.ai[2]; // Timer to keep track of progression of each stage
        private ref float Progress => ref Projectile.localAI[1]; // Position of sword relative to initial angle

        // We define timing functions for each stage, taking into account melee attack speed
        // Note that you can change this to suit the need of your projectile
        private Player Owner => Main.player[Projectile.owner];

        public float prepTime => 12f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
        public float execTime => 8f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
        public float unwindExecTime => 8f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
        public float stabExecTime => 6f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
        public float hideTime => 12f / Owner.GetTotalAttackSpeed(Projectile.DamageType);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 80; // Hitbox width of projectile
            Projectile.height = 80; // Hitbox height of projectile
            Projectile.friendly = true; // Projectile hits enemies
            Projectile.timeLeft = 180; // Time it takes for projectile to expire
            Projectile.penetrate = -1; // Projectile pierces infinitely
            Projectile.tileCollide = false; // Projectile does not collide with tiles
            Projectile.usesLocalNPCImmunity = true; // Uses local immunity frames
            Projectile.localNPCHitCooldown = -1; // We set this to -1 to make sure the projectile doesn't hit twice
            Projectile.ownerHitCheck = true; // Make sure the owner of the projectile has line of sight to the target (aka can't hit things through tile).
            Projectile.DamageType = DamageClass.Melee; // Projectile is a melee projectile
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.spriteDirection = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
            float targetAngle = (Main.MouseWorld - Owner.MountedCenter).ToRotation();

            if (Projectile.spriteDirection == 1)
            {
                // However, we limit the rangle of possible directions so it does not look too ridiculous
                targetAngle = MathHelper.Clamp(targetAngle, (float)-Math.PI * 1 / 3, (float)Math.PI * 1 / 6);
            }
            else
            {
                if (targetAngle < 0)
                {
                    targetAngle += 2 * (float)Math.PI; // This makes the range continuous for easier operations
                }

                targetAngle = MathHelper.Clamp(targetAngle, (float)Math.PI * 5 / 6, (float)Math.PI * 4 / 3);
            }

            
            CurrentAttack = (AttackType)((int)Projectile.ai[0]);
            if (CurrentAttack == AttackType.Swing)
            {
                InitialAngle = targetAngle - FIRSTHALFSWING * SWINGRANGE * Projectile.spriteDirection; // Otherwise, we calculate the angle
                
            }
            else if (CurrentAttack == AttackType.Reverse)
            {
                SWINGRANGE = MathHelper.Pi * 0.8f;
                InitialAngle = targetAngle - FIRSTHALFSWING * SWINGRANGE * Projectile.spriteDirection + MathHelper.ToRadians(72) * Projectile.spriteDirection; // Otherwise, we calculate the angle
                
            }
            else if (CurrentAttack == AttackType.Stab)
            {
                SWINGRANGE = 0;
                secretVariable = Owner.MountedCenter.DirectionTo(Main.MouseWorld);
                InitialAngle = Owner.Center.AngleTo(Main.MouseWorld);
                Owner.velocity.X += Owner.MountedCenter.DirectionTo(Main.MouseWorld).X * 2;
            }

        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            // Projectile.spriteDirection for this projectile is derived from the mouse position of the owner in OnSpawn, as such it needs to be synced. spriteDirection is not one of the fields automatically synced over the network. All Projectile.ai slots are used already, so we will sync it manually. 
            writer.Write((sbyte)Projectile.spriteDirection);
            writer.WriteVector2(secretVariable);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.spriteDirection = reader.ReadSByte();
            secretVariable = reader.ReadVector2();
        }
        public override void AI()
        {
            // Extend use animation until projectile is killed
            Owner.itemAnimation = 2;
            Owner.itemTime = 2;

            

            // Kill the projectile if the player dies or gets crowd controlled
            if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed)
            {
                Projectile.Kill();
                return;
            }

            // AI depends on stage and attack
            // Note that these stages are to facilitate the scaling effect at the beginning and end
            // If this is not desireable for you, feel free to simplify
            switch (CurrentStage)
            {
                case AttackStage.Prepare:
                    PrepareStrike();
                    break;
                case AttackStage.Execute:
                    ExecuteStrike();
                    break;
                default:
                    UnwindStrike();
                    break;
            }

            SetSwordPosition();
            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Calculate origin of sword (hilt) based on orientation and offset sword rotation (as sword is angled in its sprite)
            Vector2 origin;
            float rotationOffset;
            SpriteEffects effects;

            if (Projectile.spriteDirection > 0)
            {
                origin = new Vector2(0, Projectile.height);
                rotationOffset = MathHelper.ToRadians(45f);
                effects = SpriteEffects.None;
            }
            else
            {
                origin = new Vector2(Projectile.width, Projectile.height);
                rotationOffset = MathHelper.ToRadians(135f);
                effects = SpriteEffects.FlipHorizontally;
            }

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, default, lightColor * Projectile.Opacity, Projectile.rotation + rotationOffset, origin, Projectile.scale, effects, 0);

            // Since we are doing a custom draw, prevent it from normally drawing
            return false;
        }

        // Find the start and end of the sword and use a line collider to check for collision with enemies
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Owner.MountedCenter;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * ((Projectile.Size.Length()) * Projectile.scale);
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 15f * Projectile.scale, ref collisionPoint);
        }

        // Do a similar collision check for tiles
        public override void CutTiles()
        {
            Vector2 start = Owner.MountedCenter;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
            Utils.PlotTileLine(start, end, 15 * Projectile.scale, DelegateMethods.CutTiles);
        }

        // We make it so that the projectile can only do damage in its release and unwind phases
        public override bool? CanDamage()
        {
            if (CurrentStage == AttackStage.Prepare)
                return false;
            return base.CanDamage();
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // Make knockback go away from player
            modifiers.HitDirectionOverride = target.position.X > Owner.MountedCenter.X ? 1 : -1;

            // If the NPC is hit by the spin attack, increase knockback slightly
        }

        // Function to easily set projectile and arm position
        public void SetSwordPosition()
        {
            if (CurrentAttack == AttackType.Reverse || CurrentAttack == AttackType.Swing)
            {
                Projectile.rotation = InitialAngle + Projectile.spriteDirection * Progress;
            }
            else
            {
                Projectile.rotation = InitialAngle;
            }

            // Set composite arm allows you to set the rotation of the arm and stretch of the front and back arms independently
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f)); // set arm position (90 degree offset since arm starts lowered)
            Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2); // get position of hand

            armPosition.Y += Owner.gfxOffY;
            Projectile.Center = armPosition; // Set projectile to arm position
            if (CurrentAttack == AttackType.Stab)
            {
                Projectile.Center -= secretVariable * (1f - Progress) * 20;
                if (Progress >= 0.9f)
                {
                    int a = Projectile.NewProjectile(Projectile.GetSource_FromThis(), armPosition + secretVariable * 40 * Projectile.scale, secretVariable * 20 / stabExecTime * 5, ModContent.ProjectileType<StygianBladeShoot>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Main.projectile[a].rotation = InitialAngle;
                    Main.projectile[a].scale = Projectile.scale;
                    Projectile.Kill();
                }
            }
            Projectile.scale = Owner.GetAdjustedItemScale(Owner.HeldItem); // Slightly scale up the projectile and also take into account melee size modifiers

            Owner.heldProj = Projectile.whoAmI; // set held projectile to this projectile
        }

        // Function facilitating the taking out of the sword
        private void PrepareStrike()
        {
            var source = Projectile.GetSource_FromAI();
            if (CurrentAttack == AttackType.Swing)
            {
                Progress = 0; // Calculates rotation from initial angle
                SoundEngine.PlaySound(SoundID.Item1); // Play sword sound here since playing it on spawn is too early
                if (Main.rand.NextBool())
                {
                    CurrentStage = AttackStage.Execute;
                    var proj = Projectile.NewProjectileDirect(source, Owner.MountedCenter, new Vector2(Owner.direction, 0f), ModContent.ProjectileType<SBS>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI, Owner.direction * Owner.gravDir, execTime + hideTime, Projectile.scale);
                    if (proj.ModProjectile is SBS modProj)
                    {
                        modProj.startRotation = InitialAngle;
                    }
                }
            }
            if (CurrentAttack == AttackType.Reverse)
            {
                Progress = 1f;
                SoundEngine.PlaySound(SoundID.Item1); // Play sword sound here since playing it on spawn is too early
                if (Main.rand.NextBool())
                {
                    CurrentStage = AttackStage.Execute; // If attack is over prep time, we go to next stage
                    var proj = Projectile.NewProjectileDirect(source, Owner.MountedCenter, new Vector2(Owner.direction, 0f), ModContent.ProjectileType<SBS>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI, Owner.direction * Owner.gravDir, unwindExecTime + hideTime, Projectile.scale);
                    if (proj.ModProjectile is SBS modProj)
                    {
                        modProj.startRotation = InitialAngle;
                        modProj.reverseRot = true;
                    }
                }
            }
            if (CurrentAttack == AttackType.Stab)
            {
                Progress = 0f;
                SoundEngine.PlaySound(SoundID.Item1); // Play sword sound here since playing it on spawn is too early
                if (Main.rand.NextBool())
                    CurrentStage = AttackStage.Execute; // If attack is over prep time, we go to next stage
            }
        }

        // Function facilitating the first half of the swing
        private void ExecuteStrike()
        {
            
            if (CurrentAttack == AttackType.Swing)
            {
                Progress = MathHelper.SmoothStep(0, SWINGRANGE, (1f - UNWIND) * Timer / execTime);

                

                if (Timer >= execTime)
                {
                    CurrentStage = AttackStage.Unwind;
                }
            }
            else if (CurrentAttack == AttackType.Reverse)
            {
                Progress = 1f - MathHelper.SmoothStep(0, SWINGRANGE, (UNWIND) * Timer / unwindExecTime);
                if (Timer >= unwindExecTime)
                {
                    CurrentStage = AttackStage.Unwind;
                }
            }
            else if (CurrentAttack == AttackType.Stab)
            {
                Progress = MathHelper.SmoothStep(0, 1, Timer / stabExecTime);

                if (Timer >= stabExecTime)
                {
                    CurrentStage = AttackStage.Unwind;
                }
            }
        }

        // Function facilitating the latter half of the swing where the sword disappears
        private void UnwindStrike()
        {
            if (CurrentAttack == AttackType.Swing)
            {
                Progress = MathHelper.SmoothStep(0, SWINGRANGE, (1f - UNWIND) + UNWIND * Timer / hideTime);

                if (Timer >= hideTime)
                {
                    Projectile.Kill();

                }
            }
            else if (CurrentAttack == AttackType.Reverse)
            {
                Progress = 1f - MathHelper.SmoothStep(0, SWINGRANGE, (1f - UNWIND) + UNWIND * Timer / hideTime);

                if (Timer >= hideTime)
                {
                    Projectile.Kill();
                }

            }
            else if (CurrentAttack == AttackType.Stab)
            {
                Progress = 1;

                if (Timer >= hideTime)
                {
                    Projectile.Kill();
                }
            }
        }
        public override void Kill(int timeLeft)
        {
            base.Kill(timeLeft);
        }
    }
    public class StygianBladeShoot : ModProjectile
    {
        public override string Texture => "Conquest/Projectiles/Melee/StygianBladeProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50;    //The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 80; // Hitbox width of projectile
            Projectile.height = 80; // Hitbox height of projectile
            Projectile.friendly = true; // Projectile hits enemies
            Projectile.timeLeft = 100; // Time it takes for projectile to expire
            Projectile.penetrate = -1; // Projectile pierces infinitely
            Projectile.tileCollide = false; // Projectile does not collide with tiles
            Projectile.usesLocalNPCImmunity = true; // Uses local immunity frames
            Projectile.localNPCHitCooldown = -1; // We set this to -1 to make sure the projectile doesn't hit twice
            Projectile.DamageType = DamageClass.Melee; // Projectile is a melee projectile
            Projectile.light = 0.5f;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            base.AI();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Main.myPlayer];
            MyPlayer modPlayer = player.GetModPlayer<MyPlayer>();

            target.AddBuff(BuffID.OnFire, 60);
            Projectile.damage = (int)(Projectile.damage * 0.9);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            default(Effects.FireTrail).Draw(Projectile);

            return true;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 30; i++)
            {
                Dust.NewDust(Projectile.Center, 80, 80, DustID.Torch, Projectile.oldVelocity.X/3, Projectile.oldVelocity.Y/3);
            }
            base.Kill(timeLeft);
        }
    }
}
