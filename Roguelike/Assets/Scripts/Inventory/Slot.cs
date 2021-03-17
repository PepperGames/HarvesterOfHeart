using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{

    private Inventory inventory;

    public int number;

    public Transform temporarySlot;
    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }
    public void DropItem(int selectedSlot)
    {
        if (selectedSlot == 7)
        {
            if (inventory.isFull[7] == true)
            {
                foreach (Transform child in transform)
                {
                    string type = child.GetComponent<SpawnItem>().type;
                    if (type == "amulet")
                    {
                        child.GetComponent<Amulet>().DisableBuff();
                    }
                    print(child);
                    child.GetComponent<SpawnItem>().SpawnDroppedItem();
                    GameObject.Destroy(child.gameObject);
                    inventory.isFull[selectedSlot] = false;
                }
            }
        }
        else
        {
            foreach (Transform child in transform)
            {
                string type = child.GetComponent<SpawnItem>().type;
                if (type == "amulet")
                {
                    child.GetComponent<Amulet>().DisableBuff();
                }
                print(child);
                child.GetComponent<SpawnItem>().SpawnDroppedItem();
                GameObject.Destroy(child.gameObject);
                inventory.isFull[selectedSlot] = false;
            }
        }
        
        //inventory.GetTextInfo(selectedSlot);
    }
    public string GetInfo()
    {
        string info = "NONE";
        foreach (Transform child in transform)
        {
            info = child.GetComponent<ItemInfo>().ReturnString();
        }
        return info;
    }
    //предмет из 0-6 в 7
    public void PutOnItem(int selectedSlot)
    {
        print("зашли в пут итем");
        string type = "";
        foreach (Transform child in transform)
        {
            type = child.GetComponent<SpawnItem>().type;
        }
        if (type == "amulet")
        {
            print("Амулет");

            if (inventory.isFull[7] == false)
            {
                print("7 слот не занят");

                foreach (Transform child in transform)
                {
                    print($"тут что то должно быть {child.GetComponent<Amulet>()}");
                    child.GetComponent<Amulet>().ApplyBuff();
                    Instantiate(child, inventory.slots[7].transform, false);
                }
                inventory.isFull[selectedSlot] = false;
                inventory.isFull[7] = true;

                //foreach (Transform child in transform)
                //{
                //    if (child.CompareTag("BAmulet"))
                //    {
                //        Instantiate(child, inventory.slots[7].transform, false);
                //        AmuletBuff.SetBuff(0, 0.1f, 1);
                //        inventory.isFull[selectedSlot] = false;
                //        inventory.isFull[7] = true;
                //    }
                //    if (child.CompareTag("GAmulet"))
                //    {
                //        Instantiate(child, inventory.slots[7].transform, false);
                //        AmuletBuff.SetBuff(0.1f, 0, 1);
                //        inventory.isFull[selectedSlot] = false;
                //        inventory.isFull[7] = true;
                //    }
                //    if (child.CompareTag("YAmulet"))
                //    {
                //        Instantiate(child, inventory.slots[7].transform, false);
                //        AmuletBuff.SetBuff(0, 0, 0.91f);
                //        inventory.isFull[selectedSlot] = false;
                //        inventory.isFull[7] = true;
                //    }
                //}

                foreach (Transform child in transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
            else
            {
                SwapItem(selectedSlot);
            }
        }
        else if(type == "consumable")
        {

        }
    }

    public void PutOutItem(int selectedSlot)
    {
        print("начинаем выкидывать");
        for (int i = 0; i < inventory.slots.Length - 1; i++)
        {
            print("бежим по всеему инвентарю");

            if (inventory.isFull[i] == false)
            {
                print("встретили пустую ячейку");

                //добавляем
                
                foreach (Transform child in transform)
                {
                    print(child);
                    child.GetComponent<Amulet>().DisableBuff();
                    Instantiate(child, inventory.slots[i].transform, false);
                    GameObject.Destroy(child.gameObject);
                }
                inventory.isFull[selectedSlot] = false;
                inventory.isFull[i] = true;
                //AmuletBuff.SetBuff(0, 0, 1);

                break;
            }
        }
    }

    public void SwapItem(int selectedSlot)
    {
        if (selectedSlot != 7)
        {
            //с 7 перемещаем в буфер, 
            //основу перемещаем в 7
            //с буфера в начальній
            foreach (Transform child in inventory.slots[7].transform)
            {
                child.GetComponent<Amulet>().DisableBuff();
                Instantiate(child, temporarySlot, false);
                GameObject.Destroy(child.gameObject);
            }

            foreach (Transform child in transform)
            {
                Instantiate(child, inventory.slots[7].transform, false);
                child.GetComponent<Amulet>().ApplyBuff();
                GameObject.Destroy(child.gameObject);
            }

            foreach (Transform child in temporarySlot)
            {
                Instantiate(child, transform, false);
                GameObject.Destroy(child.gameObject);
            }
        }
    }
}
