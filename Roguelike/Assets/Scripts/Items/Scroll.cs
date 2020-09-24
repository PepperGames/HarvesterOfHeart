using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Scroll : MonoBehaviour
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
        if (Input.GetKeyDown("3"))
        {
            ApplyBuff();
        }
    }
    private void ApplyBuff()
    {
        if (target.AddBuff(new ScrollBuff(target, audioClip, buffPosition)))
        {
            Destroy(gameObject);
        }
    }
}
