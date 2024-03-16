using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseHPPotion : MonoBehaviour
{
    public void Use()
    {
        int selectedSlot = gameObject.GetComponentInParent<Slot>().number;
        gameObject.GetComponent<Heart>().ApplyBuff();
    }
    public void Drop()
    {
        if (Input.GetMouseButtonDown(1))
        {
            int selectedSlot = gameObject.GetComponentInParent<Slot>().number;
            print(selectedSlot); selectedSlot = gameObject.GetComponentInParent<Slot>().number;
            gameObject.GetComponentInParent<Slot>().DropItem(selectedSlot);
        }
        else
        {
            Use();
        }
    }
}


