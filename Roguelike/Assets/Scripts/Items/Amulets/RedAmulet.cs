using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedAmulet : Amulet
{
    public GameObject player;
    public Player target;
    public RedAmuletBuff redAmuletBuff;

    public override void ApplyBuff()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        target = player.GetComponent<Player>();

        redAmuletBuff = new RedAmuletBuff(target);

        target.AddBuff(new RedAmuletBuff(target));
    }
    public override void DisableBuff()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        target = player.GetComponent<Player>();
        print("DisableBuff start красній");
        redAmuletBuff = new RedAmuletBuff(target);
        redAmuletBuff.DisableBuff();
        print("redAmuletBuff.DisableBuff() красній");
        target.RemoveBuff(redAmuletBuff);
    }
}
