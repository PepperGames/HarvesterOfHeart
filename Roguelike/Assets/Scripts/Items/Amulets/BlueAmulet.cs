using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueAmulet : Amulet
{
    public GameObject player;
    public Player target;
    public BlueAmuletBuff blueAmuletBuff;

    public override void ApplyBuff()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        target = player.GetComponent<Player>();

        blueAmuletBuff = new BlueAmuletBuff(target);

        target.AddBuff(new BlueAmuletBuff(target));
    }
    public override void DisableBuff()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        target = player.GetComponent<Player>();
        print("DisableBuff start красній");
        blueAmuletBuff = new BlueAmuletBuff(target);
        blueAmuletBuff.DisableBuff();
        print("redAmuletBuff.DisableBuff()");
        target.RemoveBuff(blueAmuletBuff);
    }
}
