using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    // Reference to the GameObject that contains the dialogue UI.
    // This allows the script to show and hide the entire dialogue box
    // when dialogue starts or finishes.
    [SerializeField] GameObject dialogBox;

    // Reference to the UI Text component where the current dialogue
    // line will be displayed.
    [SerializeField] Text dialogText;

    // Controls how quickly each character appears on screen.
    // A higher value means the dialogue types out faster.
    [SerializeField] int lettersPerSecond;


    // Event that is triggered when dialogue begins.
    // Other systems, such as the GameManager, can listen to this event
    // and change the current game state to Dialog.
    public event Action OnShowDialog;

    // Event that is triggered when dialogue has completely finished.
    // Other systems can listen to this event and return the game
    // to the appropriate game state.
    public event Action OnHideDialog;


    // Making this ENTIRE script public to EVERY other script in Unity
    // this is allow any of classes to use this DialogManager script (CAN CREATE BAD DEPENDANCIES DON'T USE TOO OFTEN)
    //
    // This creates a Singleton reference to the DialogManager.
    // Instead of every script needing its own reference to the DialogManager,
    // other scripts can access it through:
    //
    // DialogManager.Instance
    //
    // The "private set" means other scripts can access the Instance,
    // but they cannot change which DialogManager it points to.
    public static DialogManager Instance {get; private set;}


    // Stores the Dialog currently being displayed.
    //
    // This allows the DialogManager to know which dialogue lines
    // it should display when the player presses E.
    Dialog dialog;


    // Keeps track of which dialogue line is currently being displayed.
    //
    // For example:
    // 0 = first line
    // 1 = second line
    // 2 = third line
    //
    // It starts at 0 because dialogue begins with the first line.
    int CurrentLine = 0;


    // Keeps track of whether the current dialogue line is still being typed.
    //
    // This prevents the player from pressing E and accidentally
    // skipping to the next dialogue line while the current line
    // is still being displayed.
    bool IsTyping;


    // Awake is called when the GameObject is initialized.
    // It happens before Start().
    public void Awake()
    {
        // Assigns this DialogManager to the Singleton Instance.
        //
        // This allows other scripts to access this specific
        // DialogManager using DialogManager.Instance.
        Instance = this;
    }


    // HandleUpdate is called by the GameManager when the game
    // is currently in the Dialog state.
    //
    // It checks for player input while dialogue is active.
    public void HandleUpdate()
    {
        // This will handle the typing issue
        // (It will ensure the NPC moves onto the next line and
        // it will keep typing the currentline until it finishes)
        //
        // Input.GetKeyDown checks whether E was pressed during
        // the current frame.
        //
        // !IsTyping ensures that the player cannot move to the next
        // dialogue line while the current line is still being typed.
        if(Input.GetKeyDown(KeyCode.E) && !IsTyping)
        {
            // Move to the next dialogue line.
            //
            // The ++ operator increases CurrentLine by 1.
            ++CurrentLine;


            // Checks whether there are still more dialogue lines available.
            //
            // dialog.Lines.Count gives the total number of dialogue lines.
            // If CurrentLine is smaller than that number, another line exists.
            if(CurrentLine < dialog.Lines.Count)
            {
                // Starts the coroutine that types out the next dialogue line.
                StartCoroutine(TypeDialog(dialog.Lines[CurrentLine]));
            }
            else
            {
                // No more dialogue lines are available,
                // so the dialogue box is hidden.
                dialogBox.SetActive(false);


                // Resets CurrentLine back to 0.
                //
                // This allows the same dialogue to start from the
                // beginning the next time the NPC is interacted with.
                CurrentLine = 0;


                // Notifies other scripts that the dialogue has ended.
                //
                // The ?. prevents an error if no other script
                // is currently listening to this event.
                OnHideDialog?.Invoke();
            }
        }
    }


    // Starts displaying a new Dialog.
    //
    // This method receives a Dialog object containing a list
    // of dialogue lines.
    public IEnumerator ShowDialog(Dialog dialog)
    {
        // Waits until the end of the current frame before continuing.
        //
        // This can help ensure that other changes made during the
        // current frame have finished before the dialogue starts.
        yield return new WaitForEndOfFrame();


        // Notifies other scripts that dialogue has started.
        //
        // For example, the GameManager uses this to change
        // the game state to Dialog.
        OnShowDialog?.Invoke();


        // Stores the Dialog passed into this method as the
        // currently active dialogue.
        //
        // "this.dialog" refers to the DialogManager's variable,
        // while "dialog" refers to the method parameter.
        this.dialog = dialog;


        // Makes the dialogue UI visible on screen.
        dialogBox.SetActive(true);


        // Starts the coroutine that displays the first dialogue line.
        //
        // dialog.Lines[0] accesses the first line in the Dialog's list.
        StartCoroutine(TypeDialog(dialog.Lines[0]));
    }


    // Types a dialogue line onto the screen one character at a time.
    //
    // This creates the typewriter effect used for the NPC dialogue.
    public IEnumerator TypeDialog(string line)
    {
        // Indicates that the dialogue is currently being typed.
        //
        // HandleUpdate uses this value to prevent the player
        // from moving to the next line too early.
        IsTyping = true;


        // Clears any text currently displayed in the dialogue box.
        //
        // This ensures the new dialogue line starts empty.
        dialogText.text ="";


        // Goes through every character in the dialogue line.
        //
        // "letter" represents the current character being processed.
        foreach(var letter in line)
        {
            // Adds the current character to the text displayed on screen.
            //
            // Because this happens one character at a time,
            // the player sees the typewriter effect.
            dialogText.text += letter;


            // Waits for a short amount of time before displaying
            // the next character.
            //
            // 1f / lettersPerSecond determines the delay between
            // each character.
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }


        // The entire dialogue line has finished typing.
        //
        // Setting this to false allows the player to press E
        // and move to the next dialogue line.
        IsTyping = false;
        
    }

}