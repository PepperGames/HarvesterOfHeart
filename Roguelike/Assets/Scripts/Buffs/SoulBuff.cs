using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulBuff : Buff
{
    private float time = 15f;
    private bool clipIsStarted = false;
    public SoulBuff(Player target, AudioClip audioClip, GameObject buffPosition) : base(target, audioClip, buffPosition)
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
        buffPosition.GetComponent<Animator>().SetInteger("state", 2);
        if (time <= 0)
        {
            target.currentDamage = target.currentMaxDamage;
            Debug.Log(time);
            target.RemoveBuff(this);
        }
        target.currentDamage = target.currentMaxDamage * 1.25f;

        time -= Time.deltaTime;
    }
}