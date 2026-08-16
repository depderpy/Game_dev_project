using UnityEngine;


// Allows the Spell class to be serialized by Unity.
// This means Spell objects can be created, stored, and edited
// through the Unity Inspector.
[System.Serializable] 
public class Spell
{
    // The name of the spell.
    // This is displayed when the player views their spellbook
    // and selects a spell during battle.
    public string spellName;

    // The amount of damage the spell deals to an enemy
    // when the spell is successfully cast.
    public int damage;

    // The amount of mana required to cast the spell.
    // This can be used in the future if a mana system is added.
    public int manaCost;
}