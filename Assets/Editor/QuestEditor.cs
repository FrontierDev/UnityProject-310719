using UnityEngine;
using UnityEditor;
using LitJson;
using GameUtilities;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class QuestEditor : EditorWindow
{
    private QuestDatabase questDatabase;
    //private List<Quest> conditions = new List<Quest>();
    private string QuestAssetPath = "Assets/questdb.asset";

    #region Auxillary Databases
    private ItemDatabase _itemDatabase;
    private string itemPath = "Assets/itemdb.asset";
    #endregion

    private EditorState editorState;
    private Quest selectedQuest;
    private QuestStage selectedQuestStage;
    enum EditorState { Home, Create, Edit }
    Vector2 listScrollPos;
    Vector2 editScrollPos;
    Vector2 questStagesScrolLPos;
    Vector2 addStagesScrollPos;
    Vector2 addedRewardsScrollPos;
    bool showQuestStages;
    bool showAddQuestStage;
    bool showQuestRewards;
    bool showEditQuestStage;

    #region Quest Properties
    string questName;
    int questID;
    string questShortDesc;
    int questCurrencyReward;
    List<QuestStage> questStages = new List<QuestStage>();
    List<int> questRequirements = new List<int>();
    List<int> questItemRewards = new List<int>();
    #endregion

    #region Quest Stage Propeties
    string stageName;
    string stageShortDesc;
    int stageNum;
    string stageProgDesc;
    string stageFinDesc;
    QuestStageType stageType = QuestStageType.Dialog;
    int stageTargetID;
    int stageTargetQuant;
    List<int> targetIDList = new List<int>();
    List<int> targetQuantityList = new List<int>();
    #endregion

    int itemID;


    // Add menu named "Quest Editor" to the Window menu
    [MenuItem("Window/Quest Editor")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        QuestEditor editor = (QuestEditor)EditorWindow.GetWindow(typeof(QuestEditor));
        editor.minSize = new Vector2(1500, 600);
        editor.Show();
    }

    void Awake()
    {
        LoadQuestDatabase();
        LoadAuxillaryDatabases();
    }

    void LoadQuestDatabase()
    {
        questDatabase = AssetDatabase.LoadAssetAtPath<QuestDatabase>(QuestAssetPath);

        if (questDatabase == null)
            CreateQuestDatabase();
        else
        {
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = questDatabase;
        }
    }

    void LoadAuxillaryDatabases()
    {
        // ITEM database
        _itemDatabase = AssetDatabase.LoadAssetAtPath<ItemDatabase>(itemPath);
        _itemDatabase.ReloadDatabase();
    }

    void CreateQuestDatabase()
    {
        Debug.Log("Creating Quest database...");

        questDatabase = ScriptableObject.CreateInstance<QuestDatabase>();
        Debug.Log(questDatabase);

        AssetDatabase.CreateAsset(questDatabase, "Assets/questdb.asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = questDatabase;
    }

    void OnEnable()
    {

    }

    void OnGUI()
    {
        /*
         * Editor toolbar
         */
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create New Quest", GUILayout.Width(300)))
        {
            editorState = EditorState.Create;
            return;
        }
        if (GUILayout.Button("Reload Database", GUILayout.Width(300)))
        {
            questDatabase.ReloadDatabase();
            return;
        }
        if (GUILayout.Button("Save to JSON", GUILayout.Width(300)))
        {
            // Delete this item from the database.
            questDatabase.SaveDatabase();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        EditorGUILayout.EndHorizontal();

        if (questDatabase == null || questDatabase.Quests == null)
        {
            EditorGUILayout.LabelField("The database may need reloading.");
            return;
        }

        EditorGUILayout.BeginHorizontal();
        // List all of the items on the left hand side.
        listScrollPos = EditorGUILayout.BeginScrollView(listScrollPos, false, false, GUILayout.Width(450), GUILayout.MinHeight(550));
        foreach (Quest i in questDatabase.Quests)
        {
            // Horizontal group per condition.
            EditorGUILayout.BeginHorizontal(GUILayout.Width(400.0f));

            if (GUILayout.Button("X", GUILayout.Width(50.0f)))
            {
                // Delete this item from the database.
                questDatabase.RemoveQuest(i);
                EditorUtility.SetDirty(questDatabase);
                AssetDatabase.SaveAssets();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = questDatabase;
                return;
            }

            if (GUILayout.Button("C", GUILayout.Width(50.0f)))
            {
                // Duplicate this item.
                questDatabase.DuplicateQuest(i);
                EditorUtility.SetDirty(questDatabase);
                AssetDatabase.SaveAssets();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = questDatabase;
                return;
            }

            if (GUILayout.Button(i.QuestName.ToString(), GUILayout.Width(300)))
            {
                if (editorState == EditorState.Edit)
                    SaveExistingQuest();
                else if (editorState == EditorState.Create)
                    SaveNewQuest();

                //Get the new item and its associated data.
                selectedQuest = i;
                GetQuestData();
                if (selectedQuest.QuestStages.Count > 0)
                    showQuestStages = true;

                EditorUtility.FocusProjectWindow();
                Selection.activeObject = questDatabase;

                editorState = EditorState.Edit;

                return;
            }

            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        if (editorState == EditorState.Create || editorState == EditorState.Edit)
            ShowCreateWindow();

        EditorGUILayout.EndHorizontal();
    }

    void ShowCreateWindow()
    {
        editScrollPos = EditorGUILayout.BeginScrollView(editScrollPos, false, false, GUILayout.MinWidth(540), GUILayout.MinHeight(550));

        questName = EditorGUILayout.TextField("Name: ", questName, GUILayout.Width(300));
        questID = EditorGUILayout.IntField("ID: ", questID, GUILayout.Width(300));
        questShortDesc = EditorGUILayout.TextField("Description: ", questShortDesc, GUILayout.Width(450));

        EditorGUILayout.Space();

        /*  
         *  QUEST REQUIREMENTS
         */
        //showRequirements = EditorGUILayout.Foldout(showRequirements, "REQUIRED SKILLS");
        //if (showRequirements)
        //    DisplayRequirements();
        //EditorGUILayout.Space();

        /*
         *  QUEST REWARDS
         */
        showQuestRewards = EditorGUILayout.Foldout(showQuestRewards, "QUEST REWARDS");
        if (showQuestRewards)
            DisplayQuestRewards();


        /*
         *  SAVED /CANCEL Buttons
         */
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save", GUILayout.Width(150.0f)))
        {
            // Save this item to the database, either as a new item
            // or as an existing item.
            if (editorState == EditorState.Create)
                SaveNewQuest();
            else
                SaveExistingQuest();

            EditorUtility.SetDirty(questDatabase);
            editorState = EditorState.Home;
        }
        if (GUILayout.Button("Cancel", GUILayout.Width(150.0f)))
        {
            EditorUtility.SetDirty(questDatabase);
            editorState = EditorState.Home;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndScrollView();

        /*
         *  QUEST STAGES
         */
        questStagesScrolLPos = EditorGUILayout.BeginScrollView(questStagesScrolLPos, false, false, GUILayout.MinWidth(540), GUILayout.MinHeight(550));

        showQuestStages = EditorGUILayout.Foldout(showQuestStages, "QUEST STAGES");
        if (showQuestStages)
            DisplayQuestStages();

        EditorGUILayout.EndScrollView();
    }

    void DisplayQuestRewards() {
        EditorGUILayout.LabelField("Current Item Rewards", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical();

        // Display the currently-added items
        addedRewardsScrollPos = EditorGUILayout.BeginScrollView(addedRewardsScrollPos, false, false, GUILayout.Width(400), GUILayout.Height(100));
        for (int i = 0; i < questItemRewards.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("X", GUILayout.Width(50)))
            {
                questItemRewards.RemoveAt(i);
                break;
            }
            EditorGUILayout.LabelField(string.Format("#{0}: <item name>", questItemRewards[i].ToString()));
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        // Button for adding a new item:
        EditorGUILayout.BeginHorizontal();
        itemID = EditorGUILayout.IntField("ID: ", itemID, GUILayout.Width(200));
        
        if (GUILayout.Button("+", GUILayout.Width(50)))
        {
            if(_itemDatabase.Contains(itemID))
            {
                questItemRewards.Add(itemID);
                itemID = 0;
            }
            else
            {
                Debug.LogWarning("Item database does not contain item with ID " + itemID);
                itemID = 0;
            }
            
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    void DisplayQuestStages() {
        EditorGUILayout.LabelField("Current Stages", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical();

        // Display the currently-added quest stages
        addStagesScrollPos = EditorGUILayout.BeginScrollView(addStagesScrollPos, false, false, GUILayout.Width(400), GUILayout.Height(150), GUILayout.ExpandHeight(true));
        for (int i = 0; i < questStages.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("X", GUILayout.Width(50)))
            {
                questStages.RemoveAt(i);
                break;
            }
            if (GUILayout.Button("E", GUILayout.Width(50)))
            {
                showEditQuestStage = true;
                showAddQuestStage = false;
                ResetQuestStageFields();
                selectedQuestStage = questStages[i];
                GetQuestStageData();
                break;
            }
            EditorGUILayout.LabelField(string.Format("#{0}: {1} \n{2}", questStages[i].StageNumber.ToString(), questStages[i].StageName, questStages[i].ShortDescription), GUILayout.ExpandHeight(true));
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        // Button for adding a new stage:
        if (!showAddQuestStage)
        {
            if (GUILayout.Button("New Quest Stage", GUILayout.Width(300)))
            {
                ResetQuestStageFields();
                showAddQuestStage = true;
                showEditQuestStage = false;
            }
        }
        else
        {
            ShowAddQuestStage();
        }

        if (showEditQuestStage)
            ShowEditQuestStage();

        EditorGUILayout.EndVertical();
    }

    void ShowAddQuestStage() {
        stageName = EditorGUILayout.TextField("Name: ", stageName, GUILayout.Width(295));
        stageNum = EditorGUILayout.IntField("Number: ", stageNum, GUILayout.Width(295));
        stageShortDesc = EditorGUILayout.TextField("Short Desc: ", stageShortDesc, GUILayout.Width(295));

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Type and Objectives", EditorStyles.boldLabel);
        stageType = (QuestStageType)EditorGUILayout.EnumPopup("Type: ", stageType, GUILayout.Width(480));
        switch(stageType)
        {
            case (QuestStageType.Dialog):
                break;

            case (QuestStageType.CollectItem):
            case (QuestStageType.GiveItem_Single):
            case (QuestStageType.Kill_Single):
                stageTargetID = EditorGUILayout.IntField("Item/NPC: ", stageTargetID, GUILayout.Width(295));
                break;

            case (QuestStageType.GiveItem_Multiple):
            case (QuestStageType.Kill_Multiple):
                for (int i = 0; i < targetIDList.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("X", GUILayout.Width(50)))
                    {
                        targetIDList.RemoveAt(i);
                        targetQuantityList.RemoveAt(i);
                        break;
                    }

                    EditorGUILayout.LabelField(string.Format("Item {0} x{1}", targetIDList[i].ToString(), targetQuantityList[i].ToString()));
                    EditorGUILayout.EndHorizontal();
                }

                stageTargetID = EditorGUILayout.IntField("Item/NPC: ", stageTargetID, GUILayout.Width(295));
                stageTargetQuant = EditorGUILayout.IntField("# req: ", stageTargetQuant, GUILayout.Width(295));

                if (GUILayout.Button("+", GUILayout.Width(50)))
                {
                    if (!targetIDList.Contains(stageTargetID))
                    {
                        targetIDList.Add(stageTargetID);
                        targetQuantityList.Add(stageTargetQuant);
                    }

                    else
                        Debug.LogError("Cannot add target item/npc: already exists in quest stage.");

                    stageTargetID = -1;
                    stageTargetQuant = 0;
                    break;
                }
                break;
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Quest Log Text", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Progress description:");
        stageProgDesc = EditorGUILayout.TextArea(stageProgDesc, GUILayout.Width(350), GUILayout.MinHeight(90));
        EditorGUILayout.LabelField("Finished description:");
        stageFinDesc = EditorGUILayout.TextArea(stageFinDesc, GUILayout.Width(350), GUILayout.MinHeight(90));

        if (GUILayout.Button("Add Stage", GUILayout.Width(300)))
        {
            for(int i = 0; i < questStages.Count; i++)
            {
                if(questStages[i].StageNumber == stageNum)
                {
                    Debug.LogWarning("Quest already contains a stage with number " + stageNum);
                    ResetQuestStageFields();
                    return;
                }
            }

            QuestStage stage = new QuestStage(stageName, stageShortDesc, stageProgDesc, stageFinDesc, stageNum, stageType);

            if(stageType == QuestStageType.Kill_Multiple || stageType == QuestStageType.GiveItem_Multiple)
            {
                List<int> _id = new List<int>();
                List<int> _quant = new List<int>();

                Debug.Log(targetIDList.Count);
                for (int j = 0; j < targetIDList.Count; j++)
                {
                    _id.Add(targetIDList[j]);
                    _quant.Add(targetQuantityList[j]);
                }

                selectedQuestStage.AddTargets(_id, _quant);
            }
            else if (stageType == QuestStageType.Kill_Single || stageType == QuestStageType.GiveItem_Single)
            {
                List<int> _id = new List<int>();
                _id.Add(stageTargetID);
                List<int> _quant = new List<int>();
                _quant.Add(1);

                stage.StageTargetID = _id;
                stage.StageTargetQuantity = _quant;
            }
                
            questStages.Add(stage);
            

            // Sorts the list by stage number then resets the fields.
            questStages = questStages.OrderBy(o => o.StageNumber).ToList();
            ResetQuestStageFields();
        }
    }

    void ShowEditQuestStage()
    {
        stageName = EditorGUILayout.TextField("Name: ", stageName, GUILayout.Width(295));
        stageNum = EditorGUILayout.IntField("Number: ", stageNum, GUILayout.Width(295));
        stageShortDesc = EditorGUILayout.TextField("Short Desc: ", stageShortDesc, GUILayout.Width(295));

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Type and Objectives", EditorStyles.boldLabel);
        stageType = (QuestStageType)EditorGUILayout.EnumPopup("Type: ", stageType, GUILayout.Width(480));
        switch (stageType)
        {
            case (QuestStageType.Dialog):
                break;

            case (QuestStageType.CollectItem):
            case (QuestStageType.GiveItem_Single):
            case (QuestStageType.Kill_Single):
                stageTargetID = EditorGUILayout.IntField("Item/NPC: ", stageTargetID, GUILayout.Width(295));
                break;

            case (QuestStageType.GiveItem_Multiple):
            case (QuestStageType.Kill_Multiple):
                for(int i = 0; i < targetIDList.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("X", GUILayout.Width(50)))
                    {
                        targetIDList.RemoveAt(i);
                        targetQuantityList.RemoveAt(i);
                        break;
                    }

                    EditorGUILayout.LabelField(string.Format("Item {0} x{1}", targetIDList[i].ToString(), targetQuantityList[i].ToString()));
                    EditorGUILayout.EndHorizontal();
                }

                stageTargetID = EditorGUILayout.IntField("Item/NPC: ", stageTargetID, GUILayout.Width(295));
                stageTargetQuant = EditorGUILayout.IntField("# req: ", stageTargetQuant, GUILayout.Width(295));

                if (GUILayout.Button("+", GUILayout.Width(50)))
                {
                    if (!targetIDList.Contains(stageTargetID))
                    {
                        targetIDList.Add(stageTargetID);
                        targetQuantityList.Add(stageTargetQuant);
                    }
                        
                    else
                        Debug.LogError("Cannot add target item/npc: already exists in quest stage.");

                    stageTargetID = -1;
                    stageTargetQuant = 0;
                    break;
                }
                break;
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Quest Log Text", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Progress description:");
        stageProgDesc = EditorGUILayout.TextArea(stageProgDesc, GUILayout.Width(350), GUILayout.MinHeight(90));
        EditorGUILayout.LabelField("Finished description:");
        stageFinDesc = EditorGUILayout.TextArea(stageFinDesc, GUILayout.Width(350), GUILayout.MinHeight(90));

        if (GUILayout.Button("Save Changes", GUILayout.Width(300)))
        {
            for (int i = 0; i < questStages.Count; i++)
            {
                if (questStages[i].StageNumber == stageNum && selectedQuestStage.StageNumber != stageNum)
                {
                    Debug.LogWarning("Quest already contains a stage with number " + stageNum);
                    return;
                }
            }

            selectedQuestStage.StageName = stageName;
            selectedQuestStage.StageNumber = stageNum;
            selectedQuestStage.ShortDescription = stageShortDesc;
            selectedQuestStage.ProgressDescription = stageProgDesc;
            selectedQuestStage.FinishedDescription = stageFinDesc;
            selectedQuestStage.StageType = stageType;

            if (stageType == QuestStageType.Kill_Multiple || stageType == QuestStageType.GiveItem_Multiple)
            {
                List<int> _id = new List<int>();
                List<int> _quant = new List<int>();

                for(int j = 0; j < targetIDList.Count; j++)
                {
                    _id.Add(targetIDList[j]);
                    _quant.Add(targetQuantityList[j]);
                }

                selectedQuestStage.AddTargets(_id, _quant);
            }
                
            else if (stageType == QuestStageType.Kill_Single || stageType == QuestStageType.GiveItem_Single)
            {
                List<int> _id = new List<int>();
                _id.Add(stageTargetID);
                List<int> _quant = new List<int>();
                _quant.Add(1);

                selectedQuestStage.StageTargetID = _id;
                selectedQuestStage.StageTargetQuantity = _quant;
            }

            // Sorts the list by stage number then resets the fields.
            questStages = questStages.OrderBy(o => o.StageNumber).ToList();
            ResetQuestStageFields();

            selectedQuestStage = null;
            showEditQuestStage = false;
        }

        if(GUILayout.Button("Cancel", GUILayout.Width(300)))
        {
            ResetQuestStageFields();
            selectedQuestStage = null;
            showEditQuestStage = false;
        }
    }

    void SaveNewQuest()
    {
        Quest newQuest = new Quest();

        newQuest.QuestName = questName;
        newQuest.QuestID = questID;
        newQuest.QuestShortDesc = questShortDesc;
        newQuest.QuestCurrencyReward = questCurrencyReward;

        newQuest.QuestStages = questStages;
        newQuest.QuestRequirements = questRequirements;
        newQuest.QuestItemRewards = questItemRewards;


        // Check that the given ID isn't already in the database.
        if (RequirementsMet(questID))
            questDatabase.AddQuest(newQuest);
        else
            Debug.LogError("A Quest with that ID (" + newQuest.QuestID + ") already exists.");

    }

    void SaveExistingQuest()
    {
        // Check that the given ID isn't already in the database.
        if (!RequirementsMet(questID, selectedQuest))
        {
            Debug.LogError("A Quest with that ID (" + questID + ") already exists.");
            return;
        }

        selectedQuest.QuestName = questName;
        selectedQuest.QuestID = questID;
        selectedQuest.QuestShortDesc = questShortDesc;
        selectedQuest.QuestCurrencyReward = questCurrencyReward;
        selectedQuest.QuestItemRewards = questItemRewards;
        selectedQuest.QuestRequirements = questRequirements;
        selectedQuest.QuestStages = questStages;

    }

    void GetQuestData()
    {
        questName = EditorGUILayout.TextField("Name: ", selectedQuest.QuestName);
        questID = EditorGUILayout.IntField("ID: ", selectedQuest.QuestID);
        questShortDesc = EditorGUILayout.TextField("Description: ", selectedQuest.QuestShortDesc);
        questCurrencyReward = EditorGUILayout.IntField("Gold Reward: ", selectedQuest.QuestCurrencyReward);
        questItemRewards = selectedQuest.QuestItemRewards;
        //CheckItemRewards();
        questRequirements = selectedQuest.QuestRequirements;
        //CheckRequirements();
        questStages = selectedQuest.QuestStages;
    }

    void GetQuestStageData()
    {
        stageName = EditorGUILayout.TextField("Name: ", selectedQuestStage.StageName);
        stageNum = EditorGUILayout.IntField("Number: ", selectedQuestStage.StageNumber);
        stageShortDesc = EditorGUILayout.TextField("Short Desc: ", selectedQuestStage.ShortDescription, GUILayout.Width(295));

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Type and Objectives", EditorStyles.boldLabel);
        stageType = (QuestStageType)EditorGUILayout.EnumPopup("Type: ", selectedQuestStage.StageType, GUILayout.Width(480));
        switch (stageType)
        {
            case (QuestStageType.Dialog):
                break;

            case (QuestStageType.CollectItem):
            case (QuestStageType.GiveItem_Single):
            case (QuestStageType.Kill_Single):
                stageTargetID = EditorGUILayout.IntField("Item/NPC: ", selectedQuestStage.StageTargetID[0], GUILayout.Width(295));
                break;

            case (QuestStageType.GiveItem_Multiple):
            case (QuestStageType.Kill_Multiple):
                
                targetIDList = selectedQuestStage.StageTargetID;
                targetQuantityList = selectedQuestStage.StageTargetQuantity;

                

                for (int i = 0; i < targetIDList.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("X", GUILayout.Width(50)))
                    {
                        targetIDList.RemoveAt(i);
                        targetQuantityList.RemoveAt(i);
                        break;
                    }

                    EditorGUILayout.LabelField(string.Format("Item {0} x{1}", targetIDList[i].ToString(), targetQuantityList[i].ToString()));
                    EditorGUILayout.EndHorizontal();
                }

                stageTargetID = EditorGUILayout.IntField("Item/NPC: ", stageTargetID, GUILayout.Width(295));
                stageTargetQuant = EditorGUILayout.IntField("# req: ", stageTargetQuant, GUILayout.Width(295));

                if (GUILayout.Button("+", GUILayout.Width(50)))
                {
                    if (!targetIDList.Contains(stageTargetID))
                    {
                        targetIDList.Add(stageTargetID);
                        targetQuantityList.Add(stageTargetQuant);
                    }

                    else
                        Debug.LogError("Cannot add target item/npc: already exists in quest stage.");

                    stageTargetID = -1;
                    stageTargetQuant = 0;
                    break;
                }
                break;
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Quest Log Text", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Progress description:");
        stageProgDesc = EditorGUILayout.TextArea(selectedQuestStage.ProgressDescription, GUILayout.Width(350), GUILayout.MinHeight(90));
        EditorGUILayout.LabelField("Finished description:");
        stageFinDesc = EditorGUILayout.TextArea(selectedQuestStage.FinishedDescription, GUILayout.Width(350), GUILayout.MinHeight(90));
    }

    void ResetQuestStageFields()
    {
        stageName = "";
        stageShortDesc = "";
        stageNum = 0;
        stageProgDesc = "";
        stageFinDesc = "";
        stageType = QuestStageType.Dialog;
        stageTargetID = -1;
        stageTargetQuant = 0;

        targetIDList.Clear();
        targetQuantityList.Clear();
    }

    // Check that the unique ID is not taken.
    bool RequirementsMet(int id)
    {
        if (id == -1)
            return true;

        foreach (Quest i in questDatabase.Quests)
        {
            if (i.QuestID == id)
                return false;
        }

        return true;
    }



    // Check whether the unique ID is taken BY ANOTHER ITEM...
    bool RequirementsMet(int id, Quest self)
    {
        if (id == -1 || self == null)
            return true;

        foreach (Quest i in questDatabase.Quests)
        {
            if (i != self && i.QuestID == id)
                return false;
        }

        return true;
    }
}
