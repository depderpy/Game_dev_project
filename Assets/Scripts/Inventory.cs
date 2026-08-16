using UnityEngine;
using System.Collections.Generic;

// Manages the player's collection of items.
//
// This script stores all items currently owned by the player
// and provides methods for adding, checking, and removing items.
//
// Because this inherits from MonoBehaviour, it can be attached
// to a GameObject in Unity and referenced by other scripts such
// as the GameManager and BattleManager.
public class Inventory : MonoBehaviour
{
    // Stores all of the items currently in the player's inventory.
    //
    // Each Item in this list contains information such as:
    // - The item's name
    // - The quantity of that item
    //
    // A List is used so that the inventory can contain
    // multiple different types of items.
    public List<Item> items = new List<Item>();


    // Adds a specified amount of an item to the inventory.
    //
    // itemName = the name of the item being added.
    // amount = how many of that item should be added.
    //
    // If the player already has the item, its quantity is increased.
    // If the player does not have the item, a new Item is created.
    public void AddItem(string itemName, int amount)
    {
        // Loops through every item currently stored in the inventory.
        foreach (Item item in items)
        {
            // Checks whether the current item has the same name
            // as the item that is being added.
            if (item.itemName == itemName)
            {
                // The item already exists in the inventory,
                // so increase its quantity by the specified amount.
                item.quantity += amount;

                // Stops the method because the item has already
                // been successfully added.
                return;
            }
        }

        // This code is reached if the item was not found
        // in the existing inventory.
        //
        // Creates a new Item object to represent the new item.
        Item newItem = new Item();

        // Assigns the requested item name to the new item.
        newItem.itemName = itemName;

        // Sets the initial quantity of the new item.
        newItem.quantity = amount;

        // Adds the newly created item to the inventory list.
        items.Add(newItem);
    }


    // Checks whether the player currently owns a particular item.
    //
    // Returns true if the item exists and has a quantity greater than 0.
    // Returns false if the item does not exist or has no remaining quantity.
    public bool HasItem(string itemName)
    {
        // Loops through every item currently stored in the inventory.
        foreach (Item item in items)
        {
            // Checks two conditions:
            //
            // 1. The item's name matches the requested item name.
            // 2. The item's quantity is greater than 0.
            //
            // Both conditions must be true for the player to
            // be considered to have the item.
            if (item.itemName == itemName && item.quantity > 0)
            {
                // The requested item was found and is available.
                return true;
            }
        }

        // If the loop finishes without finding a valid item,
        // the player does not currently have the requested item.
        return false;
    }


    // Removes a specified amount of an item from the inventory.
    //
    // itemName = the name of the item being removed.
    // amount = how many of that item should be removed.
    //
    // If the quantity reaches 0 or below after removing the item,
    // the item itself is removed from the inventory list.
    public void RemoveItem(string itemName, int amount)
    {
        // Loops through every item currently stored in the inventory.
        foreach (Item item in items)
        {
            // Checks whether the current item matches
            // the item that should be removed.
            if (item.itemName == itemName)
            {
                // Subtracts the requested amount from the item's quantity.
                item.quantity -= amount;

                // Checks whether there are no more of this item remaining.
                if (item.quantity <= 0)
                {
                    // Removes the item completely from the inventory
                    // because its quantity has reached 0 or below.
                    items.Remove(item);
                }

                // Stops the method after the item has been processed.
                return;
            }
        }
    }
}