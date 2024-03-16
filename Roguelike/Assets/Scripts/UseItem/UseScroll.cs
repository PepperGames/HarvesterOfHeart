using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseScroll : MonoBehaviour
{ 
    public void Use()
    {
        int selectedSlot = gameObject.GetComponentInParent<Slot>().number;
        gameObject.GetComponent<Scroll>().ApplyBuff();
    }
    public void Drop()
    {
        if (Input.GetMouseButtonDown(1))
        {
            int selectedSlot = gameObject.GetComponentInParent<Slot>().number;
            selectedSlot = gameObject.GetComponentInParent<Slot>().number;
            print(selectedSlot);
            gameObject.GetComponentInParent<Slot>().DropItem(selectedSlot);
        }
        else
        {
            Use();
        }
    }
}


