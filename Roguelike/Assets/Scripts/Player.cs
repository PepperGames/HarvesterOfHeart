using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Player : MonoBehaviour
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
    public float timeForSoul;

    public float MaxDamage; //максимальный урон без шмотки
    public float currentMaxDamage; //максимальный урон под бафом шмотки
    public float currentDamage;//текущее

    private Inventory inventory;
    private float timeBtwAttac = 0;
    private float timeBtwAttacForAttack2 = 0;
    int attackCount = 0;

    private Camera cam;

    Vector2 mousePoint;

    public GameObject Center;
    private Animator CenterAnim;
    private bool isPlayed = false;

    //хп
    public float maxHP;//максимальное хп без шмотки
    public float currentMaxHP;//максимальное хп под бафом шмотки
    public float currentHP;//текущее

    public float maxDamageRatio;//максимальный коеф без шмотки    
    public float currentMaxDamageRatio;//текущий под бафом шмотки
    public float currentDamageRatio;//текущий коеф дамага


    public Slider slider;
    public Image fillImage;
    public float timeForScroll;

    private Color color1;
    private Color color2;
    private int changeColorTime = 5;

    public bool attackable;

    private SpriteRenderer spriteRenderer;
    private bool isRed;
    private float redVariable;
    private bool isPlayedHP = false;
    private bool isPlayedScroll = false;

    public AudioSource[] audioSources;

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
        maxHP = 12f;
        currentHP = currentMaxHP = maxHP;
        maxDamageRatio = currentMaxDamageRatio = 1;
        DisplayHP();
        color1 = new Color(255, 255, 255, 1f);
        color2 = new Color(255, 255, 255, 0f);
        fillImage.color = color1;
        attackable = true;
        CenterAnim = Center.GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        isRed = false;
    }

    void Update()
    {
        DisplayHP();
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
        //атака
        if (Input.GetKeyUp("2"))
        {
            UseSoul();
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
        if (Input.GetKeyUp("1"))
        {
            UseHPPotion();
        }
        if (Input.GetKeyUp("3"))
        {
            UseScroll();
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

        //атака
        if (timeForSoul > 0f)
        {
            timeForSoul -= Time.deltaTime;
            CenterAnim.SetInteger("state", 2);
            if (!isPlayed)
            {
                //audioSource.clip = clips[0];
                //print(audioSource.clip);
                //audioSource.Play();
                //isPlayed = true;
            }
        }
        else
        {
            currentDamage = currentMaxDamage;
            if (timeForSoul <= 0 && GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHP>().timeForScroll <= 0)
                CenterAnim.SetInteger("state", 0);

        }
        //хп
        if (timeForScroll > 0f)
        {
            timeForScroll -= Time.deltaTime;
            CenterAnim.SetInteger("state", 3);
            if (!isPlayedScroll)
            {
                //audioSource.clip = clips[1];
                //print(audioSource.clip);
                //audioSource.Play();
                //isPlayedScroll = true;
            }
        }
        else
        {
            currentDamageRatio = currentMaxDamageRatio;
            if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAttack>().timeForSoul <= 0 && timeForScroll <= 0)
                CenterAnim.SetInteger("state", 0);
        }
        if (!isPlayedHP)
        {
            //audioSource.clip = clips[0];
            //print(audioSource.clip);
            //audioSource.Play();
            //isPlayedHP = true;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }

    //public void UseScroll()
    //{
    //    for (int i = 0; i < inventory.slots.Length; i++) //inventory.slots.Length
    //    {
    //        if (inventory.slots[i].transform.childCount > 0)
    //        {
    //            if (inventory.slots[i].transform.GetChild(0).CompareTag("Scroll"))
    //            {
    //                currentDamage = currentMaxDamage * 1.25f;
    //                timeForScroll = 15f;
    //                inventory.isFull[i] = false;
    //                foreach (Transform t in inventory.slots[i].transform)
    //                {
    //                    Destroy(t.gameObject);
    //                }
    //                break;
    //            }
    //        }
    //        else
    //        {
    //            continue;
    //        }

    //    }
    //}
    public void UseSoul()
    {
        if (timeForSoul <= 0 && GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHP>().timeForScroll <= 0)
        {
            for (int i = 0; i < inventory.slots.Length; i++) //inventory.slots.Length
            {
                if (inventory.slots[i].transform.childCount > 0)
                {
                    if (inventory.slots[i].transform.GetChild(0).CompareTag("Soul"))
                    {
                        currentDamage = currentMaxDamage * 1.25f;
                        timeForSoul = 15f;
                        CenterAnim.SetInteger("state", 2);
                        inventory.isFull[i] = false;
                        isPlayed = false;
                        foreach (Transform t in inventory.slots[i].transform)
                        {
                            Destroy(t.gameObject);
                        }
                        break;
                    }
                }
                else
                {
                    continue;
                }

            }
        }
    }
    public void UseSoul(int selSlot)
    {
        if (timeForSoul <= 0 && GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHP>().timeForScroll <= 0)
        {
            if (inventory.slots[selSlot].transform.childCount > 0)
            {
                if (inventory.slots[selSlot].transform.GetChild(0).CompareTag("Soul"))
                {
                    currentDamage = currentMaxDamage * 1.25f;
                    timeForSoul = 15f;
                    CenterAnim.SetInteger("state", 2);
                    inventory.isFull[selSlot] = false;
                    isPlayed = false;
                    foreach (Transform t in inventory.slots[selSlot].transform)
                    {
                        Destroy(t.gameObject);
                    }
                }
            }
        }
    }

    public void DamageBuff(float damaheBuff)
    {
        currentMaxDamage = MaxDamage * (1 + damaheBuff);
    }
    public void LvlDamageUp()
    {
        {
            bool Is = false;
            if (currentMaxDamage > MaxDamage)
                Is = true;
            MaxDamage = 2 * LevelGenerator.LVL;
            currentMaxDamage = MaxDamage;
            //тут еще что то нужно, типо когда на некст лвл переход, и есть активная шмотка, то и карентМакс нужно увеличить
            if (Is)
            {
                AmuletBuff.SetBuff(0, 0.1f, 1);
            }
        }
    }

    public void Attack1()
    {
        print(audioSources[0].clip);
        audioSources[0].Play();
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            if (enemiesToDamage[i].GetComponent<Enemy>() != null)
            {
                enemiesToDamage[i].GetComponent<Enemy>().TakeDamage(currentDamage);
            }
            else if (enemiesToDamage[i].GetComponent<SmallEnemy>() != null)
            {
                enemiesToDamage[i].GetComponent<SmallEnemy>().TakeDamage(currentDamage);
            }
            else if (enemiesToDamage[i].GetComponent<Boss1Enemy>() != null)
            {
                enemiesToDamage[i].GetComponent<Boss1Enemy>().TakeDamage(currentDamage);
            }
            else if (enemiesToDamage[i].GetComponent<Boss2Enemy>() != null)
            {
                enemiesToDamage[i].GetComponent<Boss2Enemy>().TakeDamage(currentDamage);
            }
            else if (enemiesToDamage[i].GetComponent<Boss3Enemy>() != null)
            {
                enemiesToDamage[i].GetComponent<Boss3Enemy>().TakeDamage(currentDamage);
            }
        }
    }

    public void SetStartTimeBtwAttac()
    {
        timeBtwAttac = startTimeBtwAttac;
    }


    //public void CheckStartTimeBtwAttac2()
    //{
    //    if (timeBtwAttacForAttack2 <= startTimeBtwAttac + 1)
    //    {
    //        attackCount++;
    //        timeBtwAttacForAttack2 = 0;
    //    }
    //    else
    //    {
    //        attackCount = 0;
    //        timeBtwAttacForAttack2 = 0;
    //    }
    //}
    //public void SetStartTimeBtwAttac2()
    //{
    //    attackCount = 0;
    //    timeBtwAttacForAttack2 = 0;
    //}
    private byte CalculateAngle()
    {
        Vector2 playerPos = transform.position;

        Vector2 dir = playerPos - (Vector2)mousePoint;

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
    //Screen.width / 2 - Input.mousePosition.x и Screen.height / 2 - Input.mousePosition.y


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
    public void TakingDamage(float damage)
    {
        print(attackable);
        if (attackable)
        {
            currentHP = currentHP - damage * currentDamageRatio;
            DisplayHP();
            isRed = true;
            print(isRed);
            redVariable = 1.5f;
            print(redVariable);
            if (currentHP <= 0)
            {
                SceneManager.LoadScene("OnLoseScene");
            }
        }
    }

    //восполнение хп
    public void UseHPPotion()
    {
        for (int i = 0; i < inventory.slots.Length; i++) //inventory.slots.Length
        {
            if (inventory.slots[i].transform.childCount > 0)
            {
                if (inventory.slots[i].transform.GetChild(0).CompareTag("HealthPotion"))
                {
                    currentHP += maxHP * 0.05f;
                    CenterAnim.SetInteger("state", 1);
                    if (currentHP > currentMaxHP)
                    {
                        currentHP = currentMaxHP;
                    }
                    isPlayedHP = false;
                    DisplayHP();
                    inventory.isFull[i] = false;
                    foreach (Transform t in inventory.slots[i].transform)
                    {
                        Destroy(t.gameObject);
                    }
                    break;
                }

            }
            else
            {
                continue;
            }

        }
        DisplayHP();
    }
    public void UseHPPotion(int selSlot)
    {
        if (inventory.slots[selSlot].transform.childCount > 0)
        {
            if (inventory.slots[selSlot].transform.GetChild(0).CompareTag("HealthPotion"))
            {
                currentHP += maxHP * 0.05f;
                CenterAnim.SetInteger("state", 1);
                if (currentHP > currentMaxHP)
                {
                    currentHP = currentMaxHP;
                }
                isPlayedHP = false;
                DisplayHP();
                inventory.isFull[selSlot] = false;
                foreach (Transform t in inventory.slots[selSlot].transform)
                {
                    Destroy(t.gameObject);
                }
            }
        }
        DisplayHP();
    }
    //public void UseHPPotionOnButton()
    //{
    //    GameObject parent = transform.parent.gameObject;
    //    if (parent.CompareTag("Slot"))
    //    {
    //        int n = parent.GetComponent<Slot>().number;
    //        UseHPPotion();
    //        inventory.isFull[n] = false;
    //        foreach (Transform t in inventory.slots[n].transform)
    //        {
    //            Destroy(t.gameObject);
    //        }
    //    }


    //    DisplayHP();
    //}

    public void UseScroll()
    {
        if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAttack>().timeForSoul <= 0 && timeForScroll <= 0)
        {
            for (int i = 0; i < inventory.slots.Length; i++) //inventory.slots.Length
            {
                if (inventory.slots[i].transform.childCount > 0)
                {
                    if (inventory.slots[i].transform.GetChild(0).CompareTag("Scroll"))
                    {
                        currentDamageRatio = currentMaxDamageRatio * 0.75f;
                        timeForScroll = 15f;
                        CenterAnim.SetInteger("state", 3);
                        inventory.isFull[i] = false;
                        isPlayedScroll = false;
                        foreach (Transform t in inventory.slots[i].transform)
                        {
                            Destroy(t.gameObject);
                        }
                        break;
                    }
                }
                else
                {
                    continue;
                }

            }
        }
    }

    public void UseScroll(int selSlot)
    {
        if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAttack>().timeForSoul <= 0 && timeForScroll <= 0)
        {
            if (inventory.slots[selSlot].transform.childCount > 0)
            {
                if (inventory.slots[selSlot].transform.GetChild(0).CompareTag("Scroll"))
                {
                    currentDamageRatio = currentMaxDamageRatio * 0.75f;
                    timeForScroll = 15f;
                    CenterAnim.SetInteger("state", 3);
                    isPlayedScroll = false;
                    inventory.isFull[selSlot] = false;
                    foreach (Transform t in inventory.slots[selSlot].transform)
                    {
                        Destroy(t.gameObject);
                    }
                }
            }
        }
    }


    private void DisplayHP()
    {
        float HPSlider = currentHP / currentMaxHP;

        if (HPSlider < 100)
        {
            if (HPSlider > 0.51f && HPSlider < 0.90f)
            {
                slider.value = HPSlider * 0.7f;
            }
            else if (HPSlider > 0.11f && HPSlider < 0.50f)
            {
                slider.value = HPSlider * 0.6f;
            }
            else if (HPSlider > 0.2f && HPSlider < 0.10f)
            {
                slider.value = HPSlider * 0.5f;
            }
            else
            {
                slider.value = HPSlider;
            }
        }
        else
        {
            slider.value = HPSlider;
        }

        //if(HPSlider > 0.10f)
        //{
        //    //fillImage.color = color1;
        //    slider.value = HPSlider;
        //}

        //else if (HPSlider <= 0.10f)
        //{
        //    if (HPSlider < 0.05f)
        //    {
        //        slider.value = HPSlider;
        //    }
        //    else slider.value = 0.05f;
        //}


    }

    public void LvlHPUp()
    {
        bool Is = false;
        if (currentMaxHP > maxHP)
            Is = true;
        maxHP = 16 * LevelGenerator.LVL;
        //тут еще что то нужно, типо когда на некст лвл переход, и есть активная шмотка, то и карентМакс нужно увеличить
        //пошаманить потом
        currentMaxHP = maxHP;
        if (Is)
        {
            AmuletBuff.SetBuff(0.1f, 0, 1f);
        }
        currentHP += 12f;
        if (currentHP > currentMaxHP)
        {
            currentHP = currentMaxHP;
        }
        DisplayHP();
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
}
