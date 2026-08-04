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
    public enum BattleState{
    Start,
    PlayerTurn,
    EnemyTurn,
    Busy,
    BattleOver
}
    private BattleState state;
    

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
        StartCoroutine(
            TypeBattleText("Choose an action")
        );
        
    }

    private IEnumerator PlayerAttack()
    {
        state = BattleState.Busy;

        yield return StartCoroutine(
            TypeBattleText("You Attacked")
        );

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
        battleText.text = "";

        foreach(char letter in message)
        {
            battleText.text += letter;
            yield return new WaitForSeconds(0.03f);
        }
    }
    
}
