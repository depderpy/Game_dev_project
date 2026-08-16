using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;
    [SerializeField] Dialog questCompleteDialog;
    [SerializeField] Quest quest;

    [SerializeField] QuestManager questManager;

    public void Interact()
    {
        if (quest != null)
        {
            //Start quest
            if (questManager.activeQuest == null)
            {
                questManager.StartQuest(quest);

                StartCoroutine(
                    DialogManager.Instance.ShowDialog(dialog)
                );

                return;
            }

            //Complete quest
            else if (questManager.CanCompleteQuest())
            {
                questManager.CompleteQuest();

                StartCoroutine(
                    DialogManager.Instance.ShowDialog(questCompleteDialog)
                );

                return;
            }
        }

        //Normal NPC dialogue
        StartCoroutine(
            DialogManager.Instance.ShowDialog(dialog)
        );
    }
}