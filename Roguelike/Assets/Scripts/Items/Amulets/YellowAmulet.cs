using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowAmulet : Amulet
{
    public GameObject player;
    public Player target;
    public YellowAmuletBuff yellowAmuletBuff;

    public override void ApplyBuff()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        target = player.GetComponent<Player>();

        yellowAmuletBuff = new YellowAmuletBuff(target);
        target.AddBuff(new YellowAmuletBuff(target));
    }
    public override void DisableBuff()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        target = player.GetComponent<Player>();
        print("DisableBuff start красній");
        yellowAmuletBuff = new YellowAmuletBuff(target);
        yellowAmuletBuff.DisableBuff();
        target.RemoveBuff(yellowAmuletBuff);
    }
}
