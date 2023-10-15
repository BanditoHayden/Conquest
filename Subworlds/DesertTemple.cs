using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.WorldBuilding;
using Terraria.GameContent.Generation;
using Point16 = Terraria.DataStructures.Point16;
using StructureHelper;

namespace Conquest.Subworlds
{
    public class DesertTemple : Subworld
    {
        public override int Width => 800;
        public override int Height => 900;
        public override bool NormalUpdates => false;
        public override bool ShouldSave => false;
        public override bool NoPlayerSaving => false;

        public override List<GenPass> Tasks => new List<GenPass>()
        { 
               new PassLegacy ("Generating thing", (progress, _) =>
                {
                        progress.Message = "Generating Desert Temple";
                        Main.spawnTileX = 280;
                        Main.spawnTileY = 173;

                        Main.worldSurface = 225;
                        Main.rockLayer = 243;
                        int x = 100;
                        int y = 173;
                     Point16 point = new Point16(x, y);
                    Generator.GenerateStructure("Structures/DesertTempleUpper", point, Conquest.Instance, false);
                },90f),
                new PassLegacy ("Generating thing", (progress, _) =>
                {
                        progress.Message = "Generating Desert Temple";
                        int x = 100;
                        int y = 243;
                     Point16 point2 = new Point16(x, y);
                    Generator.GenerateStructure("Structures/DesertTempleMiddle", point2, Conquest.Instance, false);
                },120f),
                 new PassLegacy ("Generating thing", (progress, _) =>
                {
                        progress.Message = "Generating Desert Temple";
                        int x = 100;
                        int y = 415;
                     Point16 point2 = new Point16(x, y);
                    Generator.GenerateStructure("Structures/DesertTempleLower", point2, Conquest.Instance, false);
                },120f),

        };
        public override void OnEnter()
        {
            SubworldSystem.hideUnderworld = true;
            
        }
     
        public override void OnExit()
        {
            base.OnExit();
        }

    }

}    
