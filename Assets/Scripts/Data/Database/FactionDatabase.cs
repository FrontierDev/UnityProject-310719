using UnityEngine;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class FactionDatabase : ScriptableObject {
    // The list which contains the actual items.
    [SerializeField]
    public List<Faction> Factions { get; set; }

    // Holds item data that is pulled in from the JSON string
    JsonData factionData;

    void Start() {
        ReloadDatabase();
    }

    /// <summary>
    /// Reloads the database, or creates the JSON file if it does not exist.
    /// </summary>
    public void ReloadDatabase() {
        Debug.Log("(Re)loading faction database...");

        if (Factions == null)
            Factions = new List<Faction>();

        factionData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/FactionsBuiltIn.json"));

        if (factionData == null)
            CreateJSONFile();

        CreateFactionDatabase();
    }

    void CreateJSONFile() {
        File.CreateText(Application.dataPath + "/StreamingAssets/FactionsBuiltIn.json");
        ReloadDatabase();
    }

    /// <summary>
    /// Saves the database to a JSON file.
    /// </summary>
    public void SaveDatabase() {
        factionData = JsonMapper.ToJson(this);
        File.WriteAllText(Application.dataPath + "/StreamingAssets/FactionsBuiltIn.json", factionData.ToString());
    }

    void CreateFactionDatabase() {
        for (int i = 0; i < factionData["Factions"].Count; i++)
        {
            if (!Contains((int)factionData["Factions"][i]["FactionID"]))
            {
                Faction newFaction = new Faction();

                // Map each line in the ith JSON entry to a variable:
                newFaction.FactionName = (string)factionData["Factions"][i]["FactionName"];
                newFaction.FactionID = (int)factionData["Factions"][i]["FactionID"];
                newFaction.FactionDescPath = (string)factionData["Factions"][i]["FactionDescPath"];
                newFaction.FactionIconPath = (string)factionData["Factions"][i]["FactionIconPath"];
                newFaction.FactionLeader = (string)factionData["Factions"][i]["FactionLeader"];

                // Get the faction's constituent races:
                if (factionData["Factions"][i]["FactionRaceList"].Count != 0)
                {
                    for(int j = 0; j < factionData["Factions"][i]["FactionRaceList"].Count; j++)
                    {
                        newFaction.FactionRaceList.Add((string)factionData["Factions"][i]["FactionRaceList"][j]);
                    }
                }

                newFaction.LoadIcon();

                Factions.Add(newFaction);
                Debug.Log("(FctnDB) " + newFaction.FactionName + " loaded.");
            }
        }
    }

    // Get Faction reference by ID
    public Faction Faction(int id) {
        foreach (Faction i in Factions)
        {
            if (i.FactionID == id)
            {
                return i;
            }
        }

        Debug.LogError("GameItem with ID " + id + " does not exist in the database.");
        return null;
    }

    public bool Contains(int id) {
        foreach (Faction i in Factions)
        {
            if (i.FactionID == id)
            {
                return true;
            }
        }

        return false;
    }
}
