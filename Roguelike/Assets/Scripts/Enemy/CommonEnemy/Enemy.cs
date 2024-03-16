
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Person
{
    //хп
    public float health;

    //путь
    public float speed;
    private List<Vector2> PathToTarget = new List<Vector2>();
    private PathFinder PathFinder;
    private bool isMooving;
    private GameObject Player;
    //позиции между которыми ходит 
    public Vector2 movePos1;
    public Vector2 movePos2;
    float[] p1p2 = new float[2];
    //время перед началом движения когда гг вышел из радиуса видимости
    public float timeToMove = 3;
    bool go1 = true;
    //атака
    [Header ("урон(коефициент перед умножением на лвл)")]
    public float damage;
    private Player player;
    public Transform attackPos;
    public float attackRange;
    public float startTimeBtwAttac;
    public float timeBtwAttac = 0;
    //кого бить
    public LayerMask whatIsEnemies;
    //чо дропает
    public GameObject HealthPotion, Scroll, Soull, GAmulet, BAmulet, YAmulet;
    //анимации
    public Animator anim;
    //пепел после смерти
    public GameObject deathEffect;
    public AudioSource[] audioSources;

    SpriteRenderer spriteRenderer;
    bool isRed = false;
    private float redVariable;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        if (Player != null)
        {
            PathFinder = GetComponent<PathFinder>();
            isMooving = true;


            Array.Copy(GameObject.FindGameObjectWithTag("levelGenerator").GetComponent<LevelGenerator>().GetPos1Pos2(), p1p2, 2);
            var position = transform.position;

            position.x = p1p2[0];
            position.y = p1p2[1];
            movePos1 = position;

            Array.Copy(GameObject.FindGameObjectWithTag("levelGenerator").GetComponent<LevelGenerator>().GetPos1Pos2(), p1p2, 2);
            position.x = p1p2[0];
            position.y = p1p2[1];
            movePos2 = position;
        }

        damage = damage * LevelGenerator.LVL;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        health = 7.8f * LevelGenerator.LVL;
        speed = UnityEngine.Random.Range(1f, 2f);

        anim = GetComponent<Animator>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        isRed = false;
    }

    void Update()
    {

        if (Player == null) return;
        float dis = Vector2.Distance(transform.position, Player.transform.position);
        if (Vector2.Distance(transform.position, Player.transform.position) < 3f)
        {
            timeToMove = 3f;
            Move(Player.transform.position, true);
        }
        if (Vector2.Distance(transform.position, Player.transform.position) >= 3f)
        {
            if (timeToMove <= 0)
            {
                if (go1)
                {
                    Move(movePos1, false);
                    if (Vector2.Distance(transform.position, movePos1) <= 1.5f)
                    {
                        go1 = false;
                        timeToMove = 3f;
                        anim.SetInteger("state", 0);
                    }
                }
                else
                {
                    Move(movePos2, false);
                    if (Vector2.Distance(transform.position, movePos2) <= 1.5f)
                    {
                        go1 = true;
                        timeToMove = 3f;
                        anim.SetInteger("state", 0);
                    }
                }
            }
            else
            {
                timeToMove -= Time.deltaTime;
            }


        }
        ChangeColor();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }

    public override void TakingDamage(float damage)
    {
        health -= damage;
        isRed = true;
        redVariable = 1.5f;
        if (health <= 0)
        {
            AmuletBuff.countDeadMobs++;

            Drop();

            GameObject.FindGameObjectWithTag("levelGenerator").GetComponent<LevelGenerator>().DecreaseMobCountOnLvl();
            RIP();
        }
    }
    private void ChangeColor()
    {
        if (isRed)
        {
            if (redVariable >= 1)
            {
                redVariable -= 0.03f;
                spriteRenderer.color = new Color(1f, 1f / redVariable, 1f / redVariable, 1f);
            }
            else
            {
                isRed = false;
            }
        }
    }

    private void Drop()
    {
        float r = (int)UnityEngine.Random.Range(0f, 4f);

        if (r == 0)
        {
            Instantiate(HealthPotion, transform.position, Quaternion.identity);
        }

        else if (r == 1)
        {
            Instantiate(Scroll, transform.position, Quaternion.identity);
        }

        else if (r == 2)
        {
            Instantiate(Soull, transform.position, Quaternion.identity);
        }

        else if (r == 3)
        {
            Instantiate(HealthPotion, transform.position, Quaternion.identity);
        }

        r = UnityEngine.Random.Range(0f, 1f);
        if (r <= DropAmuletChance(3, AmuletBuff.GdropCount, AmuletBuff.countDeadMobs))
        {
            Instantiate(GAmulet, transform.position, Quaternion.identity);
            AmuletBuff.GdropCount++;
        }
        r = UnityEngine.Random.Range(0f, 1f);
        if (r <= DropAmuletChance(2, AmuletBuff.BdropCount, AmuletBuff.countDeadMobs))
        {
            Instantiate(BAmulet, transform.position, Quaternion.identity);
            AmuletBuff.BdropCount++;
        }
        r = UnityEngine.Random.Range(0f, 1f);
        if (r <= DropAmuletChance(2, AmuletBuff.YdropCount, AmuletBuff.countDeadMobs))
        {
            Instantiate(YAmulet, transform.position, Quaternion.identity);
            AmuletBuff.YdropCount++;
        }
    }

    float DropAmuletChance(float k, float dropCount, float countDeadMobs)
    {
        float res = ((k - dropCount) / (100 - countDeadMobs)) * 1.4f * (k - dropCount);
        return res;
    }

    private void Move(Vector2 target, bool player)
    {
        //если видит игрока
        if (player)
        {
            if (Vector2.Distance(transform.position, Player.transform.position) > attackRange)
            {
                PathToTarget = PathFinder.GetPath(Player.transform.position);

                isMooving = true;
                if (Player.transform.position.x - transform.position.x < 0)
                {
                    anim.SetInteger("state", 2);

                }
                else if (Player.transform.position.x - transform.position.x >= 0)
                {
                    anim.SetInteger("state", 1);
                }

            }
            else
            {
                isMooving = false;
                anim.SetInteger("state", 0);
            }

            //атака
            if (PathToTarget.Count == 0 || PathToTarget.Count == 1)
            {
                if (timeBtwAttac <= 0)
                {
                    audioSources[0].Play();
                    if (Player.transform.position.x - transform.position.x < 0)
                    {
                        anim.SetInteger("state", 4);
                    }
                    else if (Player.transform.position.x - transform.position.x >= 0)
                    {
                        anim.SetInteger("state", 3);
                    }
                }
                else
                {
                    anim.SetInteger("state", 0);
                    timeBtwAttac -= Time.deltaTime;
                }
            }
            else
            {
                timeBtwAttac -= Time.deltaTime;
            }

            if (isMooving)
            {
                if (Vector2.Distance(transform.position, PathToTarget[PathToTarget.Count - 1]) > 0.1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(PathToTarget[PathToTarget.Count - 1].x, PathToTarget[PathToTarget.Count - 1].y, -91), speed * Time.deltaTime);
                }
                else
                {
                    anim.SetInteger("state", 0);
                    isMooving = false;
                }
            }
            else
            {
                PathToTarget = PathFinder.GetPath(Player.transform.position);
                isMooving = true;
            }
        }

        //если НЕ видит игрока
        else
        {
            if (Vector2.Distance(transform.position, target) > 0.2f)
            {
                PathToTarget = PathFinder.GetPath(target);

                isMooving = true;
                if (target.x - transform.position.x < 0)
                {
                    anim.SetInteger("state", 2);

                }
                else if (target.x - transform.position.x >= 0)
                {
                    anim.SetInteger("state", 1);
                }
            }
            if (PathToTarget.Count == 0)
            {
                return;
            }

            if (isMooving)
            {
                if (Vector2.Distance(transform.position, PathToTarget[PathToTarget.Count - 1]) > 0.1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(PathToTarget[PathToTarget.Count - 1].x, PathToTarget[PathToTarget.Count - 1].y, -91), speed * Time.deltaTime);
                    if (target.x - transform.position.x < 0)
                    {
                        anim.SetInteger("state", 2);

                    }
                    else if (target.x - transform.position.x >= 0)
                    {
                        anim.SetInteger("state", 1);
                    }
                    isMooving = true;
                }
                else
                {
                    isMooving = false;
                    anim.SetInteger("state", 0);
                }
            }
            else
            {
                PathToTarget = PathFinder.GetPath(target);
                if (target.x - transform.position.x < 0)
                {
                    anim.SetInteger("state", 2);

                }
                else if (target.x - transform.position.x >= 0)
                {
                    anim.SetInteger("state", 1);
                }
                isMooving = true;
            }
        }
    }

    public void Attack()
    {
        Collider2D PlayerToDamage = Physics2D.OverlapCircle(attackPos.position, attackRange, whatIsEnemies);
        if (PlayerToDamage != null)
        {
            print(PlayerToDamage);
                player.TakingDamage(damage/1.3f);
                timeBtwAttac = startTimeBtwAttac;
        }
        
    }

    public void SetStartTime()
    {
        timeBtwAttac = startTimeBtwAttac;
    }

    public void RIP()
    {
        Instantiate(deathEffect, new Vector3(transform.position.x, transform.position.y, -50), Quaternion.identity);
        Destroy(gameObject);
    }

}
