using Conquest.Assets.Systems;
using Conquest.NPCs.Bosses.Anubis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Conquest.Items.Weapons.Ranged;
using Conquest.Items.Weapons.Melee;
using Conquest.Items.Weapons.Magic;
using Conquest.Items.BossBags;
using Conquest.Items.Weapons.Summon;
using Conquest.Projectiles.Melee;
using Conquest.NPCs.Bosses.Danduke;
using Conquest.Items.Tile;
using Conquest.NPCs.Bosses;
using Conquest.Items.Accessory;

namespace Conquest.Assets.Common
{
    public class CrossMod : ModSystem
    {
        public override void PostSetupContent()
        {
            DoBossChecklistIntegration();
        }
        private void DoBossChecklistIntegration()
        {
            if (!ModLoader.TryGetMod("BossChecklist", out Mod bossChecklistMod))
            {
                return;
            }
            if (bossChecklistMod.Version < new Version(1, 6))
            {
                return;
            }
            string internalName = "Anubis";
            float weight = 12.7f;
            Func<bool> downed = () => DownedBossSystem.DownedAnubis;
            int bossType = ModContent.NPCType<Anubis>();
            List<int> collectibles = new List<int>()
            {

                ModContent.ItemType<AnubisBossBag>(),
                ModContent.ItemType<AnubisCannon>(),
                ModContent.ItemType<AnubisHammer>(),
                ModContent.ItemType<AnubisStaff>(),
                ModContent.ItemType<EmeraldSlate>(),
            };
            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                internalName,
                weight,
                downed,
              //  spawnInfo,
                bossType,
                new Dictionary<string, object>()
                {
                    ["collectibles"] = collectibles,
                    
                  //  ["customPortrait"] = customPortrait
                    // Other optional arguments as needed are inferred from the wiki
                }
            );
            string internalName2 = "Danduke";
            float weight2 = 5.1f;
            Func<bool> downed2 = () => DownedBossSystem.DownedDuke;
            int bossType2 = ModContent.NPCType<DandukeBoss>();
            List<int> collectibles2 = new List<int>()
            {

                ModContent.ItemType<DandukeBossBag>(),
                ModContent.ItemType<WindSong>(),
                ModContent.ItemType<BombingStaff>(),
                ModContent.ItemType<Petal>(),
                ModContent.ItemType<DandukeRelic>(),
                ModContent.ItemType<DandukeTrophy>(),
                ModContent.ItemType<WindGrass>(),


            };
            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                internalName2,
                weight2,
                downed2,
                //  spawnInfo,
                bossType2,
                new Dictionary<string, object>()
                {
                    ["collectibles"] = collectibles2,

                    //  ["customPortrait"] = customPortrait
                    // Other optional arguments as needed are inferred from the wiki
                }
            );
            string internalName3 = "AntlionQueen";
            float weight3 = 2.1f;
            Func<bool> downed3 = () => DownedBossSystem.DownedQueen;
            int bossType3 = ModContent.NPCType<AntlionQueen>();
            List<int> collectibles3 = new List<int>()
            {

                ModContent.ItemType<QueenBag>(),
                ModContent.ItemType<GloriousLauncher>(),
                ModContent.ItemType<GloriousSpear>(),
                ModContent.ItemType<GloriousScepter>(),

            };
            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                internalName3,
                weight3,
                downed3,
                //  spawnInfo,
                bossType3,
                new Dictionary<string, object>()
                {
                    ["collectibles"] = collectibles3,

                    //  ["customPortrait"] = customPortrait
                    // Other optional arguments as needed are inferred from the wiki
                }
            );
        }
    }
}
