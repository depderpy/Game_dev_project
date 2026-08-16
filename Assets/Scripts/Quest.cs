using UnityEngine;


[System.Serializable]
public class Quest
{
    public string questName;

    public string requiredItem;
    public int requiredAmount;

    public string rewardItem;
    public int rewardAmount;

    public bool questCompleted;
}
