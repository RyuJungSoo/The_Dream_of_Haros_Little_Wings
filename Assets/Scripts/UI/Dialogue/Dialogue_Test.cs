using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue_Test : MonoBehaviour
{
    Dialogue[] dialogues;
    int name_index = 0;
    int contexts_index = 0;
    string name;
    string context;
    bool isFinish = true;

    public void ShowDialogue(Dialogue[] p_dialogues)
    {
        dialogues = p_dialogues;
    }

    // Start is called before the first frame update
    void Start()
    {
        ShowDialogue(GetComponent<InteractionEvent>().GetDialogues());
    }
}
