using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
using static Terraria.ModLoader.ModContent;
using Conquest.Items;

namespace Conquest.NPCs.Bosses.LivingShadow;

// The main part of the boss, usually refered to as "body"
[AutoloadBossHead] // This attribute looks for a texture called "ClassName_Head_Boss" and automatically registers it as the NPC boss head icon

public class LivingShadow : ModNPC
{
    // This boss has a second phase and we want to give it a second boss head icon, this variable keeps track of the registered texture from Load().
    // It is applied in the BossHeadSlot hook when the boss is in its second stage
    public static int secondStageHeadSlot = -1;

    // This code here is called a property: It acts like a variable, but can modify other things. In this case it uses the NPC.ai[] array that has four entries.
    // We use properties because it makes code more readable ("if (SecondStage)" vs "if (NPC.ai[0] == 1f)").
    // We use NPC.ai[] because in combination with NPC.netUpdate we can make it multiplayer compatible. Otherwise (making our own fields) we would have to write extra code to make it work (not covered here)
    public bool SecondStage
    {
        get => NPC.ai[0] == 1f;
        set => NPC.ai[0] = value ? 1f : 0f;
    }
    // If your boss has more than two stages, and since this is a boolean and can only be two things (true, false), concider using an integer or enum

    float angle;
    float distance = 0;
    float targetAngle;
    bool underHalf;
    bool attemptedButchered;
    int lastHealth;

    bool hasBecomeDaytime = false;

    // More advanced usage of a property, used to wrap around to floats to act as a Vector2
    public Vector2 FirstStageDestination
    {
        get => new Vector2(NPC.ai[1], NPC.ai[2]);
        set
        {
            NPC.ai[1] = value.X;
            NPC.ai[2] = value.Y;
        }
    }

    // Auto-implemented property, acts exactly like a variable by using a hidden backing field
    public Vector2 LastFirstStageDestination { get; set; } = Vector2.Zero;

    // This property uses NPC.localAI[] instead which doesn't get synced, but because SpawnedMinions is only used on spawn as a flag, this will get set by all parties to true.
    // Knowing what side (client, server, all) is in charge of a variable is important as NPC.ai[] only has four entries, so choose wisely which things you need synced and not synced
    public bool SpawnedMinions
    {
        get => NPC.localAI[0] == 1f;
        set => NPC.localAI[0] = value ? 1f : 0f;
    }

    private int FirstStageTimerMax = 240;
    // This is a reference property. It lets us write FirstStageTimer as if it's NPC.localAI[1], essentially giving it our own name
    public ref float FirstStageTimer => ref NPC.localAI[1];

    public ref float RemainingShields => ref NPC.localAI[2];

    // We could also repurpose FirstStageTimer since it's unused in the second stage, or write "=> ref FirstStageTimer", but then we have to reset the timer when the state switch happens
    public ref float SecondStageTimer_SpawnEyes => ref NPC.localAI[3];

    // Do NOT try to use NPC.ai[4]/NPC.localAI[4] or higher indexes, it only accepts 0, 1, 2 and 3!
    // If you choose to go the route of "wrapping properties" for NPC.ai[], make sure they don't overlap (two properties using the same variable in different ways), and that you don't accidently use NPC.ai[] directly

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 4;

        // Add this in for bosses that have a summon item, requires corresponding code in the item (See MinionBossSummonItem.cs)
        NPCID.Sets.MPAllowedEnemies[Type] = true;
        // Automatically group with other bosses
        NPCID.Sets.BossBestiaryPriority.Add(Type);

        // Specify the debuffs it is immune to
        NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
        {
            SpecificallyImmuneTo = new int[] {
                    BuffID.Confused
				}
        };
    //    NPCID.Sets.SpecificDebuffImmunity.Add(Type, debuffData);

        // Influences how the NPC looks in the Bestiary
        NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
        {
            CustomTexturePath = "Conquest/Assets/Textures/Bestiary/LivingShadow_Portrait",
            PortraitScale = 0.6f, // Portrait refers to the full picture when clicking on the icon in the bestiary
            PortraitPositionYOverride = 0f,
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
    }

    public override void SetDefaults()
    {
        NPC.width = 18;
        NPC.height = 21;
        NPC.damage = 67;
        NPC.defense = 140;
        NPC.lifeMax = 50000;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.PlayerKilled;
        NPC.knockBackResist = 0f;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.value = Item.buyPrice(gold: 77);
        NPC.SpawnWithHigherTime(30);
        NPC.boss = true;
        NPC.buffImmune[BuffID.Confused] = true;
        NPC.npcSlots = 20f; // Take up open spawn slots, preventing random NPCs from spawning during the fight
        lastHealth = NPC.lifeMax;

        // Don't set immunities like this as of 1.4:
        // NPC.buffImmune[BuffID.Confused] = true;
        // immunities are handled via dictionaries through NPCID.Sets.DebuffImmunitySets

        // Custom AI, 0 is "bound town NPC" AI which slows the NPC down and changes sprite orientation towards the target
        NPC.aiStyle = -1;

        // The following code assigns a music track to the boss in a simple way.
        if (!Main.dedServ)
        {

            if (Main.dayTime) Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Introspectral");
            else Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Psychotortured");
        }
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        // Sets the description of this NPC that is listed in the bestiary
        bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
            new BossBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement("A dark reflection of the player, brought out through the magic of a cursed mirror.")
        });
    }


    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        int[] normalWeaponDrops = new int[]
        {
            ItemType<Items.Weapons.Melee.Nightfall>(),
            ItemType<Items.Weapons.Ranged.GammaRay>(),
            ItemType<Items.Weapons.Magic.StellarRemnant>(),
            ItemType<Items.Weapons.Summon.EchoingStarlash>(),
        };

        npcLoot.Add(ItemDropRule.FewFromOptionsNotScalingWithLuck(Main.expertMode ? 2 : 1, 1, normalWeaponDrops));

        npcLoot.Add(ItemDropRule.NotScalingWithLuck(ItemType<Drops.Pitchblende.Reflector>(), hasBecomeDaytime ? 100000 : 1));

        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Drops.Tiles.Trophy.LivingShadowTrophy>(), 10));

        //npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Materials.ShadowDrop.ShadowDrop>(), 1, 18, 36));

        npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Drops.Tiles.Relic.LivingShadowRelicItem>()));

        LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
    }

    public override void BossLoot(ref string name, ref int potionType)
    {
        potionType = ItemID.SuperHealingPotion;
        // Here you'd want to change the potion type that drops when the boss is defeated. Because this boss is early pre-hardmode, we keep it unchanged
        // (Lesser Healing Potion). If you wanted to change it, simply write "potionType = ItemID.HealingPotion;" or any other potion type
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        cooldownSlot = ImmunityCooldownID.Bosses; // use the boss immunity cooldown counter, to prevent ignoring boss attacks by taking damage from other sources
        return true;
    }

    public override void FindFrame(int frameHeight)
    {
        // This NPC animates with a simple "go from start frame to final frame, and loop back to start frame" rule
        // In this case: First stage: 0-1-2-0-1-2, Second stage: 3-4-5-3-4-5, 5 being "total frame count - 1"
        int startFrame = 0;
        int finalFrame = 3;

        int frameSpeed = 3;
        NPC.frameCounter += 0.5f;
        NPC.frameCounter += NPC.velocity.Length() / 10f; // Make the counter go faster with more movement speed
        if (NPC.frameCounter > frameSpeed)
        {
            NPC.frameCounter = 0;
            NPC.frame.Y += frameHeight;

            if (NPC.frame.Y > finalFrame * frameHeight)
            {
                NPC.frame.Y = startFrame * frameHeight;
            }
        }
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
        {
            NPC.TargetClosest();
        }

        Player player = Main.player[NPC.target];

        // If the NPC dies, spawn gore and play a sound
        if (Main.netMode == NetmodeID.Server)
        {
            // We don't want Mod.Find<ModGore> to run on servers as it will crash because gores are not loaded on servers
            return;
        }

        if (NPC.life <= 0)
        {
            var entitySource = NPC.GetSource_Death();

            SoundEngine.PlaySound(SoundID.PlayerKilled, NPC.Center);
        }
    }

    float timerBeforeStart = 0;

    public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damage)
    {
        if (hit.Damage > NPC.lifeMax / 2)
        {
            NPC.lifeMax *= 2;
            NPC.life = NPC.lifeMax;
            attemptedButchered = true;
        }
        base.OnHitByItem(player, item, hit, damage);
    }


    public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damage)
    {
        if (damage > NPC.lifeMax / 2)
        {
            NPC.lifeMax *= 2;
            NPC.life = NPC.lifeMax;
            attemptedButchered = true;
        }
        base.OnHitByProjectile(projectile, hit, damage);
    }

    public override void AI()
    {
        if (NPC.FindBuffIndex(BuffID.Confused) != -1) NPC.DelBuff(NPC.FindBuffIndex(BuffID.Confused));
        if (Main.dayTime) hasBecomeDaytime = true;
        underHalf = NPC.life < NPC.lifeMax / 2;

        // This should almost always be the first code in AI() as it is responsible for finding the proper player target
        if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
        {
            NPC.TargetClosest();
        }

        NPC.defense = (int)(Main.player[NPC.target].statDefense * 1.5f);

        Player player = Main.player[NPC.target];

        // TODO: Find the tick amount before the drop on a non-shitty computer
        if (timerBeforeStart <= 624)
        {
            distance = MathHelper.Lerp(distance, 100, 0.001f);
            angle = MathF.PI / 2;
        }
        else
        {
            distance = 200 + (200 * ((NPC.life * 1.0f) / NPC.lifeMax));
        }

        int ooga = 1;
        if (player.dead)
        {
            // If the targeted player is dead, flee
            NPC.dontTakeDamage = true;
            NPC.velocity.Y += 2;
            NPC.alpha += 20;
            if (NPC.alpha > 250)
            {
                NPC.velocity.Y += 40;
            }
            spriteScale = 0;

            // This method makes it so when the boss is in "despawn range" (outside of the screen), it despawns in 10 ticks
            NPC.EncourageDespawn(1);
            return;
        }

        NPC.damage = (int)(NPC.damage * (underHalf ? 1.5f : 1f));

        if (!Main.dayTime) NPC.damage = 999999;

        if (Main.getGoodWorld)
        {
            player.AddBuff(BuffID.Blackout, 18000, true);
            underHalf = true;
        }


        DoFirstStage(player);

        int testCoeff = 1;
        FirstStageTimerMax = ((int)(240 * ((NPC.life * 1.0f) / NPC.lifeMax)) + 120) * testCoeff;
        if (Main.getGoodWorld) FirstStageTimerMax = (int)(FirstStageTimerMax / 1.5f);
        if (attemptedButchered)
        {
            ambientTarget = 192;
            FirstStageTimerMax = 2;
        }

        NPC.Center = Vector2.Lerp(NPC.Center, player.Center + new Vector2(distance, 0).RotatedBy(angle), 0.1f);

        Dust dust = Dust.NewDustDirect(NPC.Center + Main.rand.NextVector2Circular(8, 24), 4, 4, DustID.Wraith, Scale: 1.25f);
        dust.noGravity = true;
        dust.alpha = 64;

        NPC.rotation = (player.velocity.X - NPC.velocity.X) / 28f;

        timerBeforeStart++;
    }

    float telegraph1 = 0, telegraph2 = 0, telegraph3 = 0;
    Projectile tProj1, tProj2, tProj3;

    private void DashAttack(float FirstStageTimer)
    {
        int halfway = FirstStageTimerMax / 2;

        Player player = Main.player[NPC.target];

        if (underHalf && FirstStageTimer > halfway - 80 && FirstStageTimer < halfway - 30)
        {
            spriteScale = MathHelper.Lerp(spriteScale, 160, 0.03f);
            alpha = 240;
        }

        if (!Main.getGoodWorld)
        {
            if (FirstStageTimer == halfway - 70)
            {

                telegraph1 = angle + MathF.PI;
                tProj1 = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromThis(),
                                         player.position + new Vector2(distance, 0).RotatedBy(telegraph1),
                                         Vector2.Zero,
                                         ProjectileID.BlackBolt,
                                         0,
                                         0)];
                tProj1.aiStyle = -1;
                tProj1.timeLeft = 1;
            }

            if (FirstStageTimer == halfway - 60)
            {
                telegraph2 = Main.rand.NextFloat(2 * MathF.PI);
                tProj2 = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromThis(),
                                         player.position + new Vector2(distance, 0).RotatedBy(telegraph2),
                                         Vector2.Zero,
                                         ProjectileID.BlackBolt,
                                         0,
                                         0)];
                tProj2.aiStyle = -1;
                tProj2.timeLeft = 1;
            }

            if (FirstStageTimer == halfway - 50)
            {
                telegraph3 = telegraph2 + MathF.PI;
                tProj3 = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromThis(),
                                         player.position + new Vector2(distance, 0).RotatedBy(telegraph3),
                                         Vector2.Zero,
                                         ProjectileID.BlackBolt,
                                         0,
                                         0)];
                tProj3.aiStyle = -1;
                tProj3.timeLeft = 1;
            }
        }
        else if (FirstStageTimer == halfway - 60)
        {
            telegraph1 = angle + MathF.PI;
            telegraph2 = Main.rand.NextFloat(2 * MathF.PI);
            telegraph3 = telegraph2 + MathF.PI;
            float rand = Main.rand.NextFloat(0, MathF.PI / 4f);
            for (int i = 0; i < 8; i++)
            {
                float telegraphAngle = rand + (i * MathF.PI / 4f);
                Projectile projectile = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromThis(),
                                         player.position + new Vector2(distance, 0).RotatedBy(telegraphAngle),
                                         Vector2.Zero,
                                         ProjectileID.BlackBolt,
                                         0,
                                         0)];
                projectile.aiStyle = -1;
                projectile.timeLeft = 1;
            }
        }

        if (FirstStageTimer >= halfway - 30)
        {
            spriteScale = MathHelper.Lerp(spriteScale, 0, 0.05f);
            if (underHalf) alpha = (int)MathHelper.Lerp(1.0f * alpha, 0, 0.1f);
        }

        if (FirstStageTimer == halfway)
        {
            angle = telegraph1;
        }
        if (FirstStageTimer == halfway + 30)
        {
            angle = telegraph2;
        }
        if (FirstStageTimer == halfway + 60)
        {
            angle = telegraph3;
        }
    }

    private void DashAttackII(float FirstStageTimer)
    {
        Player player = Main.player[NPC.target];
        int quarter = FirstStageTimerMax / 4;
        int halfway = FirstStageTimerMax / 2;
        int threeQuarters = 3 * quarter;

        
        if (FirstStageTimer <= 30)
        {
            if (player.velocity.Length() > float.Epsilon) angle = MathHelper.Lerp(angle, (player.Center + player.velocity).DirectionFrom(player.Center).ToRotation(), 0.15f);
        }
        if (FirstStageTimer < threeQuarters - 30 && FirstStageTimer > 30)
        {
            
            if (player.velocity.Length() > float.Epsilon) angle = (player.Center + player.velocity).DirectionFrom(player.Center).ToRotation();
        }
        if (FirstStageTimer == threeQuarters - 30)
        {
            Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromThis(),
                                     player.position + new Vector2(distance, 0).RotatedBy(angle + Math.PI),
                                     Vector2.Zero,
                                     ProjectileID.BlackBolt,
                                     0,
                                     0)].timeLeft = 1;
        }
        if (FirstStageTimer == threeQuarters) angle += MathF.PI;
        if (underHalf && FirstStageTimer > threeQuarters && FirstStageTimer % 25 == 0) angle += MathF.PI;
    }

    private void ShadowBoltsAttack(float FirstStageTimer)
    {
        alpha = 128;
        int halfway = FirstStageTimerMax / 2;
        Player player = Main.player[NPC.target];

        if (FirstStageTimer >= halfway - 60)
        {
            angle -= MathF.PI / 60;
            if (FirstStageTimer < halfway)
            {
                spriteScale = MathHelper.Lerp(spriteScale, 160, 0.01f);
            }
            shadeAngle += rotationRate;
            rotationRate = MathHelper.Lerp(rotationRate, MathF.PI / 15, 0.01f);
        }

        if (FirstStageTimer >= halfway - 40)
        {
            for (int i = 0; i < Main.rand.Next(1, 4); i++)
            {
                Vector2 pos = player.position + Main.rand.NextVector2CircularEdge(Main.screenWidth * MathF.Sqrt(2), Main.screenHeight * MathF.Sqrt(2));
                if (pos.Y < 0) pos.Y *= -1;
                NPC.NewNPCDirect(
                    NPC.GetSource_FromAI(),
                    pos,
                    ModContent.NPCType<ShadowBolt>(), ai3: NPC.damage);
            }
        }
        if (FirstStageTimer >= FirstStageTimerMax - 20)
        {
            spriteScale = MathHelper.Lerp(spriteScale, 0, 0.025f);
        }
    }

    float dashIItarget = 0;
    private void ShadowBoltsAttackII(float FirstStageTimer)
    {
        float halfway = FirstStageTimerMax / 2;
        Player player = Main.player[NPC.target];
        if (FirstStageTimer % 40 == 0 && FirstStageTimer != 0 && FirstStageTimer <= FirstStageTimerMax - 30)
        {
            for (int i = 0; i < Main.rand.Next(12, 18); i++)
            {
                NPC.NewNPCDirect(NPC.GetSource_FromThis(), NPC.Center, ModContent.NPCType<ShadowBoltII>(), ai3: NPC.damage);
            }
        }

        if (underHalf && FirstStageTimer == halfway - 30)
        {
            dashIItarget = Main.rand.NextFloat(2 * MathF.PI);
            Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromThis(),
                                     player.position + new Vector2(distance, 0).RotatedBy(dashIItarget),
                                     Vector2.Zero,
                                     ProjectileID.BlackBolt,
                                     0,
                                     0)].timeLeft = 1;

        }
        if (underHalf && FirstStageTimer == halfway)
        {
            angle = dashIItarget;
        }
    }

    private void ShadowBoltsAttackIII(float FirstStageTimer)
    {
        angle += 2 * MathF.PI / 120;

        if (FirstStageTimer % 10 == 0)
        {
            NPC.NewNPCDirect(NPC.GetSource_FromThis(), NPC.Center, ModContent.NPCType<ShadowBoltIII>(), ai3: NPC.damage);
            NPC.NewNPCDirect(NPC.GetSource_FromThis(), NPC.Center, ModContent.NPCType<ShadowBoltIII>(), ai3: NPC.damage);
            NPC.NewNPCDirect(NPC.GetSource_FromThis(), NPC.Center, ModContent.NPCType<ShadowBoltIII>(), ai3: NPC.damage);
        }
        }
    float curve = MathF.PI / 30f;
    private void ShadowBoltsDashAttack(float FirstStageTimer)
    {
        Player player = Main.player[NPC.target];
        if (FirstStageTimer % 15 == 0)
        {
            angle += MathF.PI / 12;
            curve *= -1;

            for (int i = 0; i < 4; i++)
            {
                NPC npc = NPC.NewNPCDirect(NPC.GetSource_FromThis(), NPC.Center, ModContent.NPCType<ShadowBoltIV>(), ai3: NPC.damage);
                npc.velocity = Main.rand.NextVector2CircularEdge(8, 8);
            }
        }
        if (FirstStageTimer % 30 == 0)
        {
            angle += MathF.PI / 4;
            float offsetAngle = Main.rand.NextFloat(0, (MathF.PI / 4));
        }
    }

    private void Darken(float FirstStageTimer)
    {
        int choice = 0;
        if (FirstStageTimer == 0)
        {
            choice = Main.rand.Next(6);
            ambientTarget += 32;
            if (ambientTarget >= 192) ambientTarget = 192;
        }
        if (choice == 0) DashAttack(FirstStageTimer);
        if (choice == 1) DashAttackII(FirstStageTimer);
        if (choice == 2) ShadowBoltsAttack(FirstStageTimer);
        if (choice == 3) ShadowBoltsAttackII(FirstStageTimer);
        if (choice == 4) ShadowBoltsAttackIII(FirstStageTimer);
        if (choice == 5) ShadowBoltsDashAttack(FirstStageTimer);
    }

    private void DrawBubble(Vector2 pos)
    {
        Texture2D texture = TextureAssets.Sun.Value;
        Rectangle frame = texture.Frame();
        Vector2 origin = new Vector2(frame.Width * 0.5f, frame.Height * 0.5f);
        Main.EntitySpriteDraw(texture,
                              pos,
                              frame,
                              new Color(0, 0, 0, alpha),
                              shadeAngle,
                              origin,
                              spriteScale,
                              SpriteEffects.None,
                              0);
    }

    float ambient = 0;
    float ambientTarget = 64;
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        ambient = MathHelper.Lerp(ambient, ambientTarget, 0.01f);
        Vector2 origin = new Vector2(TextureAssets.Sun.Value.Frame().Width * 0.5f, TextureAssets.Sun.Value.Frame().Height * 0.5f);
        Main.EntitySpriteDraw(TextureAssets.Sun.Value,
                              Main.player[NPC.target].position - screenPos,
                              TextureAssets.Sun.Value.Frame(),
                              new Color(0, 0, 0, (int)ambient),
                              0,
                              origin,
                              160,
                              SpriteEffects.None,
                              0);

        Main.EntitySpriteDraw(TextureAssets.Sun.Value,
                              Main.player[NPC.target].position - screenPos,
                              TextureAssets.Sun.Value.Frame(),
                              new Color(0, 0, 0, (int)ambient),
                              0,
                              NPC.position,
                              4,
                              SpriteEffects.None,
                              0);

        DrawBubble(Main.player[NPC.target].position - screenPos);
        return base.PreDraw(spriteBatch, screenPos, drawColor);
    }

    float shadeAngle = 0, spriteScale = 0, rotationRate = 0;
    int alpha = 0;

    int choice = 0;
    int oldChoice = -1;

    void ChooseAttack(int choice)
    {
        if (choice == 0) DashAttack(FirstStageTimer);
        if (choice == 1) DashAttackII(FirstStageTimer);
        if (choice == 2) ShadowBoltsAttack(FirstStageTimer);
        if (choice == 3) ShadowBoltsAttackII(FirstStageTimer);
        if (choice == 4) ShadowBoltsAttackIII(FirstStageTimer);
        if (choice == 5) ShadowBoltsDashAttack(FirstStageTimer);
        if (choice == 6) Darken(FirstStageTimer);
    }

    private void DoFirstStage(Player player)
    {

        // Each time the timer is 0, pick a random position a fixed distance away from the player but towards the opposite side
        // The NPC moves directly towards it with fixed speed, while displaying its trajectory as a telegraph

        if (timerBeforeStart >= 636)
        {
            FirstStageTimer++;
            if (FirstStageTimer > FirstStageTimerMax)
            {
                FirstStageTimer = 0;
            }
        }

        if (timerBeforeStart < 636)
        {
            NPC.dontTakeDamage = true;
            NPC.damage = 0;
        }
        if (timerBeforeStart == 636)
        {
            NPC.dontTakeDamage = false;
            NPC.damage = 67;
        }

        if (timerBeforeStart >= 636)
        {
            ChooseAttack(choice);
            if (Main.getGoodWorld && !Main.dayTime)
            {
                NPC trailer = NPC.NewNPCDirect(NPC.GetSource_FromThis(),
                                               NPC.position,
                                               ModContent.NPCType<ShadowBoltV>(), ai3: NPC.damage);
                trailer.velocity = NPC.velocity / 3f;
            }
        }


        int halfway = FirstStageTimerMax / 2;

        if (FirstStageTimer == FirstStageTimerMax - 20)
        {
            targetAngle = Main.rand.NextFloat(2 * MathF.PI);
        }

        if (FirstStageTimer >= FirstStageTimerMax - 20)
        {
            Dust.NewDustDirect(NPC.position + Main.rand.NextVector2Circular(8, 24),
                4, 4, DustID.Wraith, Scale: 4).noGravity = true;
            Dust.NewDustDirect(player.position + new Vector2(distance, 0).RotatedBy(targetAngle) + Main.rand.NextVector2Circular(8, 24),
                4, 4, DustID.Wraith, Scale: 4).noGravity = true;
        }

        if (FirstStageTimer == 0)
        {
            if (!underHalf) NPC.position = player.position + Main.rand.NextVector2Circular(8, 24) + new Vector2(distance, 0).RotatedBy(targetAngle);
            angle = targetAngle;
            spriteScale = 0;
            choice = Main.rand.Next(0, underHalf ? 7 : 5);
            while (choice == oldChoice)
            {
                choice = Main.rand.Next(0, underHalf ? 7 : 5);
            }
            oldChoice = choice;
        }

        if (FirstStageTimer <= 10 && timerBeforeStart >= 636)
        {
            Dust.NewDustDirect(NPC.position + Main.rand.NextVector2Circular(8, 24), 4, 4, DustID.Wraith, Scale: 4).noGravity = true;
        }

        if (FirstStageTimer == 15)
        {
            if (Main.rand.NextBool(1)) distance = -distance;
        }

        if (FirstStageTimer <= 3 && timerBeforeStart >= 636)
        {
            NPC.alpha -= 100;
            for (int i = 0; i < Main.rand.Next(8, 12); i++)
            {
                Dust.NewDustDirect(NPC.position + Main.rand.NextVector2Circular(8, 24), 4, 4, DustID.Wraith, Scale: 2).noGravity = true;
            }
        }
    }

    public override void OnKill()
    {
        Main.player[NPC.target].ClearBuff(BuffID.Blackout);
    }
}
