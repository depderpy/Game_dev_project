using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Controls how an NPC behaves when the player interacts with them.
// Implements the Interactable interface, meaning this NPC must contain
// an Interact() method that can be called by the player's interaction system.
public class NPCController : MonoBehaviour, Interactable
{
    // The normal dialogue shown by the NPC.
    // This can be assigned through the Unity Inspector.
    [SerializeField] Dialog dialog;

    // The dialogue shown when the player has completed the quest.
    // This is separate from the normal dialogue so the NPC can acknowledge
    // that the player has successfully completed the quest.
    [SerializeField] Dialog questCompleteDialog;

    // The quest associated with this NPC.
    // If no quest is assigned, this NPC simply acts as a normal dialogue NPC.
    [SerializeField] Quest quest;

    // Reference to the QuestManager.
    // This is used to start quests, check whether quests can be completed,
    // and complete quests.
    [SerializeField] QuestManager questManager;

    // Called when the player interacts with this NPC.
    // The method checks the current state of the NPC's quest and determines
    // which dialogue should be shown.
    public void Interact()
    {
        // Check whether this NPC actually has a quest assigned.
        if (quest != null)
        {
            // Start quest
            // If the player currently has no active quest, this NPC
            // starts its assigned quest.
            if (questManager.activeQuest == null)
            {
                // Sends the quest to the QuestManager so it becomes
                // the player's current active quest.
                questManager.StartQuest(quest);

                // Displays the normal dialogue associated with this NPC.
                StartCoroutine(
                    DialogManager.Instance.ShowDialog(dialog)
                );

                // Stops the rest of this method from running.
                // This prevents the completion check and normal dialogue
                // from being processed again during this interaction.
                return;
            }

            // Complete quest
            // If the player already has an active quest, check whether
            // they have the required items/conditions to complete it.
            else if (questManager.CanCompleteQuest())
            {
                // Completes the quest through the QuestManager.
                // This also handles the quest's rewards.
                questManager.CompleteQuest();

                // Shows the special dialogue confirming that the quest
                // has been successfully completed.
                StartCoroutine(
                    DialogManager.Instance.ShowDialog(questCompleteDialog)
                );

                // Stops the rest of this method from running so the
                // normal NPC dialogue is not shown as well.
                return;
            }
        }

        // Normal NPC dialogue
        // If the NPC does not have a quest, or the quest cannot currently
        // be completed, the NPC displays its normal dialogue.
        StartCoroutine(
            DialogManager.Instance.ShowDialog(dialog)
        );
    }
}