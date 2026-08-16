using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public List<Item> items = new List<Item>();

    public void AddItem(string itemName, int amount)
    {
        foreach (Item item in items)
        {
            if (item.itemName == itemName)
            {
                item.quantity += amount;
                return;
            }
        }

        Item newItem = new Item();
        newItem.itemName = itemName;
        newItem.quantity = amount;

        items.Add(newItem);
    }

    public bool HasItem(string itemName)
    {
        foreach (Item item in items)
        {
            if (item.itemName == itemName && item.quantity > 0)
            {
                return true;
            }
        }

        return false;
    }

    public void RemoveItem(string itemName, int amount)
    {
        foreach (Item item in items)
        {
            if (item.itemName == itemName)
            {
                item.quantity -= amount;

                if (item.quantity <= 0)
                {
                    items.Remove(item);
                }

                return;
            }
        }
    }
}