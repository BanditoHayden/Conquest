using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Conquest.Assets.Common
{
    public class ArtifactRarity : ModRarity
    {
        public override Color RarityColor => new Color(255, 221, 45);

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            return Type; 
        }
    }
}
