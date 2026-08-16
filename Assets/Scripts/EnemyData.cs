using UnityEngine;
using System.Collections.Generic;

// Stores information specific to an enemy.
//
// This class acts as a data container for an enemy and connects
// the enemy's combat statistics, battle sprite, alternate endings,
// and item drops together.
//
// Because this inherits from MonoBehaviour, it can be attached
// to an enemy GameObject in Unity.
public class EnemyData : MonoBehaviour
{
    // Stores the name of the enemy.
    //
    // This is used by the BattleManager when displaying messages
    // such as:
    // "A Wild Wolf appeared!"
    // "Wolf took 5 damage!"
    public string enemyName;


    // Stores the Combatant component containing the enemy's
    // actual combat statistics.
    //
    // This includes information such as:
    // - Maximum HP
    // - Current HP
    // - Attack damage
    // - Magic damage
    //
    // The BattleManager uses this reference to perform
    // combat actions against this enemy.
    public Combatant combatant;


    // Stores the GameObject containing the enemy's visual sprite
    // used during battles.
    //
    // The BattleManager can activate or deactivate this GameObject
    // when the enemy appears or is defeated.
    public GameObject battlesprite;


    // Determines whether this enemy can accept Meat as an item
    // for an alternate battle ending.
    //
    // This value can be configured in the Unity Inspector.
    public bool acceptMeat;


    // Stores a list of possible alternate endings for this enemy.
    //
    // Each AlternateEnding can specify an item that the player
    // can use to trigger a special outcome instead of defeating
    // the enemy through normal combat.
    //
    // For example:
    // Meat -> Wolf leaves peacefully.
    public List <AlternateEnding> alternativeEnding;


    // Stores the list of items that this enemy can drop
    // when it is defeated.
    //
    // The BattleManager uses this list to give the player
    // the appropriate rewards after winning a battle.
    public List<Item> drops;


    // Start is called when the enemy GameObject is initialized.
    //
    // This test checks whether the enemy has an alternate ending
    // associated with the item "Meat".
    private void Start()
    {
        // Searches the alternativeEnding list for an ending
        // associated with the item named "Meat".
        //
        // The returned AlternateEnding is stored in the
        // "ending" variable.
        AlternateEnding ending = GetalternativeEndings("Meat");


        // Checks whether an alternate ending was found.
        //
        // If GetalternativeEndings() successfully finds a matching
        // ending, it returns that ending instead of null.
        if(ending != null)
        {
            // Prints a message to the Unity Console indicating
            // that an alternate ending was successfully found.
            Debug.Log("Found ending");


            // Prints the message associated with the alternate ending.
            //
            // This is mainly useful for testing whether the
            // alternate ending has been correctly configured
            // in the Unity Inspector.
            Debug.Log(ending.EndMessage);
        }

        // Runs when no alternate ending for "Meat" was found.
        else
        {
            // Prints a message to the Unity Console indicating
            // that no matching alternate ending was found.
            Debug.Log("No Alternate ending found.");
        }
    }


    // Searches through the enemy's list of alternate endings
    // to find one associated with a specific item.
    //
    // The itemName parameter represents the name of the item
    // being searched for.
    //
    // For example:
    // GetalternativeEndings("Meat")
    //
    // will search for an AlternateEnding whose itemName is "Meat".
    public AlternateEnding GetalternativeEndings(string itemName)
    {
        // Loops through every AlternateEnding stored in the
        // alternativeEnding list.
        foreach (AlternateEnding ending in alternativeEnding)
        {
            // Checks whether the item associated with the current
            // alternate ending matches the item name provided
            // to the method.
            if(ending.itemName == itemName)
            {
                // Returns the matching alternate ending immediately.
                //
                // Once a match is found, there is no need to continue
                // searching through the rest of the list.
                return ending;
            }
        }


        // If the loop finishes without finding a matching ending,
        // null is returned.
        //
        // This allows the BattleManager to determine that the item
        // does not have a special effect for this enemy.
        return null;
    }
}