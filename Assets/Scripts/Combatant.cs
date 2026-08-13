using UnityEngine;

public class Combatant : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string combatantName;

    public int maxHP;
    public int currentHP;
    public int attackDMG;


    public void Initialize()
    {
        currentHP = maxHP;
    }

    public void takeDamage(int damage)
    {
        currentHP -= damage;

        if(currentHP < 0)
        currentHP = 0;

        Debug.Log(combatantName + "Took" + damage + "damage!");
        Debug.Log(combatantName + "HP:" + currentHP + "/" + maxHP);
    }

    public int getDamage()
    {
        return Random.Range(attackDMG / 2, attackDMG + 1);
    }

    public bool isDead()
    {
       return currentHP <= 0;
    }
}
