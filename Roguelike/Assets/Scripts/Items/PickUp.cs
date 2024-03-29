﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    private Inventory inventory;

    public GameObject itemButton;

    public string type;

    public AudioClip[] clips;
    AudioSource audioSource;
    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        StartCoroutine(Drop());
        audioSource = GetComponent<AudioSource>();
    }


    public void OnTriggerStay2D(Collider2D other) //PickUpToSlot OnTriggerEnter2D
    {
        if (Input.GetKeyUp("e"))
        {
            if (other.CompareTag("Player"))
            {
                audioSource.clip = clips[0];
                //print(audioSource.clip);
                audioSource.Play();
                if (type == "consumable")
                {
                    for (int i = 0; i < inventory.slots.Length - 1; i++)
                    {
                        if (inventory.isFull[i] == false)
                        {
                            //добавляем
                            inventory.isFull[i] = true;
                            Instantiate(itemButton, inventory.slots[i].transform, false);
                            Destroy(gameObject);
                            break;
                        }
                    }
                }
                else if (type == "amulet")
                {
                    if (inventory.isFull[7] == false)
                    {
                        inventory.isFull[7] = true;
                        Instantiate(itemButton, inventory.slots[7].transform, false);
                        if (inventory.selectedSlot == 7)
                            //inventory.GetTextInfo(7);

                            print("создаю в 7 слоте только что поднятый амулет");
                            inventory.slots[7].GetComponent<Slot>().PutOnItem(7);
                        foreach (Transform child in inventory.slots[7].transform)
                        {
                            child.GetComponent<Amulet>().ApplyBuff();

                            print(child);
                            Destroy(gameObject);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < inventory.slots.Length - 1; i++)
                        {
                            if (inventory.isFull[i] == false)
                            {
                                //добавляем
                                inventory.isFull[i] = true;
                                Instantiate(itemButton, inventory.slots[i].transform, false);
                                Destroy(gameObject);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    IEnumerator Drop()
    {
        var position = transform.position;
        for (float n = 2.9f; n <= 3.2f; n += 0.005f)
        {
            if (n >= 3)
            {
                position.x = transform.position.x + 0.01f;
                position.y = transform.position.y - (-Mathf.Pow((n - 3), 2) + 9) / 500;
                transform.position = position;
            }
            else
            {
                position.y = transform.position.y + 0.05f;
                transform.position = position;
            }

            yield return null;
        }

    }
}

    
