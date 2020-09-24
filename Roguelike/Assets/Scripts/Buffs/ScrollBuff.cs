using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollBuff : Buff
{
    private float time = 5f;
    private bool clipIsStarted = false;
    public ScrollBuff(Player target, AudioClip audioClip, GameObject buffPosition) : base(target, audioClip, buffPosition)
    {

    }

    // Update is called once per frame
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
        if (time<=0)
        {
            target.currentDamageRatio = target.currentMaxDamageRatio;
            Debug.Log(time);
            target.RemoveBuff(this);
        }
        target.currentDamageRatio = target.currentMaxDamageRatio * 0.75f;

        time -= Time.deltaTime;
    }
}
