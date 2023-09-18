using Conquest.Items.Materials;
using Conquest.Items.Weapons.Magic;
using Conquest.Buffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Conquest.NPCs
{
    public class CRTZombie : ModNPC
    {
        // changed by Goose
        // now a bit laggy
        // ~~Attacking the CRT Zombie lags the player~~
        // someone didn't want an interesting npc >:(
        // now only getting hit will inflict Lag
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Zombie];
        }
        public override void SetDefaults()
        {
            NPC.width = 34;
            NPC.height = 46;
            NPC.damage = 20;
            NPC.defense = 30;
            NPC.lifeMax = 100;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 500f;
            NPC.knockBackResist = 0f;
            AIType = NPCID.Zombie;
            AnimationType = NPCID.Zombie;
            NPC.aiStyle = 3;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (NPC.downedBoss3)
            {
                return SpawnCondition.OverworldNightMonster.Chance * 0.2f;
            }
            else return 0f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<Lag>(), 120);
        }

        public override void AI()
        {
            base.AI();
            NPC.AddBuff(ModContent.BuffType<Lag>(), 60);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("crtgore").Type, 1f);
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CRTGun>(), 11));

        }
    }
}
