using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum  GameState {FreeRoam, Dialog, Battle}



public class GameManager : MonoBehaviour
{
    GameState state;
    [SerializeField] player playercontroller;
    [SerializeField] BattleManager battlemanager;
    [SerializeField] public Inventory Inventory;
    [SerializeField] public Spellbook Spellbook;
    [SerializeField] private List<EnemyData> enemies;
    [SerializeField] private Combatant playerCombantant;

    public event Action OnBattleEnd;

    private void Start()
    {
        state = GameState.FreeRoam;
        playerCombantant.Initialize();

        Inventory.AddItem("Meat",3);
        Inventory.AddItem("Potion",2);

        Debug.Log("Inventory initialized");
        Debug.Log("Has Meat: " + Inventory.HasItem("Meat"));
        Debug.Log("Has Potion: " + Inventory.HasItem("Potion"));


        playercontroller.OnEncounter += () =>
        {
            StartRandomBattle();
            

        };

        battlemanager.OnBattleEnd += () =>
        {
            state = GameState.FreeRoam;
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

    private void StartRandomBattle()
    {
        int randomEnemy = UnityEngine.Random.Range(0, enemies.Count);
            EnemyData selectedEnemy = enemies[randomEnemy];
            state = GameState.Battle;
            battlemanager.StartBattle(selectedEnemy);
    }
}
