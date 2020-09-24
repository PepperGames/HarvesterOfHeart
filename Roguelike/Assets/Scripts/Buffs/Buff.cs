using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Buff
{
    protected Player target;
    public AudioClip audioClip;
    public GameObject buffPosition;
    public Buff()
    {
    }
    public Buff(Player target, AudioClip audioClip, GameObject buffPosition)
    {
        this.target = target;
        this.audioClip = audioClip;
        this.buffPosition = buffPosition;
    }

    public virtual void Update()
    {
        
    }
}
