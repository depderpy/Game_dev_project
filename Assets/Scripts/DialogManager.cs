using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] Text dialogText;
    [SerializeField] int lettersPerSecond;

    public event Action OnShowDialog;
    public event Action OnHideDialog;

    //Making this ENTIRE script public to EVERY other script in Unity
    //this is allow any of classes to use this DialogManager script (CAN CREATE BAD DEPENDANCIES DON'T USE TOO OFTEN)
    public static DialogManager Instance {get; private set;}
    Dialog dialog;
    int CurrentLine = 0;
    bool IsTyping;
    public void Awake()
    {
        Instance = this;
    }

    public void HandleUpdate()
    {
        //This will handle the typing issue (It will ensure the NPC moves onto the next line and it will keep typing the currentline until it finishes)
        if(Input.GetKeyDown(KeyCode.E) && !IsTyping())
        {
            ++CurrentLine;
            if(CurrentLine < dialog.Lines.Count)
            {
                StartCoroutine(TypeDialog(dialog.Lines[CurrentLine]));
            }
            else
            {
                dialogBox.SetActive(false);
                OnHideDialog?.Invoke();
            }
        }
    }

    public IEnumerator ShowDialog(Dialog dialog)
    {
        yield return new WaitForEndOfFrame();

        OnShowDialog?.Invoke();
        dialogBox.SetActive(true);
        StartCoroutine(TypeDialog(dialog.Lines[0]));
    }

    public IEnumerator TypeDialog(string line)
    {
        IsTyping = true;
        dialogText.text ="";
        foreach(var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        IsTyping = false;
        
    }

}
