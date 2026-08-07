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
    }
    

    public void HandleUpdate()
    {
        if(state != BattleState.PlayerTurn)
        return;
        //Menu stuff here afterwards

        if(Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(PlayerAttack());
            Debug.Log("Player Attacked");
        }

        if(Input.GetKeyDown(KeyCode.S))
        {
            currentSelection++;

            if(currentSelection >3)
            currentSelection = 0;
            
            UpdateCommandMenu();
            
        }

        if(Input.GetKeyDown(KeyCode.W))
        {
            currentSelection--;

            if(currentSelection <0)
            currentSelection = 3;
            
            UpdateCommandMenu();
            
        }
    }

    public void StartBattle()
    {
        BattleScreen.SetActive(true);
        state = BattleState.Start;

        StartCoroutine(SetUpBattle());
        Debug.Log("Battle Started");
    }

    public void EndBattle()
    {
        BattleScreen.SetActive(false);
        Debug.Log("Battle ended");
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
        state = BattleState.EnemyTurn;
        StartCoroutine(EnemyTurn());
    }

    private IEnumerator EnemyTurn()
    {
        state = BattleState.Busy;
        yield return StartCoroutine(
            TypeBattleText("Enemy Turn")
            );
        yield return new WaitForSeconds(1f);
        PlayerTurn();   
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
    
}
