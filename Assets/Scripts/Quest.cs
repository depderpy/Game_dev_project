using UnityEngine;


// Allows this class to be serialized by Unity so that
// Quest objects can be created and edited through the Inspector.
[System.Serializable]
public class Quest
{
    // The name of the quest.
    // This is used to identify the quest to the player and in Debug.Log messages.
    public string questName;


    // The name of the item that the player needs to collect
    // in order to complete the quest.
    public string requiredItem;

    // The number of the required item that the player must have
    // before the quest can be completed.
    public int requiredAmount;


    // The name of the item that the player receives as a reward
    // after successfully completing the quest.
    public string rewardItem;

    // The amount of the reward item that the player receives
    // when the quest is completed.
    public int rewardAmount;


    // Keeps track of whether the quest has already been completed.
    // false = quest has not been completed.
    // true = quest has been completed.
    public bool questCompleted;
}