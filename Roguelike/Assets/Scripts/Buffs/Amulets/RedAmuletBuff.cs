using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedAmuletBuff : Buff
{
    public RedAmuletBuff(Player target) : base(target)
    {

    }

    public override void Update()
    {
        target.currentMaxHP = target.maxHP + target.maxHP * 0.1f;
    }
    public override void DisableBuff()
    {
        print("DisableBuff()");
        target.currentMaxHP = target.maxHP;
    }
}