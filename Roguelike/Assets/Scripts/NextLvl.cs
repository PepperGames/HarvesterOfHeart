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
