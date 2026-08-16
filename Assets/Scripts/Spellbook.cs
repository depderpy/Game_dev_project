using UnityEngine;
using System.Collections.Generic;

public class Spellbook : MonoBehaviour
{
    public List<Spell> spells = new List<Spell>();

    public void LearnSpell(Spell spell)
    {
        spells.Add(spell);

    }

    public bool HasSpell(string spellName)
    {
        foreach(Spell spell in spells)
        {
            if(spell.spellName == spellName)
            {
                return true;
            }
        }
        return false;
    }
}
