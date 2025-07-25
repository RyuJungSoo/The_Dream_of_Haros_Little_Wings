using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionEvent : MonoBehaviour
{
    [SerializeField] DialogueEvent dialogue;

    public string GetName()
    {
        return dialogue.name;
    }

    public Dialogue[] GetDialogues()
    {
        dialogue.dialogues = DatabaseManager.instance.GetDialogues((int)dialogue.line.x, (int)dialogue.line.y);
        return dialogue.dialogues;
    }
}
