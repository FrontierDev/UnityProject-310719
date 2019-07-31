using UnityEngine;
using GameUtilities;
using LitJson;
using System.IO;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class NPCDatabase : ScriptableObject
{
    // The list which contains the actual npc.
    [SerializeField]
    public List<NPC> NPCs { get; set; }

    // Holds npc data that is pulled in from the JSON string
    JsonData npcData;

    void Start()
    {
        ReloadDatabase();
    }

    /// <summary>
    /// Reloads the database, or creates the JSON file if it does not exist.
    /// </summary>
    public void ReloadDatabase()
    {
        Debug.Log("(Re)loading npc database...");

        if (NPCs == null)
            NPCs = new List<NPC>();

        npcData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/NPCs.json"));

        if (npcData == null)
            CreateJSONFile();

        CreateNPCDatabase();
    }

    void CreateJSONFile()
    {
        File.CreateText(Application.dataPath + "/StreamingAssets/NPCs.json");
        ReloadDatabase();
    }

    /// <summary>
    /// Saves the database to a JSON file.
    /// </summary>
    public void SaveDatabase()
    {
        npcData = JsonMapper.ToJson(this);
        File.WriteAllText(Application.dataPath + "/StreamingAssets/NPCs.json", npcData.ToString());
    }

    // This extracts information from the JSON database (through conditionData)
    void CreateNPCDatabase()
    {
        for (int i = 0; i < npcData["NPCs"].Count; i++)
        {
            if (!Contains((int)npcData["NPCs"][i]["NPCID"]))
            {
                NPC newNPC = new NPC();

                // Map each line in the ith JSON entry to a variable:
                newNPC.NPCName = (string)npcData["NPCs"][i]["NPCName"];
                newNPC.NPCID = (int)npcData["NPCs"][i]["NPCID"];

                // Load hostile factions list
                for(int j = 0; j < npcData["NPCs"][i]["NPCHostileTo"].Count; j++)
                {
                    newNPC.NPCHostileTo.Add((int)npcData["NPCs"][i]["NPCHostileTo"][j]);
                }

                // Load NPC inventory
                //Debug.Log(npcData["NPCs"][i]["NPCInventoryIDs"].Count);
                for(int k = 0; k < npcData["NPCs"][i]["NPCInventoryIDs"].Count; k++)
                {
                    int _id = (int)npcData["NPCs"][i]["NPCInventoryIDs"][k];
                    int _quant = (int)npcData["NPCs"][i]["NPCInventoryStacks"][k];

                    newNPC.NPCInventoryIDs.Add(_id);
                    newNPC.NPCInventoryStacks.Add(_quant);
                }

                // Load NPC dialog
                for (int m = 0; m < npcData["NPCs"][i]["NPCDialogText"].Count; m++)
                {
                    NPCDialog _dialog = new NPCDialog();

                    _dialog.DialogID = (int)npcData["NPCs"][i]["NPCDialogText"][m]["DialogID"];
                    _dialog.DialogOption = (string)npcData["NPCs"][i]["NPCDialogText"][m]["DialogOption"];
                    _dialog.DialogOutput = (string)npcData["NPCs"][i]["NPCDialogText"][m]["DialogOutput"];
                    _dialog.DialogTriggerID = (int)npcData["NPCs"][i]["NPCDialogText"][m]["DialogTriggerID"];
                    _dialog.DialogTriggerOnFinish = (bool)npcData["NPCs"][i]["NPCDialogText"][m]["DialogTriggerOnFinish"];
                    _dialog.DialogType = (NPCDialogType)((int)npcData["NPCs"][i]["NPCDialogText"][m]["DialogType"]);
                    
                    for(int _m = 0; _m < npcData["NPCs"][i]["NPCDialogText"][m]["DialogNext"].Count; _m++)
                    {
                        _dialog.DialogNext.Add((int)npcData["NPCs"][i]["NPCDialogText"][m]["DialogNext"][_m]);
                    }

                    newNPC.NPCDialogText.Add(_dialog);
                }

                // Add this npc to the database.
                AddNPC(newNPC);
                //Debug.Log("(NPCDB) " + newNPC.NPCName + " loaded.");
            }
        }
    }

    public void AddNPC(NPC i)
    {
        NPCs.Add(i);
    }

    public void DuplicateNPC(NPC i)
    {
        NPC duplicate = new NPC(i);

        duplicate.NPCName = duplicate.NPCName + "copy";
        while (Contains(duplicate.NPCID))
        {
            duplicate.NPCID++;
        }

        AddNPC(duplicate);
    }

    public void RemoveNPC(NPC i)
    {
        NPCs.Remove(i);
    }

    // Get NPC reference by ID
    public NPC npc(int id)
    {
        foreach (NPC i in NPCs)
        {
            if (i.NPCID == id)
            {
                return i;
            }
        }

        Debug.LogError("NPC with ID " + id + " does not exist in the database.");
        return null;
    }

    public bool Contains(int id)
    {
        foreach (NPC i in NPCs)
        {
            if (i.NPCID == id)
            {
                return true;
            }
        }


        return false;
    }
}
