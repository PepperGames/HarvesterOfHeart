using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartBuff : Buff
{
    public HeartBuff(Player target, AudioClip audioClip, GameObject buffPosition) : base(target, audioClip, buffPosition)
    {

    }

    public override void Update()
    {
        AudioSource audioSource = buffPosition.GetComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.Play();
        buffPosition.GetComponent<Animator>().SetInteger("state", 1);
        target.currentHP += target.maxHP * 0.25f;
        if (target.currentHP > target.currentMaxHP)
        {
            target.currentHP = target.currentMaxHP;
        }
        target.DisplayHP();
        target.RemoveBuff(this);
    }
}