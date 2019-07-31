using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public class NPC {
    // NPC Name
    [SerializeField] public string NPCName { get; set; }
    // NPC ID
    [SerializeField] public int NPCID { get; set; }

    // NPC Inventory
    [SerializeField] public List<int> NPCInventoryIDs = new List<int>();
    [SerializeField] public List<int> NPCInventoryStacks = new List<int>();

    // NPC Hostile Factions (all other factions are considered friendly)
    [SerializeField] public List<int> NPCHostileTo = new List<int>();

    // NPC Dialog Options
    [SerializeField] public List<NPCDialog> NPCDialogText = new List<NPCDialog>();

    // Empty constructor
    public NPC()
    {

    }

    public NPC(string name, int id)
    {
        NPCName = name;
        NPCID = id;
    }

    // Constructor for duplication
    public NPC(NPC npc)
    {
        this.NPCName = npc.NPCName;
        this.NPCID = npc.NPCID;
        this.NPCHostileTo = npc.NPCHostileTo;
        this.NPCInventoryIDs = npc.NPCInventoryIDs;
        this.NPCInventoryStacks = npc.NPCInventoryStacks;
        this.NPCDialogText = npc.NPCDialogText;
    }

    public void SetNPCInventory (List<int> _ids, List<int> _stacks)
    {
        for(int i = 0; i < _ids.Count; i++)
        {
            NPCInventoryIDs.Add(_ids[i]);
            NPCInventoryStacks.Add(_stacks[i]);
        }
    }

    public bool HasDialogWithID(int id)
    {
        for(int i = 0; i < NPCDialogText.Count; i++)
        {
            if (id == NPCDialogText[i].DialogID)
                return true;
        }

        return false;
    }

    public NPCDialog GetDialogWithID(int id)
    {
        for (int i = 0; i < NPCDialogText.Count; i++)
        {
            if (id == NPCDialogText[i].DialogID)
                return NPCDialogText[i];
        }

        return null;
    }
}
