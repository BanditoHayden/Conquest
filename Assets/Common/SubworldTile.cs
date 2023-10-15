using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using tModPorter;

namespace Conquest.Assets.Common
{
    public class SubworldTile : GlobalTile
    {
        public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
        {
            if (SubworldSystem.AnyActive<Conquest>())
            {
                return false;
            }
            else return base.CanKillTile(i, j, type, ref blockDamaged);

        }
        public override bool CanExplode(int i, int j, int type)
        {
            if (SubworldSystem.AnyActive<Conquest>())
            {
                return false;
            }
            else return base.CanExplode(i, j, type);
        }
    }
}
    