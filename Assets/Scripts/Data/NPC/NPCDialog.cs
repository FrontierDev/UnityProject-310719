using GameUtilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCDialog {
    [SerializeField] public int DialogID;                           // Unique ID for the NPC dialog (unique to NPC)
    [SerializeField] public string DialogOption;                    // The option used to get to THIS dialog output.
    [SerializeField] public string DialogOutput;                    // What the NPC says.
    [SerializeField] public List<int> DialogNext = new List<int>(); // The dialog options available.

    // NYI Skill requirements...
    // NYI Item requirements...
    // NYI Quest requirements...

    [SerializeField] public NPCDialogType DialogType;                       // What action (if any) this dialog triggers when called.
    [SerializeField] public int DialogTriggerID;                            // The ID of the item to give /quest to start/advance/finish.
    [SerializeField] public bool DialogTriggerOnFinish = false;             // Option to trigger action when the dialog ends.

    // Empty constructor
    public NPCDialog()
    {

    }

    public NPCDialog(NPCDialog dialog)
    {
        this.DialogID = dialog.DialogID;
        this.DialogOption = dialog.DialogOption;
        this.DialogOutput = dialog.DialogOutput;
        this.DialogNext = dialog.DialogNext;

        // NYI: Requirements

        this.DialogType = dialog.DialogType;
        this.DialogTriggerID = dialog.DialogTriggerID;
        this.DialogTriggerOnFinish = dialog.DialogTriggerOnFinish;
    }

    public void SetDialogLinks(List<int> links)
    {
        DialogNext.Clear();

        for(int i = 0; i < links.Count; i++)
        {
            DialogNext.Add(links[i]);
        }
    }

    public void AddDialogLink(int link)
    {
        DialogNext.Add(link);
    }
}
