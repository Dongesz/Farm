using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEditor.Search;
using TMPro;
using static UnityEditor.Progress;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject slotHolder;
    [SerializeField] private GameObject itemCursor;
    [SerializeField] private ItemClass itemToAdd;
    [SerializeField] private ItemClass itemToRemove;

    [SerializeField] private SlotClass[] startingItems;
    private SlotClass[] Inventory;

    private GameObject[] slots;

    private SlotClass movingSlot;

    private SlotClass tempSlot;

    private SlotClass originalSlot;

    private bool isMovingItem;


    private void Start()
    {
        slots = new GameObject[slotHolder.transform.childCount];
        Inventory = new SlotClass[slots.Length];
        for (int i = 0; i < Inventory.Length; i++)
        {
            Inventory[i] = new SlotClass();
        }
        for (int i = 0; i < startingItems.Length; i++)
        {
            Inventory[i] = startingItems[i];
        }
        for (int i = 0; i < slotHolder.transform.childCount; i++)
            slots[i] = slotHolder.transform.GetChild(i).gameObject;
        RefreshUI();
        add(itemToAdd,1);
        remove(itemToRemove);
    }

    private void Update()
    {
        itemCursor.SetActive(isMovingItem);
        itemCursor.transform.position = Input.mousePosition;
        if (isMovingItem )
        {
            itemCursor.GetComponent<Image>().sprite = movingSlot.GetItem().itemIcon;
        }
        if(Input.GetMouseButtonDown(0))
        {
            if (isMovingItem)
            {
                EndItemMove();
            }
            else
                BeginItemMove();
        }
    }

    #region Inventory
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
                slots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
            }
        }
    }
    public bool add(ItemClass item, int quantity)
    {

        SlotClass slot = Contains(item);
        if (slot != null && slot.GetItem().isStackable)
        {
            slot.AddQuantity(1);
        }
        else
        {
            for(int i = 0;i < Inventory.Length; i++)
            {
                if (Inventory[i].GetItem() == null)
                {
                    Inventory[i].AddItem(item, quantity);
                    break;
                }
                   

            }
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
                int slotToRemoveIndex = 0;
                for(int i = 0; i < Inventory.Length; i++)
                {
                    if (Inventory[i].GetItem() == item)
                    {
                        slotToRemoveIndex = i;
                        break;
                    }
                }
                Inventory[slotToRemoveIndex].Clear();
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
        for (int i = 0; i < Inventory.Length; i++)
        {
            if (Inventory[i].GetItem() == item)
                return Inventory[i];
        }
        return null;

    }
    #endregion

    #region Moveing Stuff
    private bool BeginItemMove()
    {
        originalSlot = ClosestSlot();
        if (originalSlot == null || originalSlot.GetItem() == null)
            return false;
        movingSlot = new SlotClass(originalSlot);
        originalSlot.Clear();
        isMovingItem = true;
        RefreshUI();
        return true;
    }
    private bool EndItemMove()
    {
        originalSlot = ClosestSlot();
        if (originalSlot == null)
        {
            add(movingSlot.GetItem(), movingSlot.GetQuantity());
            movingSlot.Clear();
        }
        else
        {
            if (originalSlot.GetItem() != null)
            {
                if (originalSlot.GetItem() == movingSlot.GetItem())
                {
                    if (originalSlot.GetItem().isStackable)
                    {
                        originalSlot.AddQuantity(movingSlot.GetQuantity());
                        movingSlot.Clear();
                    }
                    else return false;
                }
                else
                {
                    tempSlot = new SlotClass(originalSlot);
                    originalSlot.AddItem(movingSlot.GetItem(), movingSlot.GetQuantity());
                    movingSlot.AddItem(tempSlot.GetItem(), tempSlot.GetQuantity());
                    RefreshUI();
                    return true;
                }
            }
            else
            {
                originalSlot.AddItem(movingSlot.GetItem(), movingSlot.GetQuantity());
                movingSlot.Clear();
            }
        }
        isMovingItem = false;
        RefreshUI();
        return true;


    }
    private SlotClass ClosestSlot()
    {

        for (int i = 0; i < slots.Length; i++)
        {
            if (Vector2.Distance(slots[i].transform.position, Input.mousePosition) <= 32)
                return Inventory[i];
        }
        return null;
    }
    #endregion
}
