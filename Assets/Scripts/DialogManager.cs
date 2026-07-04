using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] Text dialogText;

    [SerializeField] int lettersPerSecond;

    //Making this ENTIRE script public to EVERY other script in Unity
    //this is allow any of classes to use this DialogManager script (CAN CREATE BAD DEPENDANCIES DON'T USE TOO OFTEN)
    public static DialogManager Instance {get; private set;}

    public void Awake()
    {
        Instance = this;
    }

    public void ShowDialog(Dialog dialog)
    {
        dialogBox.SetActive(true);
        StartCoroutine(TypeDialog(dialog.Lines[0]));
    }

    public IEnumerator TypeDialog(string line)
    {
        dialogText.text ="";
        foreach(var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        
    }

}
