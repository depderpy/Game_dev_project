using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private Inventory inventory;

    public Quest activeQuest;

    public void StartQuest(Quest quest)
    {
        activeQuest = quest;

        Debug.Log("Quest Started: " + quest.questName);
    }

    public bool CanCompleteQuest()
    {
        if (activeQuest == null)
            return false;

        if (activeQuest.questCompleted)
            return false;

        return inventory.HasItem(activeQuest.requiredItem)
               && GetItemQuantity(activeQuest.requiredItem) >= activeQuest.requiredAmount;
    }

    public void CompleteQuest()
    {
        if (!CanCompleteQuest())
            return;

        inventory.RemoveItem(
            activeQuest.requiredItem,
            activeQuest.requiredAmount
        );

        inventory.AddItem(
            activeQuest.rewardItem,
            activeQuest.rewardAmount
        );

        activeQuest.questCompleted = true;

        Debug.Log("Quest Completed: " + activeQuest.questName);
    }

    private int GetItemQuantity(string itemName)
    {
        foreach (Item item in inventory.items)
        {
            if (item.itemName == itemName)
            {
                return item.quantity;
            }
        }

        return 0;
    }
}