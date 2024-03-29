﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigWomenBeam : MonoBehaviour
{
    public Player player;
    public float damage;
    public float timeToDestroy;
    private float timeBtwDamage;
    private bool right;
    public float speed;
    public AudioSource audioSource;
    void Start()
    {
        damage = 6f * LevelGenerator.LVL;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        timeBtwDamage = timeToDestroy;
        float r = Random.Range(0, 2);
        if (r < 1)
            right = false;
        else right = true;


        print(audioSource.clip);
        audioSource.Play();
    }
    private void FixedUpdate()
    {
        if (right)
        {
            transform.Translate(Vector3.right * Time.deltaTime * speed, Space.World);
            if (transform.position.x >= 11)
            {
                right = false;
            }
        }
        else
        {
            transform.Translate(Vector3.left * Time.deltaTime * speed, Space.World);
            if (transform.position.x <= 3)
            {
                right = true;
            }
        }
        
        timeToDestroy -= Time.deltaTime;
        if (timeToDestroy <= 0)
        {
            Destroy(gameObject);
        }
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        player.TakingDamage((damage * Time.deltaTime));
    }
}
