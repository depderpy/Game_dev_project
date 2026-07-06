using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum  GameState {FreeRoam, Dialog}

public class GameManager : MonoBehaviour
{
    GameState state;
    [SerializeField] player playercontroller;

    private void start()
    {
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
    

    private void update()
    {
        if(state == GameState.FreeRoam)
        {
            playercontroller.HandleUpdate();
        }
        else if(state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }
    }
}
