using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Conquest.Items.Weapons.Magic.Energy;

namespace Conquest.Items.Accessory;

public class LatentEnergy : InfoDisplay
{
    /*public override void SetStaticDefaults()
    {
        InfoName.SetDefault("Latent Energy");
    }*/

    public override bool Active()
    {
        return Main.LocalPlayer.GetModPlayer<InformationPlayer>().accEnergyTracker;
    }

    int timer = 0;
    string energyVal = "100%";
    public override string DisplayValue(ref Color displayColor)
    {
        if (timer++ % 5 == 0)
        {
            float data = Main.LocalPlayer.GetModPlayer<EnergyPlayer>().energyPower;
            energyVal = "" + MathF.Round(data * 100) + "%";
        }
        
        return "Latent Energy Level: " + energyVal;
    }
}

