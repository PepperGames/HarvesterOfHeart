using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Player : Person
{
    //движение
    public float speed;

    private enum State
    {
        Normal,
        Rolling,
    }

    private Rigidbody2D rb2d;

    public Animator anim;

    float goHorizontal;
    float goVertical;

    private Vector3 moveDir;
    private Vector3 rollDir;
    private float rollSpeed;
    private float distanceTraveled;

    public float maxRollDistance;

    private State state;

    public LayerMask rollLayerMask;

    //атака
    public Transform attackPos;
    public LayerMask whatIsEnemies;
    public float attackRange;


    public float startTimeBtwAttac;

    public float MaxDamage; //максимальный урон без шмотки
    public float currentMaxDamage; //максимальный урон под бафом шмотки
    public float currentDamage;//текущее

    private Inventory inventory;
    private float timeBtwAttac = 0;
    //private float timeBtwAttacForAttack2 = 0;
    //private int attackCount = 0;

    private Camera cam;

    Vector2 mousePoint;

    public GameObject Center;
    private Animator CenterAnim;
    //private bool isPlayed = false;

    //хп
    public float maxHP;//максимальное хп без шмотки
    public float currentMaxHP;//максимальное хп под бафом шмотки
    public float currentHP;//текущее

    public float maxDamageRatio;//максимальный коеф без шмотки    
    public float currentMaxDamageRatio;//текущий под бафом шмотки
    public float currentDamageRatio;//текущий коеф дамага

    SpriteRenderer spriteRenderer;
    bool isRed = false;

    public Slider slider;
    public Image fillImage;

    private Color color1;
    private Color color2;
    private int changeColorTime = 5;

    public bool attackable;

    private float redVariable;

    public AudioSource[] audioSources;

    private List<Buff> buffs = new List<Buff>();

    [SerializeField]
    private AnalyticsComponent analytics;

    private void Start()
    {
        //атака
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        currentMaxDamage = 2;
        currentDamage = MaxDamage = currentMaxDamage;
        anim = GetComponent<Animator>();
        cam = Camera.main;
        CenterAnim = Center.GetComponent<Animator>();
        //движжение
        rb2d = GetComponent<Rigidbody2D>();
        speed = 120f;
        anim = GetComponent<Animator>();
        state = State.Normal;
        //хп
        maxHP = 16f;
        currentHP  = maxHP;
        currentMaxHP = maxHP;
        maxDamageRatio = currentMaxDamageRatio = currentDamageRatio = 1;
        color1 = new Color(255, 255, 255, 1f);
        color2 = new Color(255, 255, 255, 0f);
        fillImage.color = color1;
        attackable = true;
        CenterAnim = Center.GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        isRed = false;
        DisplayHP();
    }

    void Update()
    {
        DisplayHP();
        HandleBuff();
        //движение
        switch (state)
        {
            case State.Normal:
                goHorizontal = Input.GetAxisRaw("Horizontal");
                goVertical = Input.GetAxisRaw("Vertical");

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    rollDir = moveDir;
                    rollSpeed = 21f;
                    state = State.Rolling;
                    distanceTraveled = 0;
                    attackable = false;

                    if (goHorizontal < 0)
                    {
                        anim.SetInteger("state", 21);
                    }
                    else if (goHorizontal > 0)
                    {
                        anim.SetInteger("state", 20);
                    }
                    else if (goHorizontal == 0)
                    {
                        anim.SetInteger("state", 20);
                    }
                    if (goHorizontal == 0 && goVertical == 1)
                    {
                        anim.SetInteger("state", 21);
                    }
                    else if (goHorizontal == 0 && goVertical == -1)
                    {
                        anim.SetInteger("state", 21);
                    }
                }
                if (Input.GetMouseButtonDown(0))
                {
                    if (timeBtwAttac <= 0)
                    {
                        byte r = CalculateAngle();

                        switch (r)
                        {
                            case 1:
                                anim.SetInteger("state", 4);
                                break;
                            case 2:
                                anim.SetInteger("state", 9);
                                break;
                            case 3:
                                anim.SetInteger("state", 3);
                                break;
                            case 4:
                                anim.SetInteger("state", 25);
                                break;
                            default:
                                anim.SetInteger("state", 3);
                                break;
                        }
                    }
                    else
                    {
                        timeBtwAttac -= Time.deltaTime;
                    }
                }
                else if (timeBtwAttac > 0)
                {
                    timeBtwAttac -= Time.deltaTime;
                }
                break;

            case State.Rolling:
                float rollSpeedDropMultiplier = 0.005f;
                rollSpeed -= rollSpeed * rollSpeedDropMultiplier * Time.deltaTime;

                float rollSpeedMinimum = 20f;
                if (rollSpeed <= rollSpeedMinimum)
                {
                    state = State.Normal;
                    anim.SetInteger("state", 0);
                    attackable = true;
                }
                break;
        }
        //хп
        if (currentHP / currentMaxHP < 0.15f)
        {
            if (changeColorTime <= 0)
            {
                if (fillImage.color == color1)
                {
                    fillImage.color = color2;
                }
                else if (fillImage.color == color2)
                {
                    fillImage.color = color1;
                }
                changeColorTime = 10;
            }
            else
            {
                changeColorTime--;
            }
        }
        ChangeColor();
    }
    private void FixedUpdate()
    {
        //движение
        switch (state)
        {
            case State.Normal:
                if (goHorizontal < 0)
                {
                    anim.SetInteger("state", 1);
                }
                else if (goHorizontal > 0)
                {
                    anim.SetInteger("state", 2);
                }
                else if (goHorizontal == 0)
                {
                    anim.SetInteger("state", 0);
                }
                if (goHorizontal == 0 && goVertical == 1)
                {
                    anim.SetInteger("state", 7);
                }
                else if (goHorizontal == 0 && goVertical == -1)
                {
                    anim.SetInteger("state", 8);
                }

                moveDir = new Vector3(goHorizontal, goVertical).normalized;
                rb2d.velocity = moveDir * speed * Time.deltaTime;
                break;


            case State.Rolling:

                RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, moveDir, 0.01f, rollLayerMask);
                if (raycastHit2D.collider != null)
                {
                    state = State.Normal;

                    attackable = true;
                }

                rb2d.velocity = rollDir * rollSpeed;
                distanceTraveled += rollSpeed * Time.deltaTime;


                if (distanceTraveled >= maxRollDistance)
                {
                    state = State.Normal;
                    if (goHorizontal < 0)
                    {
                        anim.SetInteger("state", 10);
                    }
                    else if (goHorizontal > 0)
                    {
                        anim.SetInteger("state", 10);
                    }
                    else if (goHorizontal == 0)
                    {
                        anim.SetInteger("state", 10);
                    }
                    if (goHorizontal == 0 && goVertical == 1)
                    {
                        anim.SetInteger("state", 10);
                    }
                    else if (goHorizontal == 0 && goVertical == -1)
                    {
                        anim.SetInteger("state", 10);
                    }
                    attackable = true;
                }

                break;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }

    public void DamageBuff(float damaheBuff)
    {
        currentMaxDamage = MaxDamage * (1 + damaheBuff);
    }
    
    public void LvlUp()
    {
        MaxDamage = 2 * LevelGenerator.LVL;
        maxHP = 14 * LevelGenerator.LVL;
        currentMaxHP = maxHP;
        currentHP += 14f;
        currentDamage = currentMaxDamage = MaxDamage;
        if (currentHP > currentMaxHP)
        {
            currentHP = currentMaxHP;
        }
        DisplayHP();
    }
    public void Attack1()
    {
        //print(audioSources[0].clip);
        audioSources[0].Play();
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            if (enemiesToDamage[i].GetComponent<Person>() != null)
            {
                enemiesToDamage[i].GetComponent<Person>().TakingDamage(currentDamage*2);
            }
        }
    }

    public void SetStartTimeBtwAttac()
    {
        timeBtwAttac = startTimeBtwAttac;
    }

    private byte CalculateAngle()
    {
        Vector2 playerPos = transform.position;

        Vector2 dir = playerPos - mousePoint;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        if (angle >= -45 && angle <= 45)
        {
            return 1;
        }
        else if (angle >= 45 && angle <= 135)
        {
            return 2;
        }
        else if (angle >= 135 || angle <= -135)
        {
            return 3;
        }
        else if (angle >= -135 && angle <= -45)
        {
            return 4;
        }
        else return 1;
    }

    void OnGUI()
    {
        Event currentEvent = Event.current;
        Vector2 mousePos = new Vector2();

        // Get the mouse position from Event.
        // Note that the y position from Event is inverted.
        mousePos.x = currentEvent.mousePosition.x;
        mousePos.y = cam.pixelHeight - currentEvent.mousePosition.y;

        mousePoint = cam.ScreenToWorldPoint(new Vector2(mousePos.x, mousePos.y));

        //GUILayout.BeginArea(new Rect(20, 20, 250, 120));
        //GUILayout.Label("Screen pixels: " + cam.pixelWidth + ":" + cam.pixelHeight);
        //GUILayout.Label("Mouse position: " + mousePos);
        //GUILayout.Label("World position: " + mousePoint.ToString("F3"));
        //GUILayout.EndArea();
    }
    //хп
    public override void TakingDamage(float damage)
    {
        float takingDamage = damage / 1.75f;
        if (attackable)
        {
            currentHP = currentHP - takingDamage * currentDamageRatio;
            DisplayHP();
            isRed = true;
            redVariable = 1.5f;
            if (currentHP <= 0)
            {
                Dead();
            }
        }
    }

    private void Dead()
    {
        analytics.OnPlayerDead((int)LevelGenerator.LVL);
        SceneManager.LoadScene("OnLoseScene");
    }

    public void DisplayHP()
    {
        float HPSlider = currentHP / currentMaxHP;

        slider.value = HPSlider;
    }

    public void HPBuff(float hpBuff)
    {
        currentMaxHP = maxHP * (1 + hpBuff);
        DisplayHP();
    }

    public void RatioBuff(float ratioBuff)
    {
        currentMaxDamageRatio = maxDamageRatio * ratioBuff;
        if (currentHP > currentMaxHP)
        {
            currentHP = currentMaxHP;
        }
        DisplayHP();
    }

    public void SetNonBuffAnimation()
    {
        CenterAnim.SetInteger("state", 0);
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

    public bool AddBuff(Buff buff)
    {
        if (!buffs.Exists(x => x.GetType() == buff.GetType()))
        {
            buffs.Add(buff);
            return true;
        }
        return false;
    }

    private void HandleBuff()
    {
        foreach (Buff buff in buffs)
        {
            buff.Update();
        }
    }

    public bool RemoveBuff(Buff buff)
    {
        if (buffs.Exists(x => x.GetType() == buff.GetType()))
        {
            buffs.Remove(buff);
            return true;
        }
        return false;
    }
}
