﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollBuff : Buff
{
    private float time = 15f;
    private bool clipIsStarted = false;
    public ScrollBuff(Player target, AudioClip audioClip, GameObject buffPosition) : base(target, audioClip, buffPosition)
    {

    }

    public override void Update()
    {
        if (!clipIsStarted)
        {
            AudioSource audioSource = buffPosition.GetComponent<AudioSource>();
            audioSource.clip = audioClip;
            audioSource.Play();
            clipIsStarted = true;
        }
        buffPosition.GetComponent<Animator>().SetInteger("state", 3);
        if (time <= 0)
        {
            DisableBuff();
        }
        target.currentDamageRatio = target.currentMaxDamageRatio * 0.75f;

        time -= Time.deltaTime;
    }
    public override void DisableBuff()
    {
        target.currentDamageRatio = target.currentMaxDamageRatio;
        target.RemoveBuff(this);
    }
}
