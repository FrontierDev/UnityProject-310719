using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorNPC : MonoBehaviour {
    NPC npc;

    // Dialog
    List<NPCDialog> dialog = new List<NPCDialog>();
    int currentDialog = 0;  // Always starts at zero.

    public int npcID = -1;
    public bool npcLoaded = false;

    void Start () {
        if(npcID != -1)
        {
            GameDatabase db = GameObject.FindGameObjectWithTag("Database").GetComponent<GameDatabase>();
            npc = db.npcdb.npc(npcID);
            LoadNPC(npc);
        }
    }

    public void LoadNPC (NPC _npc) {
        npc = _npc;
        dialog = _npc.NPCDialogText;

        npcLoaded = true;
    }
    public string GetNPCName() {
        return npc.NPCName;
    }

    // Interacts with the NPC, i.e., opens dialog.
    public void Interact() {
        NPCDialog current = dialog.Find(x => x.DialogID == currentDialog);

        Debug.LogWarning(npc.NPCName + " says: " + current.DialogOutput); // FOR TESTING. Puts the dialog output as a warning in the debug window.

        DisplayDialogOptions(current.DialogNext);

        ChangeDialog(current.DialogNext[0]);
    }

    void DisplayDialogOptions(List<int> options) {
        for (int i = 0; i < options.Count; i++)
        {
            NPCDialog option = dialog.Find(x => x.DialogID == options[i]);
            Debug.LogWarning("Option " + (i+1) + ": " + option.DialogOption);
        }
    }

    public void ChangeDialog(int next) {
        currentDialog = next;
    }
}
