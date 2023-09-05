using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace Conquest.Buffs;

// This class serves as an example of a debuff that causes constant loss of life
// See ExampleLifeRegenDebuffPlayer.UpdateBadLifeRegen at the end of the file for more information
public class VortexFlames : ModBuff
{
    int stacks = 0;
    public override void SetStaticDefaults()
    {
        //DisplayName.SetDefault("Vortex Flames"); // Buff display name
        //Description.SetDefault("Losing life"); // Buff description
        Main.debuff[Type] = true;  // Is it a debuff?
        Main.pvpBuff[Type] = true; // Players can give other players buffs, which are listed as pvpBuff
        Main.buffNoSave[Type] = true; // Causes this buff not to persist when exiting and rejoining the world
        BuffID.Sets.LongerExpertDebuff[Type] = true; // If this buff is a debuff, setting this to true will make this buff last twice as long on players in expert mode
    }

    public override void Update(NPC npc, ref int buffIndex)
    {
        npc.lifeRegen = -npc.buffTime[buffIndex] / 2;
        for (int i = 0; i < (int)MathF.Ceiling(npc.buffTime[buffIndex] / 100f); i++)
        {
            Dust.NewDustDirect(npc.TopLeft, npc.width, npc.height, DustID.Vortex).noGravity = true;
        }
    }

    public override bool ReApply(NPC npc, int time, int buffIndex)
    {
        if (time < 1000) return base.ReApply(npc, time + npc.buffTime[buffIndex], buffIndex);
        else return base.ReApply(npc, 1000, buffIndex);
    }
}