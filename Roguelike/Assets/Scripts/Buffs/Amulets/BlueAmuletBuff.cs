using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueAmuletBuff : Buff
{
    public BlueAmuletBuff(Player target) : base(target)
    {

    }

    public override void Update()
    {
        target.currentMaxDamage = target.MaxDamage + target.MaxDamage * 0.1f;
    }
    public override void DisableBuff()
    {
        target.currentMaxDamage = target.MaxDamage;
    }
}
