using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private Text battleText;
    [SerializeField] GameObject BattleScreen;
    [SerializeField] GameObject BattledialogBox;

    [SerializeField] private GameObject commandBox;
    [SerializeField] private Text commandText;
    public event Action OnBattleEnd;
    [SerializeField] GameManager gamemanager;
    [SerializeField] private Combatant playerCombantant;
    [SerializeField] private Combatant enemyCombatant;
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private GameObject PlayerBattleSprite;


    public Inventory Inventory => Inventory;
    public Spellbook Spellbook => Spellbook;

    [SerializeField] private GameObject itemBox;
    [SerializeField] Text itemText;

    [SerializeField] private GameObject spellBox;
    [SerializeField] Text spellText;

    private int currentItemSelection = 0;
    private int currentSpellSelection = 0;
    private int currentSelection = 0;

    private string[] commands = 
    {
        "Attack",
        "Magic",
        "Item",
        "Run"
    };
    
    public enum BattleState{
    Start,
    PlayerTurn,
    EnemyTurn,
    Busy,
    BattleOver
    }
    private BattleState state;

    private void Start()
    {
        BattleScreen.SetActive(false);
        BattledialogBox.SetActive(false);
        commandBox.SetActive(false);
        itemBox.SetActive(false);
        spellBox.SetActive(false);
        PlayerBattleSprite.SetActive(false);
    }
    

    public void HandleUpdate()
    {
        if(state == BattleState.PlayerTurn && commandBox.activeSelf)
        {
            HandleCommandMenu();
        }
        

        else if(state == BattleState.Busy && itemBox.activeSelf)
        {
            HandleItemMenu();
        }

        else if (state == BattleState.Busy && spellBox.activeSelf)
        {
            HandleSpellMenu();
        }
        
    }

    public void StartBattle(EnemyData enemy)
    {

        enemyData = enemy;

        BattleScreen.SetActive(true);
        PlayerBattleSprite.SetActive(true);
        enemyData.battlesprite.SetActive(true);

        enemyCombatant = enemy.combatant;
        enemyCombatant.Initialize();

        state = BattleState.Start;

        StartCoroutine(SetUpBattle());
        Debug.Log("Battle Started" + enemyData.enemyName);
    }

    public void EndBattle()
    {
        BattleScreen.SetActive(false);
        BattledialogBox.SetActive(false);
        commandBox.SetActive(false);
        PlayerBattleSprite.SetActive(false);
        enemyData.battlesprite.SetActive(false);
        Debug.Log("Battle ended");

        state = BattleState.BattleOver;
        Debug.Log("Battle Ended");

        OnBattleEnd?.Invoke();
    }

    private IEnumerator SetUpBattle()
    {
        yield return StartCoroutine(
            TypeBattleText("A Wild "+ enemyData.enemyName + " appeared!")
            );
        yield return new WaitForSeconds(2f);

        StartCoroutine(PlayerTurn());
    }

    private IEnumerator PlayerTurn()
    {
        state = BattleState.PlayerTurn;
        commandBox.SetActive(true);
        StartCoroutine(TypeBattleText("Choose an action"));

        currentSelection = 0;
        UpdateCommandMenu();

        yield return null;
    }


    private IEnumerator PlayerAttack()
    {
        state = BattleState.Busy;
        commandBox.SetActive(false);

        yield return StartCoroutine(TypeBattleText("You Attacked"));

        yield return new WaitForSeconds(1f);

        int damage = playerCombantant.getDamage();
        enemyCombatant.takeDamage(damage);

        yield return StartCoroutine(
            TypeBattleText(enemyCombatant.combatantName + " Took " + damage + " damage!! "));

        yield return new WaitForSeconds(1f);

        if(enemyCombatant.isDead())
        {
            enemyData.battlesprite.SetActive(false);
            yield return StartCoroutine(TypeBattleText(enemyCombatant.combatantName + " Has been defeated "));
            yield return new WaitForSeconds(1f);

            yield return StartCoroutine(giveDrop());

            yield return new WaitForSeconds(1f);

            EndBattle();
            yield break;
        }   

        state = BattleState.EnemyTurn;
        StartCoroutine(EnemyTurn());
    }

    private IEnumerator MagicAttack()
    {
        state = BattleState.Busy;
        commandBox.SetActive(false);

        yield return StartCoroutine(TypeBattleText("You cast Fire"));
    }

    private IEnumerator MagicMenu()
    {
        state = BattleState.Busy;
        commandBox.SetActive(false);

        yield return StartCoroutine(TypeBattleText("Magic Go!!!"));
        
        yield return new WaitForSeconds(1f);
        StartCoroutine(EnemyTurn());
    }

    private IEnumerator OpenMagicMenu()
    {
        state = BattleState.Busy;
        commandBox.SetActive(false);
        spellBox.SetActive(true);

        currentSpellSelection = 0;
        UpdateMagicMenu();

        yield return null;
    }

    private void CloseMagicMenu()
    {
        spellBox.SetActive(false);

        commandBox.SetActive(true);

        state = BattleState.PlayerTurn;

        currentSelection = 0;
        UpdateCommandMenu();   
    }

    private IEnumerator OpenItemMenu()
    {
        state = BattleState.Busy;
        commandBox.SetActive(false);
        itemBox.SetActive(true);

        currentItemSelection= 0;
        UpdateItemMenu();

        yield return null;

        }
    private void CloseItemMenu()
    {
        itemBox.SetActive(false);

        commandBox.SetActive(true);

        state = BattleState.PlayerTurn;

        currentSelection = 0;
        UpdateCommandMenu();    
        }

        

    private IEnumerator Run()
    {
        state = BattleState.Busy;
        commandBox.SetActive(false);

        yield return StartCoroutine(TypeBattleText("You running away"));

        yield return new WaitForSeconds(1f);

        EndBattle();
    }

    private IEnumerator EnemyTurn()
    {
        state = BattleState.Busy;
        yield return StartCoroutine(
            TypeBattleText("Enemy Turn")
            );
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(EnemyAttack());
        StartCoroutine(PlayerTurn());
    }

    private IEnumerator EnemyAttack()
        {
            state = BattleState.Busy;

            yield return StartCoroutine(TypeBattleText(enemyCombatant.combatantName + " Attacks!"));

            yield return new WaitForSeconds(0.5f);

            int damage = enemyCombatant.getDamage();

            playerCombantant.takeDamage(damage);

            yield return StartCoroutine(TypeBattleText(playerCombantant.combatantName + " Took " + damage + " damage "));

            yield return new WaitForSeconds(1f);

            if(playerCombantant.isDead())
            {
                PlayerBattleSprite.SetActive(false);
                yield return StartCoroutine(TypeBattleText("You have been defeated"));
                yield return new WaitForSeconds(1f);
                EndBattle();
                yield break;
            }
        }

    private IEnumerator TypeBattleText(string message)
    {
        BattledialogBox.SetActive(true);
        battleText.text = "";

        foreach(char letter in message)
        {
            battleText.text += letter;
            yield return new WaitForSeconds(0.03f);
        }
    }

    private void UpdateCommandMenu()
    {
        commandText.text = "";

        for(int i = 0;  i < commands.Length; i++)
        {
            if(i == currentSelection)
                commandText.text +="> ";
            else
                commandText.text += " ";

            commandText.text += commands[i]+ "\n";
        }
    }   

    private void HandleCommandMenu()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SelectCommand();
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            currentSelection++;

            if(currentSelection >= commands.Length)
            currentSelection = 0;
            
            UpdateCommandMenu();
            
        }

        if(Input.GetKeyDown(KeyCode.W))
        {
            currentSelection--;

            if(currentSelection < 0)
            currentSelection = commands.Length -1;
            
            UpdateCommandMenu();
            
        }
    }

    private void HandleItemMenu()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            currentItemSelection++;

            if(currentItemSelection >= gamemanager.Inventory.items.Count)
            currentItemSelection = 0;

            UpdateItemMenu();
        }

        if(Input.GetKeyDown(KeyCode.W))
        {
            currentItemSelection--;
            if(currentItemSelection < 0)
            currentItemSelection = gamemanager.Inventory.items.Count -1;

            UpdateItemMenu();
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SelectItem();
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            CloseItemMenu();
        }
    }

    private void HandleSpellMenu()
    {
        List<Spell> spells = gamemanager.Spellbook.spells;

        if(spells.Count ==0)
        return;

        if(Input.GetKeyDown(KeyCode.S))
        {
            currentSpellSelection++;

            if(currentSpellSelection >= gamemanager.Spellbook.spells.Count)
            currentSpellSelection = 0;

            UpdateMagicMenu();
        }

        if(Input.GetKeyDown(KeyCode.W))
        {
            currentSpellSelection--;
            if(currentSpellSelection < 0)
            currentSpellSelection = gamemanager.Spellbook.spells.Count -1;

            UpdateMagicMenu();
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SelectSpell();
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            CloseMagicMenu();
        }
    }

    private void SelectSpell()
    {
        List<Spell> spells = gamemanager.Spellbook.spells;

        if(spells.Count == 0)
        return;

        Spell selectedSpell = spells[currentSpellSelection];
        StartCoroutine(CastSpell(selectedSpell));
    }

    private IEnumerator CastSpell(Spell spell)
    {
        state = BattleState.Busy;

        spellBox.SetActive(false);
        yield return StartCoroutine(TypeBattleText("You cast " + spell.spellName + "!"));

        yield return new WaitForSeconds(0.5f);

        enemyCombatant.takeDamage(spell.damage);

        yield return StartCoroutine(TypeBattleText(enemyCombatant.combatantName + " took " + spell.damage + " damage!!"));

        yield return new WaitForSeconds(2f);

        if(enemyCombatant.isDead())
        {
            enemyData.battlesprite.SetActive(false);

        yield return StartCoroutine(
            TypeBattleText(
                enemyCombatant.combatantName + " has been defeated!"));
            yield return new WaitForSeconds(1f);
            yield return StartCoroutine(giveDrop());

            EndBattle();

            yield break;
        }
        StartCoroutine(EnemyTurn());

    }
    private void SelectItem()
    {
        List<Item> items = gamemanager.Inventory.items;

        if(items.Count == 0)
        return;

        Item selectedItem = items[currentItemSelection];
        StartCoroutine(UseItem(selectedItem));
    }
    private IEnumerator UseItem(Item item)
    {   
        state = BattleState.Busy;
        itemBox.SetActive(false);
        string selectedItemName = item.itemName;

        //Removes Item from inventory after use
        gamemanager.Inventory.RemoveItem(selectedItemName);

        yield return StartCoroutine(
            TypeBattleText("You used " + selectedItemName + "!")
        );

        yield return new WaitForSeconds(2f);

        //Potion stuff

        if(selectedItemName == "Potion")
        {
            int healAmount = 10;

            playerCombantant.heal(healAmount);

            yield return StartCoroutine(TypeBattleText("You recovered " + healAmount + " HP!"));

            yield return new WaitForSeconds(1f);

            StartCoroutine(EnemyTurn());
            yield break;
        }

        AlternateEnding ending = 
        enemyData.GetalternativeEndings(selectedItemName);

        if(ending != null)
        {
            yield return StartCoroutine(TypeBattleText(ending.EndMessage));

            yield return new WaitForSeconds(2f);

            EndBattle();
            yield break;
        }

        yield return StartCoroutine(TypeBattleText("It has no effect!"));

        yield return new WaitForSeconds(1f);
        StartCoroutine(OpenItemMenu());
    }


    private void UpdateItemMenu()
    {
        itemText.text ="";
        List<Item> items = gamemanager.Inventory.items;

        for(int i = 0; i <items.Count; i++)
        {
            if(i == currentItemSelection)
            itemText.text += "> ";
            else
            itemText.text += " ";
            

            itemText.text += items[i].itemName + "x " + items[i].quantity + "\n";
        }
    }

    private void UpdateMagicMenu()
    {
        spellText.text = "";
        List<Spell> spells = gamemanager.Spellbook.spells;

        for(int i = 0; i < spells.Count; i++)
        {
            if(i == currentSpellSelection)
            spellText.text += "> ";
            else
            spellText.text += " ";

            spellText.text += spells[i].spellName + "\n";
        }
    }

    private IEnumerator giveDrop()
    {
        foreach(Item drop in enemyData.drops)
        {
            gamemanager.Inventory.AddItem(drop.itemName, drop.quantity);

            yield return StartCoroutine(TypeBattleText
            ("The" + enemyCombatant.combatantName + " droppped " + drop.itemName + " x " + drop.quantity + "!!")
            );
        }
    }
    

    private void SelectCommand()
    {
        if(currentSelection == 0 )
        {
            StartCoroutine(PlayerAttack());
        }
        else if(currentSelection == 1)
        {
            StartCoroutine(OpenMagicMenu());
        }
        else if(currentSelection == 2)
        {
            StartCoroutine(OpenItemMenu());
        }

        else if(currentSelection == 3)
        {
            StartCoroutine(Run());
        }
    }
}
