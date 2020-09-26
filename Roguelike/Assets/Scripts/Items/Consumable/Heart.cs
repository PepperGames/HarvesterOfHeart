using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    public GameObject player;
    public Player target;
    public AudioClip audioClip;
    public GameObject buffPosition;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        target = player.GetComponent<Player>();
        buffPosition = target.Center;
    }
    private void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            ApplyBuff();
        }
    }
    public void ApplyBuff()
    {
        if (target.AddBuff(new HeartBuff(target, audioClip, buffPosition)))
        {
            int selectedSlot = gameObject.GetComponentInParent<Slot>().number;
            player.GetComponent<Inventory>().isFull[selectedSlot] = false;
            Destroy(gameObject);
        }
    }
}
