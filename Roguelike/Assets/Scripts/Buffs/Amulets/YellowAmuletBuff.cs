using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowAmuletBuff : Buff
{
    public YellowAmuletBuff(Player target) : base(target)
    {

    }

    public override void Update()
    {
        target.currentMaxDamageRatio = target.maxDamageRatio - target.maxDamageRatio * 0.1f;
    }
    public override void DisableBuff()
    {
        target.currentMaxDamageRatio = target.maxDamageRatio;
    }
}