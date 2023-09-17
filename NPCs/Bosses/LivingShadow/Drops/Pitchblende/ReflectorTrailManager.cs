using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Conquest.NPCs.Bosses.LivingShadow.Drops.Pitchblende;

public class ReflectorTrailManager : GlobalProjectile
{
    public override bool InstancePerEntity => true;

    public float[] rotations = new float[10];

    public void InsertAtZero(ref float[] arr, in float val)
    {
        for (int i = arr.Length - 1; i >= 1; i--)
        {
            arr[i] = arr[i - 1];
        }
        arr[0] = val;
    }

    public override void PostAI(Projectile projectile)
    {
        InsertAtZero(ref rotations, projectile.velocity.ToRotation());
    }
}

