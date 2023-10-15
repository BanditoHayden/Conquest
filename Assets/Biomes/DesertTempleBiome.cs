using Conquest.Subworlds;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Conquest.Assets.Biomes
{
    public class DesertTempleBiome : ModBiome
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeLow;
        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.Find<ModUndergroundBackgroundStyle>("Conquest/DesertTempleSurfaceStyle");
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/DryDry");
        /*
        public override bool IsBiomeActive(Player player)
        {
            bool b1 = SubworldSystem.IsActive<DesertTemple>();

            return b1;
        }
        */
    }
}
