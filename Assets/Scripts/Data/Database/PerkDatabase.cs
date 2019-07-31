using UnityEngine;
using GameUtilities;
using LitJson;
using System.IO;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PerkDatabase : ScriptableObject {
    // The list which contains the actual perks.
    [SerializeField]
    public List<Perk> Perks = new List<Perk>();

    // Holds perk data that is pulled in from the JSON string
    JsonData perkData;

    void Start() {
        ReloadDatabase();
    }

    /// <summary>
    /// Reloads the database, or creates the JSON file if it does not exist.
    /// </summary>
    public void ReloadDatabase() {
        Debug.Log("(Re)loading perk database...");

        if (Perks == null)
            Perks = new List<Perk>();

        perkData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Perks.json"));

        if (perkData == null)
            CreateJSONFile();

        CreatePerkDatabase();
    }

    void CreateJSONFile() {
        File.CreateText(Application.dataPath + "/StreamingAssets/Perks.json");
        ReloadDatabase();
    }

    /// <summary>
    /// Saves the database to a JSON file.
    /// </summary>
    public void SaveDatabase() {
        perkData = JsonMapper.ToJson(this);
        File.WriteAllText(Application.dataPath + "/StreamingAssets/Perks.json", perkData.ToString());
    }

    // This extracts information from the JSON database (through conditionData)
    void CreatePerkDatabase() {
        for (int i = 0; i < perkData["Perks"].Count; i++)
        {

            if(!Contains((int)perkData["Perks"][i]["PerkID"]))
            {
                Perk newPerk = new Perk();

                // Map each line in the ith JSON entry to a variable:
                newPerk.PerkName = (string)perkData["Perks"][i]["PerkName"];
                newPerk.PerkID = (int)perkData["Perks"][i]["PerkID"];
                newPerk.PerkDesc = (string)perkData["Perks"][i]["PerkDesc"];
                newPerk.PerkIconpath = (string)perkData["Perks"][i]["PerkIconpath"];
                newPerk.PerkUnlockLevel = (int)perkData["Perks"][i]["PerkUnlockLevel"];
                for (int j = 0; j < perkData["Perks"][i]["PerkAuras"].Count; j++)
                {
                    newPerk.PerkAuras.Add((int)perkData["Perks"][i]["PerkAuras"][j]);
                }


                // Load the perk icon.
                newPerk.LoadIcon();

                // Add this condition to the database.
                AddPerk(newPerk);
                //Debug.Log("(PerkDB) " + newPerk.PerkName + " loaded.");
            }
        }
    }

    public void AddPerk(Perk i) {
        Perks.Add(i);
    }

    public void DuplicatePerk(Perk i) {
        Perk duplicate = new Perk(i);

        duplicate.PerkName = duplicate.PerkName + "copy";
        while (Contains(duplicate.PerkID))
        {
            duplicate.PerkID++;
        }

        AddPerk(duplicate);
    }

    public void RemovePerk(Perk i) {
        Perks.Remove(i);
    }

    // Get Perk reference by ID
    public Perk perk(int id) {
        foreach (Perk i in Perks)
        {
            if (i.PerkID == id)
            {
                return i;
            }
        }

        Debug.LogError("Perk with ID " + id + " does not exist in the database.");
        return null;
    }

    public bool Contains(int id) {
        foreach (Perk i in Perks)
        {
            if (i.PerkID == id)
            {
                return true;
            }
        }


        return false;
    }
}
