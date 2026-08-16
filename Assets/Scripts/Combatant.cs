using UnityEngine;

public class Combatant : MonoBehaviour
{
    // Stores the name of the combatant.
    // This is used when displaying battle messages such as
    // "Wolf Took 5 damage!" or "Player healed 10 HP!"
    public string combatantName;

    // Stores the maximum amount of HP this combatant can have.
    public int maxHP;

    // Stores the combatant's current HP.
    // This value changes when the combatant takes damage or heals.
    public int currentHP;

    // Stores the base physical attack damage of the combatant.
    public int attackDMG;

    // Stores the base magic damage of the combatant.
    // This can be used later when implementing more advanced magic mechanics.
    public int magicDMG;


    // Initializes the combatant's HP at the beginning of combat.
    // The current HP is set to the combatant's maximum HP.
    public void Initialize()
    {
        currentHP = maxHP;

    }

    // Reduces the combatant's current HP by the specified damage amount.
    public void takeDamage(int damage)
    {
        // Subtract the incoming damage from the current HP.
        currentHP -= damage;

        // Prevents the combatant's HP from going below 0.
        if(currentHP < 0)
        currentHP = 0;

        // Displays a message in the Unity Console showing how much damage
        // the combatant received.
        Debug.Log(combatantName + "Took" + damage + "damage!");

        // Displays the combatant's current HP and maximum HP.
        Debug.Log(combatantName + "HP:" + currentHP + "/" + maxHP);
    }

    // Calculates and returns the amount of physical damage
    // the combatant will deal with a normal attack.
    public int getDamage()
    {
        // Random.Range is used to make attacks deal variable damage.
        // The minimum value is half of the base attack damage.
        // The maximum value is the full attack damage.
        //
        // "+ 1" is used because Unity's integer Random.Range
        // excludes the maximum value.
        return Random.Range(attackDMG / 2, attackDMG + 1);
    }

    // Checks whether the combatant has reached 0 HP.
    // Returns true if the combatant is dead.
    public bool isDead()
    {
       return currentHP <= 0;
    }

    // Restores a specified amount of HP to the combatant.
    public void heal(int amount)
    {
        // Add the healing amount to the combatant's current HP.
        currentHP += amount;

        // Prevents the combatant's HP from going above their maximum HP.
        if(currentHP > maxHP)
        currentHP = maxHP;

        // Displays a message in the Unity Console showing
        // how much HP the combatant recovered.
        Debug.Log(combatantName + "healed" + amount + " HP!");

        // Displays the combatant's current HP and maximum HP after healing.
        Debug.Log(combatantName + "HP: " + currentHP + " / " + maxHP);

    }
}