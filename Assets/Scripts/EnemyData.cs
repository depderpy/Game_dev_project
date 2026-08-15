using UnityEngine;
using System.Collections.Generic;

public class EnemyData : MonoBehaviour
{
    public string enemyName;
    public Combatant combatant;
    public GameObject battlesprite;
    public bool acceptMeat;
    public List <AlternateEnding> alternativeEnding;

    public List<Item> drops;

    private void Start()
    {
        AlternateEnding ending = GetalternativeEndings("Meat");

        if(ending != null)
        {
            Debug.Log("Found ending");
            Debug.Log(ending.EndMessage);
        }

        else
        {
            Debug.Log("No Alternate ending found.");
        }
    }

    public AlternateEnding GetalternativeEndings(string itemName)
    {
        foreach (AlternateEnding ending in alternativeEnding)
        {
            if(ending.itemName == itemName)
            {
                return ending;
            }
        }
        return null;
    }
}
