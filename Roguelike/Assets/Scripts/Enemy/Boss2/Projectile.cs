using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    private Vector3 Player;

    public float damage;
    public float speed;
    private Player player;
    private Vector3 V = new Vector3(0, 0, -90);
    public float smoothTime = 0.9F;
    void Start()
    {
        GameObject _player = GameObject.FindWithTag("Player");
        Player = _player.transform.position;
        damage = 5 * LevelGenerator.LVL;
        player = _player.GetComponent<Player>();
    }


    void Update()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, Player, step);
        if (transform.position == Player)
        {
            Destroy(gameObject);
        }
    }

    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.GetComponent<Player>().attackable)
            {
                player.TakingDamage(damage);
                Destroy(gameObject);
            }
        }

        else if (other.gameObject.CompareTag("wall"))
            Destroy(gameObject);
    }
}
