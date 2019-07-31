using UnityEngine;
using GameUtilities;
using LitJson;
using System.IO;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SkillDatabase : ScriptableObject {
    // The list which contains the actual skill.
    [SerializeField]
    public List<Skill> Skills { get; set; }

    // Holds skill data that is pulled in from the JSON string
    JsonData skillData;

    void Start() {
        ReloadDatabase();
    }

    /// <summary>
    /// Reloads the database, or creates the JSON file if it does not exist.
    /// </summary>
    public void ReloadDatabase() {
        Debug.Log("(Re)loading skill database...");

        if (Skills == null)
            Skills = new List<Skill>();

        skillData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Skills.json"));

        if (skillData == null)
            CreateJSONFile();

        CreateSkillDatabase();
    }

    void CreateJSONFile() {
        File.CreateText(Application.dataPath + "/StreamingAssets/Skills.json");
        ReloadDatabase();
    }

    /// <summary>
    /// Saves the database to a JSON file.
    /// </summary>
    public void SaveDatabase() {
        skillData = JsonMapper.ToJson(this);
        File.WriteAllText(Application.dataPath + "/StreamingAssets/Skills.json", skillData.ToString());
    }

    /// <summary>
    /// Reads the JSON file and creates the ScriptableObject in the Editor.
    /// </summary>
    void CreateSkillDatabase() {
        for (int i = 0; i < skillData["Skills"].Count; i++)
        {
            if (!Contains((int)skillData["Skills"][i]["SkillID"]))
            {
                Skill newSkill = new Skill();

                // Map each line in the ith JSON entry to a variable:
                newSkill.SkillName = (string)skillData["Skills"][i]["SkillName"];
                newSkill.SkillID = (int)skillData["Skills"][i]["SkillID"];
                newSkill.SkillShortDesc = (string)skillData["Skills"][i]["SkillShortDesc"];
                newSkill.SkillLongDesc = (string)skillData["Skills"][i]["SkillLongDesc"];
                newSkill.SkillIconPath = (string)skillData["Skills"][i]["SkillIconPath"];

                // Get associated perks.
                for (int j = 0; j < skillData["Skills"][i]["perkIDs"].Count; j++)
                {
                    newSkill.perkIDs.Add((int)skillData["Skills"][i]["perkIDs"][j]);
                }

                // Get skill icon
                newSkill.GetIcon();

                // Add this condition to the database.
                AddSkill(newSkill);
                //Debug.Log("(SkillDB) " + newSkill.SkillName + " loaded.");
            }
        }
    }

    public void AddSkill(Skill i) {
        Skills.Add(i);
    }

    public void RemoveSkill(Skill i) {
        Skills.Remove(i);
    }

    // Get Skill reference by ID
    public Skill skill(int id) {
        foreach (Skill i in Skills)
        {
            if (i.SkillID == id)
            {
                return i;
            }
        }

        Debug.LogError("Skill with ID " + id + " does not exist in the database.");
        return null;
    }

    /// <summary>
    /// Returns 'true' if the Spell Database contains a skill with the ID no. provided.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool Contains(int id)
    {
        foreach (Skill i in Skills)
        {
            if (i.SkillID == id)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns 'true' if the Skill Database contains a skill with the name provided.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool Contains(string name)
    {
        foreach (Skill i in Skills)
        {
            if (i.SkillName == name)
            {
                return true;
            }
        }
        return false;
    }
}
