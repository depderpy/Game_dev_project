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
    [SerializeField] private GameObject EnemyBattleSprite;


    public Inventory Inventory => Inventory;

    [SerializeField] private GameObject itemBox;
    [SerializeField] Text itemText;

    private int currentItemSelection = 0;

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
        PlayerBattleSprite.SetActive(false);
        EnemyBattleSprite.SetActive(false);
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
        
    }

    public void StartBattle()
    {
        BattleScreen.SetActive(true);
        PlayerBattleSprite.SetActive(true);
        EnemyBattleSprite.SetActive(true);

        playerCombantant.Initialize();
        enemyCombatant.Initialize();

        state = BattleState.Start;

        StartCoroutine(SetUpBattle());
        Debug.Log("Battle Started");
    }

    public void EndBattle()
    {
        BattleScreen.SetActive(false);
        BattledialogBox.SetActive(false);
        commandBox.SetActive(false);
        PlayerBattleSprite.SetActive(false);
        EnemyBattleSprite.SetActive(false);
        Debug.Log("Battle ended");

        state = BattleState.BattleOver;
        Debug.Log("Battle Ended");

        OnBattleEnd?.Invoke();
    }

    private IEnumerator SetUpBattle()
    {
        yield return StartCoroutine(
            TypeBattleText("A Wild Slime appeared")
            );
        yield return new WaitForSeconds(2f);

        PlayerTurn();
    }

    private void PlayerTurn()
    {
        state = BattleState.PlayerTurn;
        commandBox.SetActive(true);
        StartCoroutine(TypeBattleText("Choose an action"));

        currentSelection = 0;
        UpdateCommandMenu();
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
            EnemyBattleSprite.SetActive(false);
            yield return StartCoroutine(TypeBattleText(enemyCombatant.combatantName + " Has been defeated "));
            yield return new WaitForSeconds(1f);
            EndBattle();
            yield break;
        }   

        state = BattleState.EnemyTurn;
        StartCoroutine(EnemyTurn());
    }

    private IEnumerator MagicMenu()
    {
        state = BattleState.Busy;
        commandBox.SetActive(false);

        yield return StartCoroutine(TypeBattleText("Magic Go!!!"));
        
        yield return new WaitForSeconds(1f);
        PlayerTurn();
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
        PlayerTurn();
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

        gamemanager.Inventory.RemoveItem(selectedItemName);

        yield return StartCoroutine(
            TypeBattleText("You used " + selectedItemName + "!")
        );

        yield return new WaitForSeconds(2f);

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

    private void SelectCommand()
    {
        if(currentSelection == 0 )
        {
            StartCoroutine(PlayerAttack());
        }
        else if(currentSelection == 1)
        {
            StartCoroutine(MagicMenu());
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
