using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria;
using System.Collections.Generic;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;
using Microsoft.Xna.Framework.Graphics;
using System;
using ReLogic.Content;
using System.Security.Cryptography;
using Terraria.ID;
using Terraria.Audio;
using SubworldLibrary;
using Terraria.WorldBuilding;
using Terraria.IO;
using StructureHelper;
using Terraria.DataStructures;
using Conquest.Subworlds;
using Conquest.Items.Weapons.Melee;

namespace Conquest.Assets.Common {
    public class MyWorld : ModSystem
    {

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));
            int DesertIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
            int finalCleanupIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
            if (finalCleanupIndex != -1)
            {
                tasks.Insert(finalCleanupIndex + 1, new TempleTest("Bruh", 237.4298f));

            }
        }
        public override void PreUpdateWorld()
        {
            if (SubworldSystem.IsActive<DesertTemple>())
            {
                // Main.LocalPlayer.ZoneDesert = true;
                Wiring.UpdateMech();
                TileEntity.UpdateStart();
                foreach (TileEntity te in TileEntity.ByID.Values)
                {
                    te.Update();
                }
                TileEntity.UpdateEnd();
                if (++Liquid.skipCount > 1)
                {
                    Liquid.UpdateLiquid();
                    Liquid.skipCount = 0;
                }
            }
        }
        public override void PostWorldGen()
        {
            int[] itemsToPlaceInIceChests = { ItemType<MohgsTrident>() };
            int itemsToPlaceInIceChestsChoice = 0;
            for (int chestIndex = 0; chestIndex < 1000; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                if (chest != null && Main.tile[chest.x, chest.y].TileType == TileID.Containers && Main.tile[chest.x, chest.y].TileFrameX == 4 * 36)
                {
                    for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                    {
                        if (chest.item[inventoryIndex].type == 0)
                        {
                            if (!WorldGen.genRand.NextBool(5)) break;
                            chest.item[inventoryIndex].SetDefaults(itemsToPlaceInIceChests[itemsToPlaceInIceChestsChoice]);
                            itemsToPlaceInIceChestsChoice = (itemsToPlaceInIceChestsChoice + 1) % itemsToPlaceInIceChests.Length;
                            break;
                        }
                    }
                }
            }

        }
        public class TempleTest : GenPass
        {
            public TempleTest(string name, double loadWeight) : base(name, loadWeight)
            {
            }

            protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
            {
                int x = GenVars.UndergroundDesertLocation.X + (GenVars.UndergroundDesertLocation.Width / 2);
                int y = GenVars.UndergroundDesertLocation.Y + (GenVars.UndergroundDesertLocation.Height / 2);
                Point16 point = new Point16(x, y);
                Generator.GenerateStructure("Structures/AntlionQueenSpawner", point, Conquest.Instance, false);
            }

        }
    }
}