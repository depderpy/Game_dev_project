using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum  GameState {FreeRoam, Dialog, Battle}

public enum BattleState{
    Start,
    PlayerTurn,
    EnemyTurn,
    Busy,
    BattleOver
}

public class GameManager : MonoBehaviour
{
    private BattleState B_state;
    GameState state;
    [SerializeField] player playercontroller;
    [SerializeField] BattleManager battlemanager;

    private void Start()
    {
        state = GameState.FreeRoam;

        playercontroller.OnEncounter += () =>
        {
            state = GameState.Battle;
            battlemanager.StartBattle();
        };

        DialogManager.Instance.OnShowDialog += ()=>
        {
            state = GameState.Dialog;
        };
        DialogManager.Instance.OnHideDialog += ()=>
        {
            if(state == GameState.Dialog)
            state = GameState.FreeRoam;
        };
    }
    

    private void Update()
    {
        if(state == GameState.FreeRoam)
        {
            playercontroller.HandleUpdate();
        }
        else if(state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }
        else if(state ==GameState.Battle)
        {
            battlemanager.HandleUpdate();
        }
    }
}
