using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;

namespace Conquest.Items.Weapons.Magic.Energy;

public class EnergyPlayer : ModPlayer
{
    public float energyPower = 0.00f;
    float privateEnergyPower = 0.00f;

    public int[] magicTiles = new int[]
    {
        TileID.CrystalBall,
        TileID.ManaCrystal,
        TileID.LunarBlockNebula,
        TileID.LunarBrick,
    };

    public int[] veryMagicTiles = new int[]
    {
        TileID.Gold,
        TileID.GoldBrick,
        TileID.GoldCoinPile,
        TileID.AncientGoldBrick,
    };

    public int[] veryUnmagicTiles = new int[]
    {
        TileID.Platinum,
        TileID.PlatinumBrick,
        TileID.PlatinumCoinPile,
        TileID.PlatinumCandle,
    };

    public override void PreUpdate()
    {
        float count = 0;
        float energyScore = 0f;
        energyPower = 0;

        if (Player.ZoneShimmer)
        {
            energyScore += 2.00f;
            count++;
        }
        if (Player.ZoneCorrupt || Player.ZoneCrimson)
        {
            energyScore += 0.50f;
            count++;
        }
        if (Player.ZoneGraveyard)
        {
            energyScore += 0.50f;
            count++;
        }
        if (Player.ZoneDungeon || Player.ZoneLihzhardTemple)
        {
            energyScore += 1.25f;
            count++;
        }
        if (Player.ZoneMeteor)
        {
            energyScore += 1.50f;
            count++;
        }
        if (Player.ZoneWaterCandle)
        {
            energyScore += 1.25f;
            count++;
        }
        if (Player.ZoneShadowCandle)
        {
            energyScore += 0.25f;
            count++;
        }


        

        for (int i = -10; i < 11; i++)
        {
            for (int j = -10; j < 11; j++)
            {
                Point playerCoord = Player.Center.ToTileCoordinates();
                Terraria.Tile t = Main.tile[playerCoord.X + i, playerCoord.Y + j];
                if (Contains(magicTiles, t.TileType))
                {
                    energyScore += 1.50f;
                    count++;
                }
                if (Contains(veryMagicTiles, t.TileType))
                {
                    energyScore += 2.00f;
                    count++;
                }
                if (Contains(veryUnmagicTiles, t.TileType))
                {
                    energyScore += 0.7f;
                    count++;
                }
            }
        }

        if (count == 0) energyPower = 1.00f;
        else
        {
            privateEnergyPower = MathHelper.Lerp(privateEnergyPower, (energyScore / count), 0.01f);
            energyPower = privateEnergyPower;
        }


        if (Player.HasBuff(BuffID.MagicPower)) energyPower *= 1.25f;
        if (Player.HasBuff(BuffID.Clairvoyance)) energyPower *= 1.50f;


        if (energyPower < 0.01f) energyPower = 0.01f;
    }


    public bool Contains(int[] arr, int val)
    {
        foreach (int v in arr)
        {
            if (v == val) return true;
        }
        return false;
    }
}

