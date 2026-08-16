using UnityEngine;


[System.Serializable]
public class Quest
{
    public string questName;
    
    public int requireAmount;
    public int currentAmount;

    public bool isActive;
    public bool isComplete;

    public void StartQuest()
    {
        isActive = true;
        isComplete = false;

        Debug.Log("Quest Started: " + questName);
    }

    public void AddProgress(int amount)
    {
        if(!isActive || isComplete)
        {
            return;

            currentAmount += amount;
        }

        if(currentAmount >= requireAmount)
        {
            currentAmount = requireAmount;
            isComplete = true;

            Debug.Log("Quest Completed: " + questName);
        }
    }
}
