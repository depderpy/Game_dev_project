using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Allows this class to be serialized by Unity.
// This means a Dialog object can be created and edited directly
// in the Unity Inspector, even though it is not a MonoBehaviour.
[System.Serializable]


public class Dialog
{
    // Stores all of the dialogue lines for this particular Dialog.
    // Each string represents one line of dialogue that can be displayed
    // one after another by the DialogManager.
    //
    // SerializeField allows the private list to appear in the Unity Inspector
    // so dialogue lines can be entered without modifying the code.
    [SerializeField] List<string> lines;

    // Public property that allows other scripts, such as DialogManager,
    // to access the list of dialogue lines.
    //
    // The "get" means other scripts can read the dialogue lines,
    // but cannot directly replace the entire list through this property.
    public List<string> Lines
    {
        // Returns the list containing all dialogue lines.
        get {return lines;}
    }
    
}