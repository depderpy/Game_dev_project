using UnityEngine;
using System.Collections.Generic;


// Represents the player's collection of learned spells.
//
// This script acts similarly to the Inventory system,
// except that instead of storing Items, it stores Spell objects.
//
// Because this inherits from MonoBehaviour, the Spellbook can
// be attached to a GameObject in Unity and referenced by other scripts.
public class Spellbook : MonoBehaviour
{
    // A list containing all of the spells the player currently knows.
    //
    // List<Spell> means that every element in this list must be
    // a Spell object.
    //
    // new List<Spell>() creates an empty spell list when the
    // Spellbook is created.
    //
    // Other scripts can add spells to this list through LearnSpell().
    public List<Spell> spells = new List<Spell>();


    // Adds a new spell to the player's spellbook.
    //
    // The Spell parameter represents the spell that the player
    // is learning.
    //
    // This method allows spells to be added dynamically during
    // gameplay rather than having to hardcode every spell.
    public void LearnSpell(Spell spell)
    {
        // Adds the provided Spell object to the spell list.
        spells.Add(spell);

    }


    // Checks whether the player currently knows a specific spell.
    //
    // The spellName parameter is the name of the spell that
    // we are searching for.
    //
    // Returns true if the spell is found in the spellbook.
    // Returns false if the spell cannot be found.
    public bool HasSpell(string spellName)
    {
        // Goes through every Spell currently stored
        // inside the player's spellbook.
        foreach(Spell spell in spells)
        {
            // Compares the name of the current spell in the list
            // with the spell name that we are looking for.
            if(spell.spellName == spellName)
            {
                // The requested spell was found,
                // so return true immediately.
                return true;
            }
        }

        // If the foreach loop finishes without finding
        // a matching spell, return false.
        return false;
    }
}