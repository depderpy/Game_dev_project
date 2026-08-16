using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// ============================================================
// GAME STATE
// ============================================================
// This enum keeps track of what the player is currently doing.
//
// FreeRoam -> Player can move around the world
// Dialog   -> Player is talking to an NPC
// Battle   -> Player is currently fighting an enemy
//
// GameManager uses this to decide which system should receive
// player input inside Update().
// ============================================================
public enum GameState
{
    FreeRoam,
    Dialog,
    Battle
}

public class GameManager : MonoBehaviour
{

    // ========================================================
    // CURRENT GAME STATE
    // ========================================================
    // Stores the current state of the game.
    //
    // For example:
    // state = GameState.FreeRoam;
    //
    // means the player is currently exploring the world.
    //
    // This is private by default because we only want the
    // GameManager itself to control the game state.
    // ========================================================

    GameState state;

    // ========================================================
    // REFERENCES TO OTHER GAME SYSTEMS
    // ========================================================
    // These are references to other scripts/components in the
    // Unity Inspector.
    //
    // GameManager acts as the "middle man" between these systems.
    //
    // Example:
    //
    // Player encounters enemy
    //       ↓
    // Player tells GameManager
    //       ↓
    // GameManager changes state to Battle
    //       ↓
    // GameManager tells BattleManager to start the battle
    // ========================================================

    // Handles player movement and detecting encounters/NPCs.

    // Handles player movement and detecting encounters/NPCs.
    [SerializeField] player playercontroller;

    // Handles everything that happens during combat.
    [SerializeField] BattleManager battlemanager;

     // Stores the player's inventory.
    //
    // public allows other scripts such as BattleManager and
    // QuestManager to access the inventory through GameManager.
    [SerializeField] public Inventory Inventory;

    // Stores the player's learned spells.
    //
    // Similar to Inventory, this allows the magic system to
    // access the player's available spells.
    [SerializeField] public Spellbook Spellbook;

    // Stores the list of enemies that can appear in random
    // encounters.
    //
    // The actual enemies are assigned through the Unity
    // Inspector.
    [SerializeField] private List<EnemyData> enemies;

    // Stores the player's Combatant data.
    //
    // This contains things such as HP, attack damage, etc.
    [SerializeField] private Combatant playerCombantant;


    // ========================================================
    // BATTLE END EVENT
    // ========================================================
    // This event can notify other systems when a battle ends.
    //
    // In this project, BattleManager invokes this event when
    // EndBattle() is called.
    //
    // GameManager listens to that event and changes the game
    // state back to FreeRoam.
    // ========================================================
    public event Action OnBattleEnd;

    // ========================================================
    // START
    // ========================================================
    // Start() runs once when the GameManager begins.
    //
    // This is where we:
    // - Set the starting game state
    // - Initialize the player's HP
    // - Give the player starting items
    // - Connect events between our different systems
    // ========================================================

    private void Start()
    {

        // ----------------------------------------------------
        // Set the initial game state.
        // ----------------------------------------------------
        // When the game first starts, the player should be
        // allowed to freely walk around.
        // ----------------------------------------------------
        state = GameState.FreeRoam;

        // ----------------------------------------------------
        // Initialize the player's Combatant.
        // ----------------------------------------------------
        // This sets up the player's starting HP/stats.
        //
        // IMPORTANT:
        // We only want to initialize the player's HP once here.
        // If Initialize() resets currentHP to maxHP every battle,
        // calling it from BattleManager.StartBattle() would make
        // the player's HP reset after every encounter.
        // ----------------------------------------------------
        // Initialize player HP ONCE when the game starts
        playerCombantant.Initialize();



        // ----------------------------------------------------
        // Debug messages
        // ----------------------------------------------------
        // These messages appear in Unity's Console.
        //
        // They are useful for checking whether the inventory
        // was successfully initialized.
        // ----------------------------------------------------
        Debug.Log("Inventory initialized");
        Debug.Log("Has Meat: " + Inventory.HasItem("Meat"));
        Debug.Log("Has Potion: " + Inventory.HasItem("Potion"));

        // ====================================================
        // PLAYER ENCOUNTER EVENT
        // ====================================================
        // The player script has an OnEncounter event.
        //
        // When the player walks into a random encounter,
        // PlayerController invokes OnEncounter.
        //
        // GameManager responds by calling StartRandomBattle().
        // ====================================================
        playercontroller.OnEncounter += () =>
        {
            StartRandomBattle();
        };

        // ====================================================
        // BATTLE END EVENT
        // ====================================================
        // BattleManager tells GameManager when the battle is
        // finished.
        //
        // Once the battle ends, we return the player to the
        // FreeRoam state so they can move again.
        //
        // This also handles:
        // - Winning
        // - Running away
        // - Alternate endings
        // - Player death
        // ====================================================
        battlemanager.OnBattleEnd += () =>
        {
            state = GameState.FreeRoam;
        };

        // ====================================================
        // DIALOG START EVENT
        // ====================================================
        // DialogManager tells GameManager when dialogue starts.
        //
        // We change the game state to Dialog so that the player
        // cannot walk around while talking to an NPC.
        // ====================================================
        DialogManager.Instance.OnShowDialog += () =>
        {
            state = GameState.Dialog;
        };

        // ====================================================
        // DIALOG END EVENT
        // ====================================================
        // When the dialogue finishes, DialogManager sends
        // OnHideDialog.
        //
        // We then return to FreeRoam.
        //
        // The "if" prevents accidentally changing the state if
        // something else has already changed the game state.
        // ====================================================
        DialogManager.Instance.OnHideDialog += () =>
        {
            if (state == GameState.Dialog)
                state = GameState.FreeRoam;
        };
    }


    // ========================================================
    // UPDATE
    // ========================================================
    // Update() runs every frame.
    //
    // The important job of this method is to decide which
    // system is currently allowed to receive player input.
    //
    // This prevents different systems from fighting over the
    // same keyboard inputs.
    // ========================================================
    private void Update()
    {
        // ----------------------------------------------------
        // FREE ROAM
        // ----------------------------------------------------
        // If the player is exploring:
        //
        // → Allow the player controller to process movement,
        //   NPC interaction and encounter detection.
        // ----------------------------------------------------

        if (state == GameState.FreeRoam)
        {
            playercontroller.HandleUpdate();
        }


        // ----------------------------------------------------
        // DIALOG
        // ----------------------------------------------------
        // If the player is talking to an NPC:
        //
        // → Send input to DialogManager instead.
        //
        // This allows the player to press E to advance
        // through dialogue without moving the character.
        // ----------------------------------------------------

        else if (state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }


        // ----------------------------------------------------
        // BATTLE
        // ----------------------------------------------------
        // If the player is fighting:
        //
        // → Send input to BattleManager.
        //
        // BattleManager then handles:
        // - Attack
        // - Magic
        // - Items
        // - Run
        // - Battle menu navigation
        // ----------------------------------------------------

        else if (state == GameState.Battle)
        {
            battlemanager.HandleUpdate();
        }
    }

     // ========================================================
    // START RANDOM BATTLE
    // ========================================================
    // This method chooses a random enemy from the enemies list
    // and starts a battle against that enemy.
    //
    // It is called when PlayerController detects a random
    // encounter.
    // ========================================================

    private void StartRandomBattle()
    {
        // ----------------------------------------------------
        // Pick a random index.
        // ----------------------------------------------------
        // Random.Range(0, enemies.Count) generates a number
        // starting at 0 and stopping BEFORE enemies.Count.
        //
        // Example:
        //
        // enemies.Count = 3
        //
        // Possible results:
        // 0
        // 1
        // 2
        //
        // This is important because List indexes start at 0.
        // ----------------------------------------------------
        int randomEnemy = UnityEngine.Random.Range(0, enemies.Count);


        // ----------------------------------------------------
        // Get the EnemyData at the randomly selected index.
        // ----------------------------------------------------
        //
        // For example:
        //
        // enemies[0] = Wolf
        // enemies[1] = Slime
        // enemies[2] = Goblin
        //
        // If randomEnemy is 1, selectedEnemy becomes Slime.
        // ----------------------------------------------------

        EnemyData selectedEnemy = enemies[randomEnemy];

        // ----------------------------------------------------
        // Change the game state to Battle.
        // ----------------------------------------------------
        // This is VERY important.
        //
        // Once the state becomes Battle, GameManager.Update()
        // will stop sending input to the player controller and
        // start sending input to BattleManager instead.
        // ----------------------------------------------------
        state = GameState.Battle;

        // ----------------------------------------------------
        // Tell BattleManager which enemy was selected.
        // ----------------------------------------------------
        //
        // BattleManager receives the EnemyData and uses it to
        // set up:
        //
        // - Enemy stats
        // - Enemy name
        // - Enemy sprite
        // - Enemy drops
        // - Alternate endings
        //
        // This is what makes the battle system dynamic instead
        // of being hardcoded to one specific enemy.
        // ----------------------------------------------------
        battlemanager.StartBattle(selectedEnemy);
    }
}