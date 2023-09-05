using Conquest.Items.Accessory;
using Conquest.Items.Tools;
using Conquest.Items.Weapons.Magic;
using Conquest.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Conquest.Assets.Common
{
    public class MyNpc : GlobalNPC
    {
        public int electrifiedTier = 0;
        private float velocityIntensity = 0.01f;
        private int oldAI;
        private bool oldAIisStored;
        private float oldRotation;
        private bool oldRotationisStored;
        public static bool noBoss;
        public int minionMark = 0;
        public int speedMelee = 0;
        public int speedMeleeClear = 0;

        public override bool InstancePerEntity => true;

        public override void ResetEffects(NPC npc)
        {
            electrifiedTier = 0;
            if (npc.GetGlobalNPC<MyNpc>().minionMark > 0)
            {
                if (npc.GetGlobalNPC<MyNpc>().minionMark == 1)
                {
                    Terraria.Dust.NewDust(npc.position, 30, 30, DustID.Firework_Red, 0, 0, 0, default, 1);
                }
                else if (npc.GetGlobalNPC<MyNpc>().minionMark == 2)
                {
                    Terraria.Dust.NewDust(npc.position, 30, 30, DustID.Firework_Yellow, 0, 0, 0, default, 1);
                }
                else if (npc.GetGlobalNPC<MyNpc>().minionMark == 3)
                {
                    Terraria.Dust.NewDust(npc.position, 30, 30, DustID.Firework_Blue, 0, 0, 0, default, 1);
                }
            }
            if (npc.GetGlobalNPC<MyNpc>().speedMelee > 5)
            {
                npc.GetGlobalNPC<MyNpc>().speedMelee = 5;
            }

            if (++npc.GetGlobalNPC<MyNpc>().speedMeleeClear >= 100)
            {
                npc.GetGlobalNPC<MyNpc>().speedMeleeClear = 0;
                npc.GetGlobalNPC<MyNpc>().speedMelee = 0;
            }
        }
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (electrifiedTier > 0)
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= 70 * electrifiedTier;
                if (damage < 10 * electrifiedTier)
                {
                    damage = 10 * electrifiedTier;
                }
            }
        }
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            MyPlayer modPlayer = player.GetModPlayer<MyPlayer>();
            if (modPlayer.war)
            {
                spawnRate = (int)(spawnRate * 0.01);
                maxSpawns = (int)(maxSpawns * 3);
            }
        }
        public override void OnKill(NPC npc)
        {
            Player player = Main.player[Main.myPlayer];
            MyPlayer modPlayer = player.GetModPlayer<MyPlayer>();
            if (modPlayer.Glory)
            {
                if (Main.rand.NextBool(150))
                {
                    Item.NewItem(npc.GetSource_Death(), npc.position, new Vector2(npc.width, npc.height), ItemID.GoldCoin);
                }
            }
        }
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {

            if (npc.type == NPCID.Bunny)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RabbitsFoot>(), 75));
            }

            if (npc.type == NPCID.GoldBunny)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GoldenRabbitsFoot>(), 20));
            }
            if (npc.type == NPCID.GraniteGolem)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<OreClub>(), 25));
            }
            if (npc.type == NPCID.GoblinPeon || npc.type == NPCID.GoblinSorcerer || npc.type == NPCID.GoblinThief || npc.type == NPCID.GoblinWarrior)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<WarPick>(), 95));
            }
            if(npc.type == NPCID.GoblinSummoner)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShadowflameCandle>(), 10));
            }
            if (npc.type == NPCID.Harpy)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AeroScimitar>(), 300));
            }
            if (npc.type == NPCID.BlueJellyfish || npc.type == NPCID.GreenJellyfish || npc.type == NPCID.PinkJellyfish)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FriendshipBracelet>(), 30));
            }
            if(npc.type == NPCID.RustyArmoredBonesFlail)
            {
              npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ChainChomp>(), 10));
            }
            if (npc.type == NPCID.Mothron)
            {
                if (NPC.downedGolemBoss)
                {
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DarkDawn>(), 10));
                }
            }
        }
        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (electrifiedTier > 0)
            {
                for (int i = 0; i < electrifiedTier; i++)
                {
                    int dust = Dust.NewDust(npc.position, npc.width, npc.height, 226, npc.velocity.X * 0f, npc.velocity.Y * 0f, 100, default(Color), .4f);
                    Main.dust[dust].noGravity = true;
                }
            }
        }
    }
}
