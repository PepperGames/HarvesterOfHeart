﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WomenBeam : MonoBehaviour
{
    public float damage;
    public float timeBtwAttac;
    private Player player;
    public GameObject attackPos;
    public float attackRange;
    public LayerMask whatIsEnemies;
    public AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        damage = 12 * LevelGenerator.LVL;

        print(audioSource.clip);
        audioSource.Play();
    }

    public void Attack()
    {
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.transform.position, attackRange, whatIsEnemies);
        if (enemiesToDamage.Length > 0)
        {
            player.TakingDamage(damage / 16);
        }   
            
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.transform.position, attackRange);
    }

    public void RIP()
    {
        Destroy(gameObject);
    }
}
