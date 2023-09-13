using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
namespace Conquest.Assets.Systems
{
    public class DownedBossSystem : ModSystem
    {
        public static bool DownedDuke = false;
        public static bool DownedAnubis = false;
        public static bool DownedBruiser = false;
        public static bool DownedQueen = false;

        public override void SaveWorldData(TagCompound tag)
        {
            if (DownedDuke)
            {
                tag["DownedDuke"] = true;
            }
            if (DownedBruiser)
            {
                tag["DownedBruiser"] = true;
            }
            if (DownedAnubis)
            {
                tag["DownedAnubis"] = true;
            }
            if (DownedQueen)
            {
                tag["DownedQueen"] = true;
            }
        }
        public override void ClearWorld()
        {
            DownedAnubis = false;
            DownedBruiser = false;
            DownedDuke = false;
            DownedQueen = false;

        }
        public override void LoadWorldData(TagCompound tag)
        {
            DownedDuke = tag.ContainsKey("DownedDuke");
            DownedBruiser = tag.ContainsKey("DownedBruiser");
            DownedAnubis = tag.ContainsKey("DownedAnubis");
            DownedQueen = tag.ContainsKey("DownedQueen");

        }

        public override void NetSend(BinaryWriter writer)
        {
            var flags = new BitsByte();
            flags[0] = DownedDuke;
            flags[1] = DownedBruiser;
            flags[2] = DownedAnubis;
            flags[3] = DownedQueen;
            writer.Write(flags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            DownedDuke = flags[0];
            DownedBruiser = flags[1];
            DownedAnubis= flags[2];
            DownedQueen = flags[3];

        }
    }
}
