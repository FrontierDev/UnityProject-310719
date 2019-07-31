using UnityEngine;
using GameUtilities;
using LitJson;
using System.IO;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AuraDatabase : ScriptableObject {
    // The list which contains the actual items.
    [SerializeField]
	public List<Aura> Auras { get; set; }

    // Holds item data that is pulled in from the JSON string
    JsonData auraData;

    void Start() {
        ReloadDatabase();
    }

    /// <summary>
    /// Reloads the database, or creates the JSON file if it does not exist.
    /// </summary>
    public void ReloadDatabase() {
        Debug.Log("(Re)loading aura database...");

        if (Auras == null)
            Auras = new List<Aura>();

        auraData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Auras.json"));

        if (auraData == null)
            CreateJSONFile();

        CreateAuraDatabase();
    }

    void CreateJSONFile() {
        File.CreateText(Application.dataPath + "/StreamingAssets/Auras.json");
        ReloadDatabase();
    }

    /// <summary>
    /// Saves the database to a JSON file.
    /// </summary>
    public void SaveDatabase() {
        auraData = JsonMapper.ToJson(this);
        File.WriteAllText(Application.dataPath + "/StreamingAssets/Auras.json", auraData.ToString());
    }

    // This extracts information from the JSON database (through auraData)
    void CreateAuraDatabase() {
        for (int i = 0; i < auraData["Auras"].Count; i++)
        {
            if(!Contains((int)auraData["Auras"][i]["AuraID"]))
            {
                Aura newAura = new Aura();

                // Map each line in the ith JSON entry to a variable:
                newAura.AuraName = (string)auraData["Auras"][i]["AuraName"];
                newAura.AuraID = (int)auraData["Auras"][i]["AuraID"];
                newAura.AuraDesc = (string)auraData["Auras"][i]["AuraDesc"];
                newAura.AuraDuration = (double)auraData["Auras"][i]["AuraDuration"];
                newAura.AuraStat = (AuraStat)((int)auraData["Auras"][i]["AuraStat"]);
                newAura.AuraValue = (int)auraData["Auras"][i]["AuraValue"];
                newAura.IsHarmful = (bool)auraData["Auras"][i]["IsHarmful"];
                newAura.HasDuration = (bool)auraData["Auras"][i]["HasDuration"];
                newAura.AuraSkill = (string)auraData["Auras"][i]["AuraSkill"];

                // Add this aura to the database.
                AddAura(newAura);

                //Debug.Log("(AuraDB) " + newAura.AuraName + " loaded.");
            }
        }
    }

    public void AddAura(Aura i) {
        Auras.Add(i);
    }

    public void DuplicateAura(Aura i) {
        Aura duplicate = new Aura(i);

        duplicate.AuraName = duplicate.AuraName + "copy";
        while (Contains(duplicate.AuraID))
        {
            duplicate.AuraID++;
        }

        AddAura(duplicate);
    }

    public void RemoveAura(Aura i) {
        Auras.Remove(i);
    }

    // Get Item reference by ID
    public Aura Aura(int id) {
        foreach (Aura i in Auras)
        {
            if (i.AuraID == id)
            {
                return i;
            }
        }

        Debug.LogError("Aura with ID " + id + " does not exist in the database.");
        return null;
    }

    public bool Contains(int id) {
        foreach (Aura i in Auras)
        {
            if (i.AuraID == id)
            {
                return true;
            }
        }
        return false;
    }

    public bool Contains(string name) {
        foreach (Aura i in Auras)
        {
            if (i.AuraName == name)
            {
                return true;
            }
        }
        return false;
    }
}
