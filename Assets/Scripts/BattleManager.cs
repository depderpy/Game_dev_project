using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [SerializeField] GameObject BattleScreen;
    

    public void HandleUpdate()
    {

    }

    public void StartBattle()
    {
        BattleScreen.SetActive(true);
        Debug.Log("Battle Started");
    }

    public void EndBattle()
    {
        BattleScreen.SetActive(false);
        Debug.Log("Battle ended");
    }
    
}
