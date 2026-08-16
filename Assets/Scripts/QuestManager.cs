using UnityEngine;


// Manages the player's active quest and handles
// checking and completing quests.
public class QuestManager : MonoBehaviour
{
    // Reference to the player's Inventory.
    // This is used to check whether the player has the
    // required quest item and to give/remove quest items.
    [SerializeField] private Inventory inventory;

    // Stores the quest that the player is currently working on.
    // If this is null, the player currently has no active quest.
    public Quest activeQuest;


    // Starts a new quest and makes it the player's active quest.
    public void StartQuest(Quest quest)
    {
        // Store the provided quest as the currently active quest.
        activeQuest = quest;

        // Display a message in the Unity Console showing
        // that the quest has started.
        Debug.Log("Quest Started: " + quest.questName);
    }


    // Checks whether the player's current quest can be completed.
    // Returns true if all requirements have been fulfilled.
    public bool CanCompleteQuest()
    {
        // If there is no active quest, the player cannot
        // complete a quest.
        if (activeQuest == null)
            return false;

        // If the active quest has already been completed,
        // prevent it from being completed again.
        if (activeQuest.questCompleted)
            return false;

        // Check two conditions:
        // 1. The player has the required item.
        // 2. The player has enough of that item.
        //
        // Both conditions must be true for the quest to be completed.
        return inventory.HasItem(activeQuest.requiredItem)
               && GetItemQuantity(activeQuest.requiredItem) >= activeQuest.requiredAmount;
    }


    // Completes the currently active quest if all of its
    // requirements have been fulfilled.
    public void CompleteQuest()
    {
        // Check whether the quest is actually ready to be completed.
        // If it is not, stop the method immediately.
        if (!CanCompleteQuest())
            return;

        // Remove the required amount of the quest item
        // from the player's inventory.
        inventory.RemoveItem(
            activeQuest.requiredItem,
            activeQuest.requiredAmount
        );

        // Add the quest reward and its specified quantity
        // to the player's inventory.
        inventory.AddItem(
            activeQuest.rewardItem,
            activeQuest.rewardAmount
        );

        // Mark the quest as completed so it cannot
        // be completed repeatedly.
        activeQuest.questCompleted = true;

        // Display a message in the Unity Console confirming
        // that the quest has been completed.
        Debug.Log("Quest Completed: " + activeQuest.questName);
    }


    // Searches through the player's inventory to find
    // the quantity of a specific item.
    private int GetItemQuantity(string itemName)
    {
        // Go through every Item currently stored in the inventory.
        foreach (Item item in inventory.items)
        {
            // Check whether the current item's name matches
            // the item name being searched for.
            if (item.itemName == itemName)
            {
                // Return the quantity of the matching item.
                return item.quantity;
            }
        }

        // If the item cannot be found in the inventory,
        // return 0 to indicate that the player has none.
        return 0;
    }
}