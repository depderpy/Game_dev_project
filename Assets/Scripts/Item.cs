using UnityEngine;

// Allows the Item class to be serialized by Unity.
// This means Item objects and their public variables can be displayed
// and edited in the Unity Inspector when used by another serialized class.
[System.Serializable] 
public class Item 
{
    // Stores the name of the item.
    // Examples: "Potion", "Meat", "Wolf Fur".
    public string itemName;

    // Stores how many of this item the player currently has.
    // For example, quantity = 3 means the player has 3 of this item.
    public int quantity;
}