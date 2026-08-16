using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// BattleManager is responsible for controlling everything that happens
// during a battle, including menus, attacks, items, magic, enemy turns,
// battle text, HP display, and ending the battle.
public class BattleManager : MonoBehaviour
{
    // UI Text used to display messages during battle.
    [SerializeField] private Text battleText;

    // The main GameObject containing the entire battle UI.
    [SerializeField] GameObject BattleScreen;

    // The GameObject containing the battle dialogue/text box.
    [SerializeField] GameObject BattledialogBox;

    // The GameObject containing the player's four battle commands.
    [SerializeField] private GameObject commandBox;

    // Text used to display the available battle commands.
    [SerializeField] private Text commandText;

    // Event that is triggered when a battle ends.
    // GameManager uses this to return the game back to FreeRoam.
    public event Action OnBattleEnd;

    // Reference to the GameManager.
    // Used to access the player's inventory, spellbook and other systems.
    [SerializeField] GameManager gamemanager;

    // The player's Combatant data.
    // Handles the player's HP, damage, healing, etc.
    [SerializeField] private Combatant playerCombantant;

    // The Combatant data belonging to the current enemy.
    [SerializeField] private Combatant enemyCombatant;

    // Stores information about the current enemy.
    // This includes the enemy's name, sprite, drops and alternative endings.
    [SerializeField] private EnemyData enemyData;

    // The GameObject containing the player's battle sprite.
    [SerializeField] private GameObject PlayerBattleSprite;

    // GameObject containing the player's HP UI.
    [SerializeField] private GameObject playerHPUI;

    // Text displaying the player's current HP.
    [SerializeField] private Text playerHPText;


    // Gives other scripts access to the Inventory.
    public Inventory Inventory => Inventory;

    // Gives other scripts access to the Spellbook.
    public Spellbook Spellbook => Spellbook;

    // Gives other scripts access to the QuestManager.
    public QuestManager QuestManager => QuestManager;

    // GameObject containing the inventory menu during battle.
    [SerializeField] private GameObject itemBox;

    // Text displaying the player's available items.
    [SerializeField] Text itemText;

    // GameObject containing the magic/spell menu during battle.
    [SerializeField] private GameObject spellBox;

    // Text displaying the player's available spells.
    [SerializeField] Text spellText;

    // Keeps track of which item the player currently has selected.
    private int currentItemSelection = 0;

    // Keeps track of which spell the player currently has selected.
    private int currentSpellSelection = 0;

    // Keeps track of which command the player currently has selected.
    // 0 = Attack
    // 1 = Magic
    // 2 = Item
    // 3 = Run
    private int currentSelection = 0;

    // List of commands available to the player during their turn.
    private string[] commands = 
    {
        "Attack",
        "Magic",
        "Item",
        "Run"
    };
    
    // Represents the different stages/states of a battle.
    public enum BattleState{
        Start,
        PlayerTurn,
        EnemyTurn,
        Busy,
        BattleOver
    }

    // Stores the current state of the battle.
    private BattleState state;

    // Runs when the BattleManager is first initialized.
    // Used to make sure all battle UI elements start hidden.
    private void Start()
    {
        // Hide the entire battle screen.
        BattleScreen.SetActive(false);

        // Hide the battle dialogue box.
        BattledialogBox.SetActive(false);

        // Hide the command menu.
        commandBox.SetActive(false);

        // Hide the item menu.
        itemBox.SetActive(false);

        // Hide the spell menu.
        spellBox.SetActive(false);

        // Hide the player's battle sprite.
        PlayerBattleSprite.SetActive(false);

        // Hide the player's HP UI.
        playerHPUI.SetActive(false);
    }

    // Updates the HP text shown on the battle UI.
    private void UpdatePlayerHP()
    {
        // Displays the player's current HP followed by their maximum HP.
        // Example: HP: 25 / 50
        playerHPText.text =
            "HP: " + playerCombantant.currentHP +
            " / " + playerCombantant.maxHP;
    }
    
    // Called by GameManager every frame while the game is in Battle state.
    // Determines which battle menu should receive player input.
    public void HandleUpdate()
    {
        // If the battle has already ended, do nothing.
        if(state ==BattleState.BattleOver)
        {
            return;
        }

        // If it is currently the player's turn and the command box is visible,
        // allow the player to navigate and select commands.
        if(state == BattleState.PlayerTurn && commandBox.activeSelf)
        {
            HandleCommandMenu();
        }
        
        // If the battle is busy and the item menu is open,
        // allow the player to navigate the item menu.
        else if(state == BattleState.Busy && itemBox.activeSelf)
        {
            HandleItemMenu();
        }

        // If the battle is busy and the spell menu is open,
        // allow the player to navigate the spell menu.
        else if (state == BattleState.Busy && spellBox.activeSelf)
        {
            HandleSpellMenu();
        }
    }

    // Starts a battle using the EnemyData passed in by GameManager.
    public void StartBattle(EnemyData enemy)
    {
        // Store the selected enemy as the current enemy.
        enemyData = enemy;

        // Display the battle screen.
        BattleScreen.SetActive(true);

        // Display the player's battle sprite.
        PlayerBattleSprite.SetActive(true);

        // Display the selected enemy's battle sprite.
        enemyData.battlesprite.SetActive(true);

        // Display the player's HP UI.
        playerHPUI.SetActive(true);

        // Get the Combatant belonging to the selected enemy.
        enemyCombatant = enemy.combatant;

        // Initialize the enemy's combat stats.
        enemyCombatant.Initialize();

        // Update the HP display when the battle starts.
        UpdatePlayerHP();

        // Set the initial battle state.
        state = BattleState.Start;

        // Begin the battle setup coroutine.
        StartCoroutine(SetUpBattle());

        // Print the enemy name in the Unity Console for debugging.
        Debug.Log("Battle Started" + enemyData.enemyName);
    }

    // Ends the current battle and hides all battle-related UI.
    public void EndBattle()
    {
        // Hide the entire battle screen.
        BattleScreen.SetActive(false);

        // Hide the battle dialogue box.
        BattledialogBox.SetActive(false);

        // Hide the command menu.
        commandBox.SetActive(false);

        // Hide the item menu.
        itemBox.SetActive(false);

        // Hide the player's HP UI.
        playerHPUI.SetActive(false);
        
        // Hide the player's battle sprite.
        PlayerBattleSprite.SetActive(false);

        // Hide the current enemy's battle sprite.
        enemyData.battlesprite.SetActive(false);

        // Debug message showing that the battle has ended.
        Debug.Log("Battle ended");

        // Change the battle state to BattleOver.
        state = BattleState.BattleOver;

        // Additional debug message.
        Debug.Log("Battle Ended");

        // Notify GameManager that the battle has ended.
        OnBattleEnd?.Invoke();
    }

    // Handles the initial setup of a battle.
    private IEnumerator SetUpBattle()
    {
        // Display the message announcing the enemy.
        yield return StartCoroutine(
            TypeBattleText("A Wild "+ enemyData.enemyName + " appeared!")
            );

        // Wait two seconds before continuing.
        yield return new WaitForSeconds(2f);

        // Start the player's turn.
        StartCoroutine(PlayerTurn());
    }

    // Starts the player's turn.
    private IEnumerator PlayerTurn()
    {
        // Change the battle state to PlayerTurn.
        state = BattleState.PlayerTurn;

        // Display the command menu.
        commandBox.SetActive(true);

        // Tell the player to choose an action.
        StartCoroutine(TypeBattleText("Choose an action"));

        // Reset the command selection to the first command.
        currentSelection = 0;

        // Update the command menu to show the selection.
        UpdateCommandMenu();

        // Wait one frame before finishing the coroutine.
        yield return null;
    }


    // Handles the player's normal physical attack.
    private IEnumerator PlayerAttack()
    {
        // Prevent other battle actions while the attack is happening.
        state = BattleState.Busy;

        // Hide the command menu during the attack.
        commandBox.SetActive(false);

        // Display the attack message.
        yield return StartCoroutine(TypeBattleText("You Attacked"));

        // Give the player a short delay after attacking.
        yield return new WaitForSeconds(1f);

        // Calculate the player's damage.
        int damage = playerCombantant.getDamage();

        // Apply the damage to the enemy.
        enemyCombatant.takeDamage(damage);

        // Display the amount of damage dealt.
        yield return StartCoroutine(
            TypeBattleText(enemyCombatant.combatantName + " Took " + damage + " damage!! "));

        // Wait before checking the enemy's HP.
        yield return new WaitForSeconds(1f);

        // Check whether the enemy has been defeated.
        if(enemyCombatant.isDead())
        {
            // Hide the enemy's battle sprite.
            enemyData.battlesprite.SetActive(false);

            // Display the defeat message.
            yield return StartCoroutine(TypeBattleText(enemyCombatant.combatantName + " Has been defeated "));

            // Wait before giving the player their drops.
            yield return new WaitForSeconds(1f);

            // Give the player the enemy's item drops.
            yield return StartCoroutine(giveDrop());
            
            // Small delay before ending the battle.
            yield return new WaitForSeconds(1f);

            // End the battle.
            EndBattle();

            yield break;
        }   

        // If the enemy survived, move to the enemy's turn.
        state = BattleState.EnemyTurn;

        // Start the enemy turn.
        StartCoroutine(EnemyTurn());
    }

    // Handles the magic attack itself.
    // Currently displays a Fire spell message.
    private IEnumerator MagicAttack()
    {
        // Prevent other actions while magic is being performed.
        state = BattleState.Busy;

        // Hide the command menu.
        commandBox.SetActive(false);

        // Display the spell message.
        yield return StartCoroutine(TypeBattleText("You cast Fire"));
    }

    // Placeholder magic menu function.
    private IEnumerator MagicMenu()
    {
        // Set the battle to Busy while magic is being handled.
        state = BattleState.Busy;

        // Hide the command menu.
        commandBox.SetActive(false);

        // Display the magic message.
        yield return StartCoroutine(TypeBattleText("Magic Go!!!"));
        
        // Wait one second.
        yield return new WaitForSeconds(1f);

        // Start the enemy's turn.
        StartCoroutine(EnemyTurn());
    }

    // Opens the player's spell selection menu.
    private IEnumerator OpenMagicMenu()
    {
        // Prevent other battle actions while the spell menu is open.
        state = BattleState.Busy;

        // Hide the main command menu.
        commandBox.SetActive(false);

        // Display the spell menu.
        spellBox.SetActive(true);

        // Reset the selected spell to the first spell.
        currentSpellSelection = 0;

        // Update the spell menu display.
        UpdateMagicMenu();

        // Wait one frame.
        yield return null;
    }

    // Closes the spell menu and returns to the main battle command menu.
    private void CloseMagicMenu()
    {
        // Hide the spell menu.
        spellBox.SetActive(false);

        // Show the main command menu.
        commandBox.SetActive(true);

        // Return control to the player.
        state = BattleState.PlayerTurn;

        // Reset the selected command to the first command.
        currentSelection = 0;

        // Refresh the command menu.
        UpdateCommandMenu();   
    }

    // Opens the player's inventory during battle.
    private IEnumerator OpenItemMenu()
    {
        // Prevent other actions while the item menu is open.
        state = BattleState.Busy;

        // Hide the command menu.
        commandBox.SetActive(false);

        // Show the item menu.
        itemBox.SetActive(true);

        // Start with the first item selected.
        currentItemSelection= 0;

        // Refresh the item menu.
        UpdateItemMenu();

        // Wait one frame.
        yield return null;
    }

    // Closes the item menu and returns to the main command menu.
    private void CloseItemMenu()
    {
        // Hide the item menu.
        itemBox.SetActive(false);

        // Show the command menu.
        commandBox.SetActive(true);

        // Return control to the player.
        state = BattleState.PlayerTurn;

        // Reset command selection.
        currentSelection = 0;

        // Refresh the command menu.
        UpdateCommandMenu();    
    }

    // Handles the player attempting to run away.
    private IEnumerator Run()
    {
        // Prevent other actions during the escape attempt.
        state = BattleState.Busy;

        // Hide the command menu.
        commandBox.SetActive(false);

        // Display the escape message.
        yield return StartCoroutine(TypeBattleText("You running away"));

        // Wait one second.
        yield return new WaitForSeconds(1f);

        // End the battle.
        EndBattle();
    }

    // Controls the enemy's turn.
    private IEnumerator EnemyTurn()
    {
        // Set the battle to Busy.
        state = BattleState.Busy;

        // Display the enemy turn message.
        yield return StartCoroutine(
            TypeBattleText("Enemy Turn")
            );

        // Wait one second.
        yield return new WaitForSeconds(1f);

        // Perform the enemy attack.
        yield return StartCoroutine(EnemyAttack());

        // If the player died and the battle ended,
        // stop the coroutine here.
        if(state == BattleState.BattleOver)
        yield break;

        // Otherwise start the player's turn.
        StartCoroutine(PlayerTurn());
    }

    // Handles the actual enemy attack.
    private IEnumerator EnemyAttack()
    {
        // Set the battle to Busy.
        state = BattleState.Busy;

        // Display the enemy attack message.
        yield return StartCoroutine(TypeBattleText(enemyCombatant.combatantName + " Attacks!"));

        // Short delay before damage is applied.
        yield return new WaitForSeconds(0.5f);

        // Calculate enemy damage.
        int damage = enemyCombatant.getDamage();

        // Apply damage to the player.
        playerCombantant.takeDamage(damage);

        // Update the player's HP UI.
        UpdatePlayerHP();

        // Display the damage taken.
        yield return StartCoroutine(TypeBattleText(playerCombantant.combatantName + " Took " + damage + " damage "));

        // Wait before checking whether the player died.
        yield return new WaitForSeconds(1f);

        // Check if the player has been defeated.
        if(playerCombantant.isDead())
        {
            // Hide the player's battle sprite.
            PlayerBattleSprite.SetActive(false);

            // Hide the command menu.
            commandBox.SetActive(false);

            // Display the defeat message.
            yield return StartCoroutine(TypeBattleText("You have been defeated. Try Again."));

            // Reset the player's combat stats.
            playerCombantant.Initialize();

            // Wait one second.
            yield return new WaitForSeconds(1f);

            // End the battle.
            EndBattle();

            yield break;
        }
    }

    // Displays text in the battle dialogue box one character at a time.
    private IEnumerator TypeBattleText(string message)
    {
        // Make sure the dialogue box is visible.
        BattledialogBox.SetActive(true);

        // Clear the previous message.
        battleText.text = "";

        // Go through each character in the message.
        foreach(char letter in message)
        {
            // Add one character at a time to create a typing effect.
            battleText.text += letter;

            // Wait briefly before displaying the next character.
            yield return new WaitForSeconds(0.03f);
        }
    }

    // Updates the command menu and displays the currently selected command.
    private void UpdateCommandMenu()
    {
        // Clear the existing command text.
        commandText.text = "";

        // Loop through every available command.
        for(int i = 0;  i < commands.Length; i++)
        {
            // Display the selection arrow beside the currently selected command.
            if(i == currentSelection)
                commandText.text +="> ";
            else
                commandText.text += " ";

            // Add the command name to the UI.
            commandText.text += commands[i]+ "\n";
        }
    }   

    // Handles keyboard navigation of the main battle menu.
    private void HandleCommandMenu()
    {
        // Pressing Space selects the current command.
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SelectCommand();
        }

        // Pressing S moves the selection down.
        if(Input.GetKeyDown(KeyCode.S))
        {
            currentSelection++;

            // Wrap around to the first command when reaching the bottom.
            if(currentSelection >= commands.Length)
            currentSelection = 0;
            
            // Refresh the menu.
            UpdateCommandMenu();
        }

        // Pressing W moves the selection up.
        if(Input.GetKeyDown(KeyCode.W))
        {
            currentSelection--;

            // Wrap around to the last command when moving above the first.
            if(currentSelection < 0)
            currentSelection = commands.Length -1;
            
            // Refresh the menu.
            UpdateCommandMenu();
        }
    }

    // Handles navigation of the inventory menu during battle.
    private void HandleItemMenu()
    {
        // Press S to move down through the item list.
        if(Input.GetKeyDown(KeyCode.S))
        {
            currentItemSelection++;

            // Wrap around to the first item.
            if(currentItemSelection >= gamemanager.Inventory.items.Count)
            currentItemSelection = 0;

            // Refresh the item menu.
            UpdateItemMenu();
        }

        // Press W to move up through the item list.
        if(Input.GetKeyDown(KeyCode.W))
        {
            currentItemSelection--;

            // Wrap around to the final item.
            if(currentItemSelection < 0)
            currentItemSelection = gamemanager.Inventory.items.Count -1;

            // Refresh the item menu.
            UpdateItemMenu();
        }

        // Press Space to use the selected item.
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SelectItem();
        }

        // Press Escape to close the item menu.
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            CloseItemMenu();
        }
    }

    // Handles navigation of the spell menu.
    private void HandleSpellMenu()
    {
        // Get the player's current list of spells.
        List<Spell> spells = gamemanager.Spellbook.spells;

        // If there are no spells, stop here.
        if(spells.Count ==0)
        return;

        // Press S to move down through the spell list.
        if(Input.GetKeyDown(KeyCode.S))
        {
            currentSpellSelection++;

            // Wrap around to the first spell.
            if(currentSpellSelection >= gamemanager.Spellbook.spells.Count)
            currentSpellSelection = 0;

            // Refresh the spell menu.
            UpdateMagicMenu();
        }

        // Press W to move up through the spell list.
        if(Input.GetKeyDown(KeyCode.W))
        {
            currentSpellSelection--;

            // Wrap around to the last spell.
            if(currentSpellSelection < 0)
            currentSpellSelection = gamemanager.Spellbook.spells.Count -1;

            // Refresh the spell menu.
            UpdateMagicMenu();
        }

        // Press Space to cast the selected spell.
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SelectSpell();
        }

        // Press Escape to close the spell menu.
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            CloseMagicMenu();
        }
    }

    // Gets the currently selected spell and begins casting it.
    private void SelectSpell()
    {
        // Get the player's spell list.
        List<Spell> spells = gamemanager.Spellbook.spells;

        // Do nothing if there are no spells.
        if(spells.Count == 0)
        return;

        // Get the spell currently selected by the player.
        Spell selectedSpell = spells[currentSpellSelection];

        // Start the spell casting process.
        StartCoroutine(CastSpell(selectedSpell));
    }

    // Handles the actual spell attack.
    private IEnumerator CastSpell(Spell spell)
    {
        // Prevent other actions while the spell is being cast.
        state = BattleState.Busy;

        // Hide the spell menu.
        spellBox.SetActive(false);

        // Display the spell casting message.
        yield return StartCoroutine(TypeBattleText("You cast " + spell.spellName + "!"));

        // Short delay before applying damage.
        yield return new WaitForSeconds(0.5f);

        // Apply the spell's damage to the enemy.
        enemyCombatant.takeDamage(spell.damage);

        // Display the damage dealt.
        yield return StartCoroutine(TypeBattleText(enemyCombatant.combatantName + " took " + spell.damage + " damage!!"));

        // Wait before checking the enemy's HP.
        yield return new WaitForSeconds(2f);

        // Check whether the enemy was defeated.
        if(enemyCombatant.isDead())
        {
            // Hide the enemy sprite.
            enemyData.battlesprite.SetActive(false);

            // Display the defeat message.
            yield return StartCoroutine(
                TypeBattleText(
                    enemyCombatant.combatantName + " has been defeated!"));

            // Wait before giving the player their drops.
            yield return new WaitForSeconds(1f);

            // Give the enemy drops.
            yield return StartCoroutine(giveDrop());

            // End the battle.
            EndBattle();

            yield break;
        }

        // If the enemy survived, begin the enemy turn.
        StartCoroutine(EnemyTurn());
    }

    // Gets the currently selected item and begins using it.
    private void SelectItem()
    {
        // Get the player's inventory list.
        List<Item> items = gamemanager.Inventory.items;

        // Do nothing if there are no items.
        if(items.Count == 0)
        return;

        // Get the currently selected item.
        Item selectedItem = items[currentItemSelection];

        // Start the item-use process.
        StartCoroutine(UseItem(selectedItem));
    }

    // Handles the effects of using an item.
    private IEnumerator UseItem(Item item)
    {   
        // Prevent other battle actions while the item is being used.
        state = BattleState.Busy;

        // Hide the item menu.
        itemBox.SetActive(false);

        // Store the item's name.
        string selectedItemName = item.itemName;

        // Removes one item from the inventory after it is used.
        gamemanager.Inventory.RemoveItem(selectedItemName,1);

        // Display the item usage message.
        yield return StartCoroutine(
            TypeBattleText("You used " + selectedItemName + "!")
        );
        

        // Wait before applying the item's effect.
        yield return new WaitForSeconds(2f);

        // Potion functionality.
        if(selectedItemName == "Potion")
        {
            // Amount of HP restored by the potion.
            int healAmount = 10;

            // Restore HP to the player.
            playerCombantant.heal(healAmount);

            // Display the healing message.
            yield return StartCoroutine(TypeBattleText("You recovered " + healAmount + " HP!"));

            // Update the HP UI.
            UpdatePlayerHP();

            // Wait before the enemy attacks.
            yield return new WaitForSeconds(1f);

            // Start the enemy turn.
            StartCoroutine(EnemyTurn());

            yield break;
        }

        // Check whether the selected item has an alternative ending associated with it.
        AlternateEnding ending = 
        enemyData.GetalternativeEndings(selectedItemName);

        // If an alternative ending exists, trigger it.
        if(ending != null)
        {
            // Display the alternative ending message.
            yield return StartCoroutine(TypeBattleText(ending.EndMessage));

            // Wait before ending the battle.
            yield return new WaitForSeconds(2f);

            // End the battle.
            EndBattle();

            yield break;
        }

        // If the item has no special effect, tell the player.
        yield return StartCoroutine(TypeBattleText("It has no effect!"));

        // Wait before reopening the item menu.
        yield return new WaitForSeconds(1f);

        // Reopen the item menu.
        StartCoroutine(OpenItemMenu());
    }


    // Updates the inventory menu displayed during battle.
    private void UpdateItemMenu()
    {
        // Clear the existing item menu text.
        itemText.text ="";

        // Get the player's inventory list.
        List<Item> items = gamemanager.Inventory.items;

        // Loop through every item in the inventory.
        for(int i = 0; i <items.Count; i++)
        {
            // Add the selection arrow beside the currently selected item.
            if(i == currentItemSelection)
            itemText.text += "> ";
            else
            itemText.text += " ";

            // Display the item's name and quantity.
            itemText.text += items[i].itemName + "x " + items[i].quantity + "\n";
        }
    }

    // Updates the spell menu displayed during battle.
    private void UpdateMagicMenu()
    {
        // Clear the existing spell menu text.
        spellText.text = "";

        // Get the player's spell list.
        List<Spell> spells = gamemanager.Spellbook.spells;

        // Loop through every spell.
        for(int i = 0; i < spells.Count; i++)
        {
            // Add the selection arrow beside the selected spell.
            if(i == currentSpellSelection)
            spellText.text += "> ";
            else
            spellText.text += " ";

            // Display the spell name.
            spellText.text += spells[i].spellName + "\n";
        }
    }

    // Gives the player all items listed in the enemy's drop list.
    private IEnumerator giveDrop()
    {
        // Loop through every possible item drop assigned to the enemy.
        foreach(Item drop in enemyData.drops)
        {
            // Add the dropped item to the player's inventory.
            gamemanager.Inventory.AddItem(drop.itemName, drop.quantity);

            // Tell the player what item was dropped.
            yield return StartCoroutine(TypeBattleText
            ("The" + enemyCombatant.combatantName + " droppped " + drop.itemName + " x " + drop.quantity + "!!")
            );
        }
    }
    
    // Determines which command the player selected from the main battle menu.
    private void SelectCommand()
    {
        // Attack command.
        if(currentSelection == 0 )
        {
            StartCoroutine(PlayerAttack());
        }

        // Magic command.
        else if(currentSelection == 1)
        {
            StartCoroutine(OpenMagicMenu());
        }

        // Item command.
        else if(currentSelection == 2)
        {
            StartCoroutine(OpenItemMenu());
        }

        // Run command.
        else if(currentSelection == 3)
        {
            StartCoroutine(Run());
        }
    }
}