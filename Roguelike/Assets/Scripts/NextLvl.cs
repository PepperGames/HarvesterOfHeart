using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLvl : MonoBehaviour
{
    public LayerMask whatIsPlayer;
    public GameObject levelGenerator;
    public float w;
    public float h;
    public int count = 0;
    void Update()
    {
        if (Input.GetKeyDown("f"))
        {
            Restart();
        }
    }

    public void Restart()
    {
        Collider2D Player = Physics2D.OverlapBox(transform.position, new Vector2(w, h), 0, whatIsPlayer);
        if (Player != null)
        {
            LevelGenerator.LVL++;
            foreach (Transform child in levelGenerator.transform)
            {
                count++;
                Destroy((child as Transform).gameObject);
            }
            //print(count);
            //var objs = GameObject.FindGameObjectsWithTag("wall"); // возвращает МАССИВ!
            //for (int i = 0; i < objs.Length; i++)
            //    Destroy(objs[i]);

            //var objs2 = GameObject.FindGameObjectsWithTag("floor"); // возвращает МАССИВ!
            //for (int i = 0; i < objs2.Length; i++)
            //    Destroy(objs2[i]);

            //var objs3 = GameObject.FindGameObjectsWithTag("Portal"); // возвращает МАССИВ!
            //for (int i = 0; i < objs3.Length; i++)
            //    Destroy(objs3[i]);

            //var objs4 = GameObject.FindGameObjectsWithTag("HealthPotionItem"); // возвращает МАССИВ!
            //for (int i = 0; i < objs4.Length; i++)
            //    Destroy(objs4[i]);

            //var objs5 = GameObject.FindGameObjectsWithTag("ScrollItem"); // возвращает МАССИВ!
            //for (int i = 0; i < objs5.Length; i++)
            //    Destroy(objs5[i]);

            //var objs6 = GameObject.FindGameObjectsWithTag("SoulItem"); // возвращает МАССИВ!
            //for (int i = 0; i < objs6.Length; i++)
            //    Destroy(objs6[i]);

            //objs2 = GameObject.FindGameObjectsWithTag("floor"); // возвращает МАССИВ!
            //for (int i = 0; i < objs2.Length; i++)
            //    Destroy(objs2[i]);
            ////убрать


            //objs2 = GameObject.FindGameObjectsWithTag("AmuletItem"); // возвращает МАССИВ!
            //for (int i = 0; i < objs2.Length; i++)
            //    Destroy(objs2[i]);

            //objs2 = GameObject.FindGameObjectsWithTag("Menoreh"); // возвращает МАССИВ!
            //for (int i = 0; i < objs2.Length; i++)
            //    Destroy(objs2[i]);
            var objs2 = GameObject.FindGameObjectsWithTag("Enemy"); // возвращает МАССИВ!
            for (int i = 0; i < objs2.Length; i++)
                Destroy(objs2[i]);
            Player pl = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            pl.LvlUp();

            //GameObject go = GameObject.FindGameObjectWithTag("levelGenerator");
            levelGenerator.GetComponent<LevelGenerator>().RestartLVL();

            if(LevelGenerator.LVL >= 10)
                SceneManager.LoadScene("OnWinScene");

            Destroy(gameObject);
        }

    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector2(w, h));
    }
}
