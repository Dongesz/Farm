using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEditor.Search;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject slotHolder;
    [SerializeField] private ItemClass itemToAdd;
    [SerializeField] private ItemClass itemToRemove;

    public List<SlotClass> Inventory = new List<SlotClass>();

    private GameObject[] slots;

    private void Start()
    {
        slots = new GameObject[slotHolder.transform.childCount];
        for (int i = 0; i < slotHolder.transform.childCount; i++)
            slots[i] = slotHolder.transform.GetChild(i).gameObject;
        RefreshUI();
        add(itemToAdd);
        remove(itemToRemove);
    }

    public void RefreshUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            try
            {
                slots[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                slots[i].transform.GetChild(0).GetComponent<Image>().sprite = Inventory[i].GetItem().itemIcon;
                if (Inventory[i].GetItem().isStackable)
                    slots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Inventory[i].GetQuantity() + "";
                else
                    slots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
            }
            catch 
            {
                slots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                slots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
            }
        }
    }
    public bool add(ItemClass item)
    {

        SlotClass slot = Contains(item);
        if (slot != null && slot.GetItem().isStackable)
        {
            slot.AddQuantity(1);
        }
        else
        {
            if (Inventory.Count < slots.Length)
                 Inventory.Add(new SlotClass(item, 1));
            else
                return false;
        }
        RefreshUI();
        return true;
    }
    public bool remove(ItemClass item)
    {
        SlotClass temp = Contains(item);
        if (temp != null)
        {
            if (temp.GetQuantity() >= 1)
                 temp.SubQuantity(1);
            else
            {
                SlotClass slotToRemove = new SlotClass();
                foreach (SlotClass slot in Inventory)
                {
                    if (slot.GetItem() == item)
                    {
                        slotToRemove = slot;
                        break;
                    }
                }
                Inventory.Remove(slotToRemove);
            }
        }  
        else
        {
            return false;
        }
            
        RefreshUI();
        return true;
    }

    public SlotClass Contains(ItemClass item)
    {
        foreach(SlotClass slot in Inventory)
        {
            if (slot.GetItem() == item)
            {
                return slot;
            }
        }
        return null;

    }
}
