using UnityEngine;
using UnityEditor;
using LitJson;
using GameUtilities;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class NPCEditor : EditorWindow
{
    private NPCDatabase npcDatabase;
    //private List<NPC> conditions = new List<NPC>();
    private string NPCAssetPath = "Assets/npcdb.asset";

    #region Auxillary Databases
    private ItemDatabase _itemDatabase;
    private string itemPath = "Assets/itemdb.asset";
    private QuestDatabase _questDatabase;
    private string questPath = "Assets/questdb.asset";
    #endregion

    private EditorState editorState;
    private NPC selectedNPC;
    private NPCDialog selectedDialog;
    enum EditorState { Home, Create, Edit }
    Vector2 listScrollPos;
    Vector2 editScrollPos;
    Vector2 npcDialogScrollPos;
    Vector2 addedDialogScrollPos;
    Vector2 npcInventoryScrollPos;
    bool showNPCDialog;
    bool showNPCInventory;
    bool showAddDialog;
    bool showEditDialog;

    #region NPC Properties
    string npcName;
    int npcID;
    List<int> npcHostileTo = new List<int>();
    List<int> npcInventoryIDs = new List<int>();
    List<int> npcInventoryStacks = new List<int>();
    List<NPCDialog> npcDialog = new List<NPCDialog>();

    int hostileFactionID;
    int itemID;
    int itemQuantity;
    #endregion

    #region NPC Dialog Properties
    int dialogID;
    string dialogOption;
    string dialogOutput;
    int dialogNextID;
    List<int> dialogNext = new List<int>();
    //Requirements...
    NPCDialogType dialogType = NPCDialogType.Dialog;
    int dialogTriggerID;
    bool dialogTriggerOnFinish; 
    #endregion  



    // Add menu named "NPC Editor" to the Window menu
    [MenuItem("Window/NPC Editor")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        NPCEditor editor = (NPCEditor)EditorWindow.GetWindow(typeof(NPCEditor));
        editor.minSize = new Vector2(1500, 600);
        editor.Show();
    }

    void Awake()
    {
        LoadNPCDatabase();
        LoadAuxillaryDatabases();
    }

    void LoadNPCDatabase()
    {
        npcDatabase = AssetDatabase.LoadAssetAtPath<NPCDatabase>(NPCAssetPath);

        if (npcDatabase == null)
            CreateNPCDatabase();
        else
        {
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = npcDatabase;
        }
    }

    void LoadAuxillaryDatabases()
    {
        // ITEM database
        _itemDatabase = AssetDatabase.LoadAssetAtPath<ItemDatabase>(itemPath);
        _itemDatabase.ReloadDatabase();

        // ITEM database
        _questDatabase = AssetDatabase.LoadAssetAtPath<QuestDatabase>(questPath);
        _questDatabase.ReloadDatabase();
    }

    void CreateNPCDatabase()
    {
        Debug.Log("Creating NPC database...");

        npcDatabase = ScriptableObject.CreateInstance<NPCDatabase>();
        Debug.Log(npcDatabase);

        AssetDatabase.CreateAsset(npcDatabase, "Assets/npcdb.asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = npcDatabase;
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
        if (GUILayout.Button("Create New NPC", GUILayout.Width(300)))
        {
            editorState = EditorState.Create;
            ResetDialogFields();
            ResetInventoryFields();
            npcDialog = new List<NPCDialog>();
            return;
        }
        if (GUILayout.Button("Reload Database", GUILayout.Width(300)))
        {
            npcDatabase.ReloadDatabase();
            return;
        }
        if (GUILayout.Button("Save to JSON", GUILayout.Width(300)))
        {
            // Delete this item from the database.
            npcDatabase.SaveDatabase();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        EditorGUILayout.EndHorizontal();

        if (npcDatabase == null || npcDatabase.NPCs == null)
        {
            EditorGUILayout.LabelField("The database may need reloading.");
            return;
        }

        EditorGUILayout.BeginHorizontal();
        // List all of the items on the left hand side.
        listScrollPos = EditorGUILayout.BeginScrollView(listScrollPos, false, false, GUILayout.Width(450), GUILayout.MinHeight(550));
        foreach (NPC i in npcDatabase.NPCs)
        {
            // Horizontal group per condition.
            EditorGUILayout.BeginHorizontal(GUILayout.Width(400.0f));

            if (GUILayout.Button("X", GUILayout.Width(50.0f)))
            {
                // Delete this item from the database.
                npcDatabase.RemoveNPC(i);
                EditorUtility.SetDirty(npcDatabase);
                AssetDatabase.SaveAssets();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = npcDatabase;
                return;
            }

            if (GUILayout.Button("C", GUILayout.Width(50.0f)))
            {
                // Duplicate this item.
                npcDatabase.DuplicateNPC(i);
                EditorUtility.SetDirty(npcDatabase);
                AssetDatabase.SaveAssets();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = npcDatabase;
                return;
            }

            if (GUILayout.Button(i.NPCName.ToString(), GUILayout.Width(300)))
            {
                if (editorState == EditorState.Edit)
                    SaveExistingNPC();
                else if (editorState == EditorState.Create)
                    SaveNewNPC();

                //Get the new item and its associated data.
                selectedNPC = i;
                GetNPCData();

                EditorUtility.FocusProjectWindow();
                Selection.activeObject = npcDatabase;

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

        npcName = EditorGUILayout.TextField("Name: ", npcName, GUILayout.Width(300));
        npcID = EditorGUILayout.IntField("ID: ", npcID, GUILayout.Width(300));

        EditorGUILayout.Space();

        /*
         *  INVENTORY
         */

        showNPCInventory = EditorGUILayout.Foldout(showNPCInventory, "INVENTORY");
        if (showNPCInventory)
            DisplayNPCInventory();

        EditorGUILayout.Space();

        /*
         *  SAVED /CANCEL Buttons
         */
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save", GUILayout.Width(150.0f)))
        {
            // Save this item to the database, either as a new item
            // or as an existing item.
            if (editorState == EditorState.Create)
                SaveNewNPC();
            else
                SaveExistingNPC();

            EditorUtility.SetDirty(npcDatabase);
            editorState = EditorState.Home;
        }
        if (GUILayout.Button("Cancel", GUILayout.Width(150.0f)))
        {
            EditorUtility.SetDirty(npcDatabase);
            editorState = EditorState.Home;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndScrollView();

        /*
         *  QUEST STAGES
         */
        EditorGUILayout.BeginVertical();
        showNPCDialog = EditorGUILayout.Foldout(showNPCDialog, "NPC DIALOG");
        if (showNPCDialog)
        {
            npcDialogScrollPos = EditorGUILayout.BeginScrollView(npcDialogScrollPos, false, false, GUILayout.MinWidth(540), GUILayout.MinHeight(550));
            DisplayNPCDialog();
            EditorGUILayout.EndScrollView();
        }

        EditorGUILayout.EndVertical();

        
    }

    void DisplayNPCDialog()
    {
        EditorGUILayout.LabelField("Current Dialog", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical();

        // Show the currently-added dialog options...
        addedDialogScrollPos = EditorGUILayout.BeginScrollView(addedDialogScrollPos, false, true, GUILayout.Width(400), GUILayout.Height(150));
        for(int i = 0; i < npcDialog.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("X", GUILayout.Width(50)))
            {
                npcDialog.RemoveAt(i);
                break;
            }
            if (GUILayout.Button("E", GUILayout.Width(50)))
            {
                showEditDialog = true;
                showAddDialog = false;
                //ResetQuestStageFields();
                selectedDialog = npcDialog[i];
                GetDialogData();
                break;
            }

            string links = " ";
            for(int j = 0; j < npcDialog[i].DialogNext.Count; j++)
            {
                links += npcDialog[i].DialogNext[j].ToString() + " ";
            }
            EditorGUILayout.LabelField(string.Format("#{0}: ->{1} \n'{2}' \n Links: {3}", npcDialog[i].DialogID, npcDialog[i].DialogOption,
                                                                                            npcDialog[i].DialogOutput, links), 
                                                                                            GUILayout.MinHeight(40));
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        // Button for adding a new stage:
        if (!showAddDialog)
            {
                if (GUILayout.Button("New Quest Stage", GUILayout.Width(300)))
                {
                    //ResetQuestStageFields();
                    showAddDialog = true;
                    showEditDialog = false;
                }
            }
            else
            {
                ShowModifyDialog();
            }

        if (showEditDialog)
            ShowModifyDialog();

        EditorGUILayout.EndVertical();
    }

    public void ShowModifyDialog()
    {
        dialogID = EditorGUILayout.IntField("ID: ", dialogID, GUILayout.Width(295));
        dialogOption = EditorGUILayout.TextField("Option: ", dialogOption, GUILayout.Width(295));
        EditorGUILayout.LabelField("Dialog text:");
        dialogOutput = EditorGUILayout.TextArea(dialogOutput, GUILayout.Width(350), GUILayout.MinHeight(90));

        EditorGUILayout.Space();
        // What the dialog links to...
        EditorGUILayout.LabelField("Dialog links to...");
        for(int i = 0; i < dialogNext.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("X", GUILayout.Width(50)))
            {
                dialogNext.RemoveAt(i);
                break;
            }

            string label = string.Format("#{0}: ", dialogNext[i].ToString());
            if(selectedNPC != null)
            {
                if (selectedNPC.HasDialogWithID(dialogNext[i]))
                    label += selectedNPC.GetDialogWithID(dialogNext[i]).DialogOption;
            }
            EditorGUILayout.LabelField(label, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.BeginHorizontal();
        dialogNextID = EditorGUILayout.IntField("New Link: ", dialogNextID, GUILayout.Width(295));
        if (GUILayout.Button("+", GUILayout.Width(50)))
        {
            dialogNext.Add(dialogNextID);
            dialogNextID = 0;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        // Dialog type, target item/quest...
        dialogType = (NPCDialogType)EditorGUILayout.EnumPopup("Type: ", dialogType, GUILayout.Width(480));
        switch(dialogType)
        {
            case (NPCDialogType.Dialog):
                break;

            case (NPCDialogType.Player_GiveItem):
            case (NPCDialogType.Player_ReceiveItem):
                EditorGUILayout.BeginHorizontal();
                string itemLabel = string.Format("#{0}: ", dialogTriggerID.ToString());
                if (_itemDatabase.Contains(dialogTriggerID))
                    itemLabel += _itemDatabase.Item(dialogTriggerID).ItemName;

                EditorGUILayout.LabelField(itemLabel);
                EditorGUILayout.EndHorizontal();
                break;

            case (NPCDialogType.Quest_Start):
            case (NPCDialogType.Quest_Advance):
            case (NPCDialogType.Quest_Finish):
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("X", GUILayout.Width(50)))
                {
                    dialogTriggerID = -1;
                    break;
                }

                string questLabel = string.Format("#{0}: ", dialogTriggerID.ToString());
                if (_questDatabase.Contains(dialogTriggerID))
                    questLabel += _questDatabase.quest(dialogTriggerID);

                EditorGUILayout.LabelField(questLabel);
                EditorGUILayout.EndHorizontal();
                break;
        }

        dialogTriggerOnFinish = EditorGUILayout.Toggle("Trigger on Finish: ", dialogTriggerOnFinish);
        dialogTriggerID = EditorGUILayout.IntField("Item/NPC ID: ", dialogTriggerID, GUILayout.Width(295));

        if(showEditDialog)
        {
            if (GUILayout.Button("Save Dialog", GUILayout.Width(300)))
            {
                if(selectedNPC.HasDialogWithID(dialogID) && selectedDialog.DialogID != dialogID)
                {
                    Debug.LogWarning("NPC already contains a dialog with ID  " + dialogID);
                    return;
                }

                selectedDialog.DialogID = dialogID;
                selectedDialog.DialogOption = dialogOption;
                selectedDialog.DialogOutput = dialogOutput;
                selectedDialog.DialogType = dialogType;
                selectedDialog.DialogTriggerOnFinish = dialogTriggerOnFinish;
                selectedDialog.DialogTriggerID = dialogTriggerID;

                List<int> _next = new List<int>();
                for (int i = 0; i < dialogNext.Count; i++)
                {
                    _next.Add(dialogNext[i]);
                }

                selectedDialog.SetDialogLinks(_next);

                // Sorts the list by dialog number then resets the fields.
                // npcDialog = npcDialog.OrderBy(o => o.DialogID).ToList();
                ResetDialogFields();

                selectedDialog = null;
                showEditDialog = false;
            }

            if (GUILayout.Button("Cancel", GUILayout.Width(300)))
            {
                ResetDialogFields();
                selectedDialog = null;
                showEditDialog = false;
            }
        }
        else if (showAddDialog)
        {
            if (GUILayout.Button("Save Dialog", GUILayout.Width(300)))
            {
                if(selectedNPC != null)
                {
                    if (selectedNPC.HasDialogWithID(dialogID))
                    {
                        Debug.LogWarning("NPC already contains a dialog with ID " + dialogID);
                        return;
                    }
                }
                

                NPCDialog _dialog = new NPCDialog();

                _dialog.DialogID = dialogID;
                _dialog.DialogOption = dialogOption;
                _dialog.DialogOutput = dialogOutput;
                _dialog.DialogType = dialogType;
                _dialog.DialogTriggerOnFinish = dialogTriggerOnFinish;
                _dialog.DialogTriggerID = dialogTriggerID;

                List<int> _next = new List<int>();
                for (int i = 0; i < dialogNext.Count; i++)
                {
                    _next.Add(dialogNext[i]);
                }

                _dialog.SetDialogLinks(_next);

                npcDialog.Add(_dialog);

                // Sorts the list by dialog number then resets the fields.
                //npcDialog = npcDialog.OrderBy(o => o.DialogID).ToList();
                ResetDialogFields();

                selectedDialog = null;
                showEditDialog = false;
            }
        }

        EditorGUILayout.Space();
    }

    void DisplayNPCInventory()
    {
        EditorGUILayout.LabelField("Current Inventory", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical();

        // Display the currently-added quest stages
        npcInventoryScrollPos = EditorGUILayout.BeginScrollView(npcInventoryScrollPos, false, false, GUILayout.Width(400), GUILayout.Height(150));
        for (int i = 0; i < npcInventoryIDs.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("X", GUILayout.Width(50)))
            {
                
                npcInventoryIDs.RemoveAt(i);
                npcInventoryStacks.RemoveAt(i);
                Debug.Log("Removed." + npcInventoryIDs.Count);


                EditorUtility.SetDirty(npcDatabase);
                AssetDatabase.SaveAssets();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = npcDatabase;
                return;
            }

            EditorGUILayout.LabelField(string.Format("{0} x{1}", _itemDatabase.Item(npcInventoryIDs[i]).ItemName, npcInventoryStacks[i]));
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        // Button for adding a new item:
        itemID = EditorGUILayout.IntField("ID: ", itemID, GUILayout.Width(200));
        itemQuantity = EditorGUILayout.IntField(" x", itemQuantity, GUILayout.Width(200));

        // Display the button to add the item, plus a label that shows the item name.
        EditorGUILayout.BeginHorizontal();
        if (itemID != -1 && _itemDatabase.Contains(itemID) && itemQuantity > 0)
        {
            EditorGUILayout.LabelField(string.Format("[{0}] {1}", itemID, _itemDatabase.Item(itemID).ItemName), GUILayout.Width(100));

            if (GUILayout.Button("Add Item", GUILayout.Width(100)))
            {
                if (!npcInventoryIDs.Contains(itemID))
                {
                    npcInventoryIDs.Add(itemID);
                    npcInventoryStacks.Add(itemQuantity);
                    itemID = -1;
                    itemQuantity = 0;
                }
                else
                {
                    int currentStack = npcInventoryStacks[npcInventoryIDs.IndexOf(itemID)];
                    npcInventoryStacks[npcInventoryIDs.IndexOf(itemID)] += itemQuantity;

                    itemID = -1;
                    itemQuantity = 0;
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    void SaveNewNPC()
    {
        NPC newNPC = new NPC();

        newNPC.NPCName = npcName;
        newNPC.NPCID = npcID;
        newNPC.NPCHostileTo = npcHostileTo;
        newNPC.NPCDialogText = npcDialog;
        newNPC.NPCInventoryIDs = npcInventoryIDs;
        newNPC.NPCInventoryStacks = npcInventoryStacks;

        // Check that the given ID isn't already in the database.
        if (RequirementsMet(npcID))
            npcDatabase.AddNPC(newNPC);
        else
            Debug.LogError("A NPC with that ID (" + newNPC.NPCID + ") already exists.");
    }

    void SaveExistingNPC()
    {
        // Check that the given ID isn't already in the database.
        if (!RequirementsMet(npcID, selectedNPC))
        {
            Debug.LogError("A NPC with that ID (" + npcID + ") already exists.");
            return;
        }

        selectedNPC.NPCName = npcName;
        selectedNPC.NPCID = npcID;
        selectedNPC.NPCInventoryIDs = npcInventoryIDs;
        selectedNPC.NPCInventoryStacks = npcInventoryStacks;
        selectedNPC.NPCHostileTo = npcHostileTo;
        selectedNPC.NPCDialogText = npcDialog;
    }

    void GetNPCData()
    {
        npcName = EditorGUILayout.TextField("Name: ", selectedNPC.NPCName);
        npcID = EditorGUILayout.IntField("ID: ", selectedNPC.NPCID);
        npcHostileTo = selectedNPC.NPCHostileTo;
        npcDialog = selectedNPC.NPCDialogText;
        npcInventoryIDs = selectedNPC.NPCInventoryIDs;
        npcInventoryStacks = selectedNPC.NPCInventoryStacks;
    }

    void GetDialogData()
    {
        dialogID = EditorGUILayout.IntField("ID: ", selectedDialog.DialogID, GUILayout.Width(295));
        dialogOption = EditorGUILayout.TextField("Option: ", selectedDialog.DialogOption, GUILayout.Width(295));
        EditorGUILayout.LabelField("Dialog text:");
        dialogOutput = EditorGUILayout.TextArea(selectedDialog.DialogOutput, GUILayout.Width(350), GUILayout.MinHeight(90));

        EditorGUILayout.Space();
        // What the dialog links to...
        EditorGUILayout.LabelField("Dialog links to...");
        
        for(int h = 0; h < selectedDialog.DialogNext.Count; h++)
        {
            dialogNext.Add(selectedDialog.DialogNext[h]);
        }


        for (int i = 0; i < dialogNext.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("X", GUILayout.Width(50)))
            {
                dialogNext.RemoveAt(i);
                break;
            }

            string label = string.Format("#{0}: ", dialogNext[i].ToString());
            if (selectedNPC != null)
            {
                if (selectedNPC.HasDialogWithID(dialogNext[i]))
                    label += selectedNPC.GetDialogWithID(dialogNext[i]).DialogOption;
            }
            EditorGUILayout.LabelField(label, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.BeginHorizontal();
        dialogNextID = EditorGUILayout.IntField("New Link: ", dialogNextID, GUILayout.Width(295));
        if (GUILayout.Button("+", GUILayout.Width(50)))
        {
            dialogNext.Add(dialogNextID);
            dialogNextID = 0;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        // Dialog type, target item/quest...
        dialogType = (NPCDialogType)EditorGUILayout.EnumPopup("Type: ", selectedDialog.DialogType, GUILayout.Width(480));
        switch (dialogType)
        {
            case (NPCDialogType.Dialog):
                break;

            case (NPCDialogType.Player_GiveItem):
            case (NPCDialogType.Player_ReceiveItem):
                EditorGUILayout.BeginHorizontal();
                string itemLabel = string.Format("#{0}: ", dialogTriggerID.ToString());
                if (_itemDatabase.Contains(dialogTriggerID))
                    itemLabel += _itemDatabase.Item(dialogTriggerID).ItemName;

                EditorGUILayout.LabelField(itemLabel);
                EditorGUILayout.EndHorizontal();
                break;

            case (NPCDialogType.Quest_Start):
            case (NPCDialogType.Quest_Advance):
            case (NPCDialogType.Quest_Finish):
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("X", GUILayout.Width(50)))
                {
                    dialogTriggerID = -1;
                    break;
                }

                string questLabel = string.Format("#{0}: ", dialogTriggerID.ToString());
                if (_questDatabase.Contains(dialogTriggerID))
                    questLabel += _questDatabase.quest(dialogTriggerID);

                EditorGUILayout.LabelField(questLabel);
                EditorGUILayout.EndHorizontal();
                break;
        }

        dialogTriggerOnFinish = EditorGUILayout.Toggle("Trigger on Finish: ", selectedDialog.DialogTriggerOnFinish);
        dialogTriggerID = EditorGUILayout.IntField("Item/NPC ID: ", selectedDialog.DialogTriggerID, GUILayout.Width(295));

        if (showEditDialog)
        {
            if (GUILayout.Button("Save Dialog", GUILayout.Width(300)))
            {
                if (selectedNPC.HasDialogWithID(dialogID))
                {
                    Debug.LogWarning("NPC already contains a dialog with ID  " + dialogID);
                    return;
                }

                selectedDialog.DialogID = dialogID;
                selectedDialog.DialogOption = dialogOption;
                selectedDialog.DialogOutput = dialogOutput;
                selectedDialog.DialogType = dialogType;
                selectedDialog.DialogTriggerOnFinish = dialogTriggerOnFinish;
                selectedDialog.DialogTriggerID = dialogTriggerID;

                List<int> _next = new List<int>();
                for (int i = 0; i < dialogNext.Count; i++)
                {
                    _next.Add(dialogNext[i]);
                }

                selectedDialog.SetDialogLinks(_next);

                // Sorts the list by dialog number then resets the fields.
                // npcDialog = npcDialog.OrderBy(o => o.DialogID).ToList();
                ResetDialogFields();

                selectedDialog = null;
                showEditDialog = false;
            }

            if (GUILayout.Button("Cancel", GUILayout.Width(300)))
            {
                ResetDialogFields();
                selectedDialog = null;
                showEditDialog = false;
            }
        }
        else if (showAddDialog)
        {
            if (GUILayout.Button("Save Dialog", GUILayout.Width(300)))
            {
                if (selectedNPC.HasDialogWithID(dialogID))
                {
                    Debug.LogWarning("NPC already contains a dialog with ID " + dialogID);
                    return;
                }

                NPCDialog _dialog = new NPCDialog();

                _dialog.DialogID = dialogID;
                _dialog.DialogOption = dialogOption;
                _dialog.DialogOutput = dialogOutput;
                _dialog.DialogType = dialogType;
                _dialog.DialogTriggerOnFinish = dialogTriggerOnFinish;
                _dialog.DialogTriggerID = dialogTriggerID;
                _dialog.SetDialogLinks(dialogNext);

                List<int> _next = new List<int>();
                for (int i = 0; i < dialogNext.Count; i++)
                {
                    _next.Add(dialogNext[i]);
                }

                _dialog.SetDialogLinks(_next);

                // Sorts the list by dialog number then resets the fields.
                // npcDialog = npcDialog.OrderBy(o => o.DialogID).ToList();
                ResetDialogFields();

                selectedDialog = null;
                showEditDialog = false;
            }
        }
    }

    void ResetDialogFields()
    {
        dialogID = 0;
        dialogNext.Clear();
        dialogNextID = -1;
        dialogOption = " ";
        dialogOutput = " ";
        dialogTriggerID = -1;
        dialogTriggerOnFinish = false;
        dialogType = NPCDialogType.Dialog;
    }

    void ResetInventoryFields() {
        npcInventoryStacks = new List<int>();
        npcInventoryIDs = new List<int>();
    }

    // Check that the unique ID is not taken.
    bool RequirementsMet(int id)
    {
        if (id == -1)
            return true;

        foreach (NPC i in npcDatabase.NPCs)
        {
            if (i.NPCID == id)
                return false;
        }

        return true;
    }



    // Check whether the unique ID is taken BY ANOTHER ITEM...
    bool RequirementsMet(int id, NPC self)
    {
        if (id == -1 || self == null)
            return true;

        foreach (NPC i in npcDatabase.NPCs)
        {
            if (i != self && i.NPCID == id)
                return false;
        }

        return true;
    }
}
