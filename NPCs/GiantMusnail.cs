using Conquest.Items.Accessory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Conquest.NPCs
{
    public class GiantMusnail : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Velocity = 1f,
                Direction = -1

            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundMushroom,

                new FlavorTextBestiaryInfoElement("It's shell is extremely durable, but its rather heavy, causing the Musnail to be very slow."),

            });
        }
        public override void SetDefaults()
        {
            NPC.width = 80;
            NPC.height = 38;
            NPC.damage = 20;
            NPC.defense = 60;
            NPC.lifeMax = 55;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = 3;
            NPC.npcSlots = 1;
            NPC.noGravity = false;
            NPC.npcSlots = 1;
            AIType = NPCID.ZombieMushroom;
            AnimationType = NPCID.ZombieMushroom;

        }
        public override void AI()
        {
            NPC.TargetClosest(true);
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Snail1").Type, 1f);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Snail2").Type, 1f);
            }
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.SpawnTileType == TileID.MushroomGrass && NPC.CountNPCS(ModContent.NPCType<GiantMusnail>()) != 3)
            {
                return 0.1f;
            }
            else return 0f;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SnailShell>(), 30));
        }
    }
}

