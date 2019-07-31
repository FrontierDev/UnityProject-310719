using UnityEngine;
using GameUtilities;
using LitJson;
using System.IO;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class QuestDatabase : ScriptableObject
{
    // The list which contains the actual quest.
    [SerializeField]
    public List<Quest> Quests { get; set; }

    // Holds quest data that is pulled in from the JSON string
    JsonData questData;

    void Start()
    {
        ReloadDatabase();
    }

    /// <summary>
    /// Reloads the database, or creates the JSON file if it does not exist.
    /// </summary>
    public void ReloadDatabase()
    {
        Debug.Log("(Re)loading quest database...");

        if (Quests == null)
            Quests = new List<Quest>();

        questData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Quests.json"));

        if (questData == null)
            CreateJSONFile();

        CreateQuestDatabase();
    }

    void CreateJSONFile()
    {
        File.CreateText(Application.dataPath + "/StreamingAssets/Quests.json");
        ReloadDatabase();
    }

    /// <summary>
    /// Saves the database to a JSON file.
    /// </summary>
    public void SaveDatabase()
    {
        questData = JsonMapper.ToJson(this);
        File.WriteAllText(Application.dataPath + "/StreamingAssets/Quests.json", questData.ToString());
    }

    // This extracts information from the JSON database (through conditionData)
    void CreateQuestDatabase()
    {
        for (int i = 0; i < questData["Quests"].Count; i++)
        {
            if (!Contains((int)questData["Quests"][i]["QuestID"]))
            {
                Quest newQuest = new Quest();

                // Map each line in the ith JSON entry to a variable:
                newQuest.QuestName = (string)questData["Quests"][i]["QuestName"];
                newQuest.QuestID = (int)questData["Quests"][i]["QuestID"];
                newQuest.QuestShortDesc = (string)questData["Quests"][i]["QuestShortDesc"];
                newQuest.QuestCurrencyReward = (int)questData["Quests"][i]["QuestCurrencyReward"];

                // Get associated quest stages
                for (int j = 0; j < questData["Quests"][i]["QuestStages"].Count; j++)
                {
                    QuestStage newQuestStage = new QuestStage();

                    newQuestStage.StageName = (string)questData["Quest"][i]["QuestStage"][j]["StageName"];
                    newQuestStage.ProgressDescription = (string)questData["Quest"][i]["QuestStage"][j]["ProgressDescription"];
                    newQuestStage.FinishedDescription = (string)questData["Quest"][i]["QuestStage"][j]["FinishedDescription"];
                    newQuestStage.StageNumber = (int)questData["Quest"][i]["QuestStage"][j]["StageNumber"];
                    newQuestStage.StageType = (QuestStageType)((int)questData["Quest"][i]["QuestStage"][j]["StageType"]);

                    newQuest.QuestStages.Add(newQuestStage);
                }

                // Get associated requirements
                for (int m = 0; m < questData["Quests"][i]["QuestStages"].Count; m++)
                {
                    newQuest.QuestRequirements.Add((int)questData["Quests"][i]["QuestRequirements"][m]);
                }

                // Get associated item rewards
                for (int n = 0; n < questData["Quests"][i]["QuestStages"].Count; n++)
                {
                    newQuest.QuestItemRewards.Add((int)questData["Quests"][i]["QuestItemRewards"][n]);
                }

                // Add this quest to the database.
                AddQuest(newQuest);
                Debug.Log("(QuestDB) " + newQuest.QuestName + " loaded.");
            }
        }
    }

    public void AddQuest(Quest i)
    {
        Quests.Add(i);
    }

    public void DuplicateQuest(Quest i)
    {
        Quest duplicate = new Quest(i);

        duplicate.QuestName = duplicate.QuestName + "copy";
        while (Contains(duplicate.QuestID))
        {
            duplicate.QuestID++;
        }

        AddQuest(duplicate);
    }

    public void RemoveQuest(Quest i)
    {
        Quests.Remove(i);
    }

    // Get Quest reference by ID
    public Quest quest(int id)
    {
        foreach (Quest i in Quests)
        {
            if (i.QuestID == id)
            {
                return i;
            }
        }

        Debug.LogError("Quest with ID " + id + " does not exist in the database.");
        return null;
    }

    public bool Contains(int id)
    {
        foreach (Quest i in Quests)
        {
            if (i.QuestID == id)
            {
                return true;
            }
        }


        return false;
    }
}
