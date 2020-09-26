using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WomenBeamSplash : MonoBehaviour
{
    private Player player;
    public float damage;
    public float timeToDestroy;
    float timeBtwDamage;
    public AudioSource audioSource;
    void Start()
    {
        damage = 5 * LevelGenerator.LVL;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        timeBtwDamage = timeToDestroy;

        audioSource = GetComponent<AudioSource>();

        print(audioSource.clip);
        audioSource.Play();
    }
    //переделывать под анимацию
    void Update()
    {
        var scale = transform.localScale;
        if (scale.x <= 1f)
        {
            scale.x += 0.01f;
            scale.y += 0.01f;
            transform.localScale = scale;
        }
        timeToDestroy -= Time.deltaTime;
        if (timeToDestroy <= 0)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        player.TakingDamage((damage * Time.deltaTime * 0.58f)/ timeBtwDamage);
    }
}
