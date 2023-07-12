﻿using Terraria;
using Terraria.ModLoader;
namespace Conquest.Dusts
{
    public class BrainDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true; // Makes the dust have no gravity.
            dust.noLight = true;

        }
    }
}
