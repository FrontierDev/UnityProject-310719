using UnityEngine;
using UnityEditor;
using LitJson;
using GameUtilities;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class PerkEditor : EditorWindow {
    private PerkDatabase perkDatabase;
    //private List<Aura> conditions = new List<Aura>();
    private string perkAssetPath = "Assets/perkdb.asset";

    #region Auxillary Databases
    private AuraDatabase _conditionDatabase;
    private string conditionPath = "Assets/auradb.asset";
    #endregion

    private EditorState editorState;
    private Perk selectedPerk;
    enum EditorState { Home, Create, Edit }
    Vector2 listScrollPos;
    Vector2 editScrollPos;
    Vector2 addedAuraScrollPos;
    Vector2 conditionScrollPos;

    #region Perk Properties
    string perkName;
    int perkID;
    string perkDesc;
    string perkIconPath;
    Texture2D perkIcon;
    List<int> perkAuras = new List<int>();
    int conditionID;
    #endregion


    // Add menu named "Perk Editor" to the Window menu
    [MenuItem("Window/Perk Editor")]
    static void Init() {
        // Get existing open window or if none, make a new one:
        PerkEditor editor = (PerkEditor)EditorWindow.GetWindow(typeof(PerkEditor));
        editor.minSize = new Vector2(1000, 600);
        editor.Show();
    }

    void Awake() {
        
    }

    void LoadPerkDatabase() {
        perkDatabase = AssetDatabase.LoadAssetAtPath<PerkDatabase>(perkAssetPath);
        perkDatabase.ReloadDatabase();

        if (perkDatabase == null)
            CreatePerkDatabase();
        else
        {
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = perkDatabase;
        }
    }

    void LoadAuxillaryDatabases() {
        // CONDITION database
        _conditionDatabase = AssetDatabase.LoadAssetAtPath<AuraDatabase>(conditionPath);
        _conditionDatabase.ReloadDatabase();
    }

    void CreatePerkDatabase() {
        Debug.Log("Creating perk database...");

        perkDatabase = ScriptableObject.CreateInstance<PerkDatabase>();

        AssetDatabase.CreateAsset(perkDatabase, perkAssetPath);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = perkDatabase;
    }

    void OnEnable() {
        LoadPerkDatabase();
        LoadAuxillaryDatabases();
    }

    void OnGUI() {
        /*
         * Editor Toolbar
         */ 
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create New Perk", GUILayout.Width(300)))
        {
            editorState = EditorState.Create;
            return;
        }
        if (GUILayout.Button("Reload Database", GUILayout.Width(300)))
        {
            perkDatabase.ReloadDatabase();
            return;
        }
        if (GUILayout.Button("Save to JSON", GUILayout.Width(300)))
        {
            // Delete this perk from the database.
            perkDatabase.SaveDatabase();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        EditorGUILayout.EndHorizontal();

        if (perkDatabase == null || perkDatabase.Perks == null)
        {
            EditorGUILayout.LabelField("The database may need reloading.");
            return;
        }

        EditorGUILayout.BeginHorizontal();
        // List all of the items on the left hand side.
        listScrollPos = EditorGUILayout.BeginScrollView(listScrollPos, false, false, GUILayout.Width(450), GUILayout.MinHeight(550));

        foreach (Perk i in perkDatabase.Perks)
        {
            // Horizontal group per perk.
            EditorGUILayout.BeginHorizontal(GUILayout.Width(400.0f));

            if (GUILayout.Button("X", GUILayout.Width(50.0f)))
            {
                // Delete this item from the database.
                perkDatabase.RemovePerk(i);
                EditorUtility.SetDirty(perkDatabase);
                AssetDatabase.SaveAssets();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = perkDatabase;
                return;
            }

            if (GUILayout.Button("C", GUILayout.Width(50.0f)))
            {
                // Duplicate this item.
                perkDatabase.DuplicatePerk(i);
                EditorUtility.SetDirty(perkDatabase);
                AssetDatabase.SaveAssets();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = perkDatabase;
                return;
            }

            if (GUILayout.Button(i.PerkName.ToString(), GUILayout.Width(300)))
            {
                if (editorState == EditorState.Edit)
                    SaveExistingPerk();
                else if (editorState == EditorState.Create)
                    SaveNewPerk();

                //Get the new item and its associated data.
                selectedPerk = i;
                GetPerkData();
                AssetDatabase.SaveAssets();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = perkDatabase;

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

    void ShowCreateWindow() {
        editScrollPos = EditorGUILayout.BeginScrollView(editScrollPos, false, false, GUILayout.MinWidth(540), GUILayout.MinHeight(550));

        perkName = EditorGUILayout.TextField("Name: ", perkName, GUILayout.Width(300));
        perkID = EditorGUILayout.IntField("ID: ", perkID, GUILayout.Width(300));
        perkDesc = EditorGUILayout.TextField("Description: ", perkDesc, GUILayout.Width(450));

        perkIcon = EditorGUILayout.ObjectField("Icon: ", perkIcon, typeof(Texture2D), true, GUILayout.Width(450)) as Texture2D;

        // Perk conditions editor
        DisplayPerkAuras();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save", GUILayout.Width(150.0f)))
        {
            // Save this perk to the database, either as a new perk
            // or as an existing perk.
            if (editorState == EditorState.Create)
                SaveNewPerk();
            else
                SaveExistingPerk();

            EditorUtility.SetDirty(perkDatabase);
            AssetDatabase.SaveAssets();
            editorState = EditorState.Home;
        }
        if (GUILayout.Button("Cancel", GUILayout.Width(150.0f)))
        {
            EditorUtility.SetDirty(perkDatabase);
            AssetDatabase.SaveAssets();
            editorState = EditorState.Home;
        }
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.EndScrollView();
    }

    void SaveNewPerk() {
        Perk newPerk = new Perk();

        /*
         * Check to ensure that the condition has a unique id.
         */

        newPerk.PerkName = perkName;
        newPerk.PerkID = perkID;
        newPerk.PerkDesc = perkDesc;
        newPerk.PerkAuras = perkAuras;

        // Set icon
        newPerk.SetIcon(perkIcon);

        // Find icon path
        perkIconPath = AssetDatabase.GetAssetPath(perkIcon);
        perkIconPath = GameUtility.CleanItemResourcePath(perkIconPath, "Assets/Resources/");
        perkIconPath = GameUtility.CleanItemResourcePath(perkIconPath, ".png");
        newPerk.PerkIconpath = perkIconPath;

        // Check that the given ID isn't already in the database.
        if (RequirementsMet(perkID))
            perkDatabase.AddPerk(newPerk);
        else
            Debug.LogError("An item with that ID (" + newPerk.PerkID + ") already exists.");

    }

    void SaveExistingPerk() {
        // Check that the given ID isn't already in the database.
        if (!RequirementsMet(perkID, selectedPerk))
        {
            Debug.LogError("An item with that ID (" + perkID + ") already exists.");
            return;
        }

        selectedPerk.PerkName = perkName;
        selectedPerk.PerkID = perkID;
        selectedPerk.PerkDesc = perkDesc;
        selectedPerk.PerkAuras = perkAuras;

        // Set icon
        selectedPerk.SetIcon(perkIcon);

        // Find icon path
        perkIconPath = AssetDatabase.GetAssetPath(perkIcon);
        perkIconPath = GameUtility.CleanItemResourcePath(perkIconPath, "Assets/Resources/");
        perkIconPath = GameUtility.CleanItemResourcePath(perkIconPath, ".png");
        selectedPerk.PerkIconpath = perkIconPath;

    }

    void GetPerkData() {
        perkName = EditorGUILayout.TextField("Name: ", selectedPerk.PerkName);
        perkID = EditorGUILayout.IntField("ID: ", selectedPerk.PerkID);
        perkDesc = EditorGUILayout.TextField("Description: ", selectedPerk.PerkDesc);
        perkAuras = selectedPerk.PerkAuras;
        perkIcon = EditorGUILayout.ObjectField("Icon: ", selectedPerk.GetIcon(), typeof(Texture2D), true) as Texture2D;
    }

    void DisplayPerkAuras() {
        EditorGUILayout.LabelField("Assigned Auras", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        // Display the currently-added conditions
        addedAuraScrollPos = EditorGUILayout.BeginScrollView(addedAuraScrollPos, false, false, GUILayout.MinWidth(270), GUILayout.Height(180));
        for (int i = 0; i < perkAuras.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("X", GUILayout.Width(50)))
            {
                perkAuras.RemoveAt(i);
                break;
            }
            EditorGUILayout.LabelField(_conditionDatabase.Aura(perkAuras[i]).AuraName + " (ID " + _conditionDatabase.Aura(perkAuras[i]).AuraID + ")");
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
        // Search for perks in the perk database
        conditionScrollPos = EditorGUILayout.BeginScrollView(conditionScrollPos, false, false, GUILayout.MinWidth(270), GUILayout.Height(180));
        for (int j = 0; j < _conditionDatabase.Auras.Count; j++)
        {
            int _id = _conditionDatabase.Auras[j].AuraID;
            string _name = _conditionDatabase.Auras[j].AuraName;

            if (perkAuras.Contains(_id))
                continue;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+", GUILayout.Width(50)))
            {
                conditionID = _id;

                if (AuraRequirementsMet())
                {
                    perkAuras.Add(conditionID);
                    // now sort the list...
                }

            }
            EditorGUILayout.LabelField(_name + " (ID " + _id + ")");
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // Display a box which allows the user to add new conditions by their IDs
        conditionID = EditorGUILayout.IntField("Aura ID: ", conditionID);
        if (GUILayout.Button("Add", GUILayout.Width(350.0f)))
        {
            if (AuraRequirementsMet())
                perkAuras.Add(conditionID);
        }
    }

    bool AuraRequirementsMet() {
        if (!_conditionDatabase.Contains(conditionID))
        {
            Debug.LogError("Aura with ID " + conditionID + " does not exist in the database.");
            return false;
        }

        if (perkAuras.Contains(conditionID))
        {
            Debug.LogError("This condition has already been added.");
            return false;
        }

        return true;
    }

    // Check that the unique ID is not taken.
    bool RequirementsMet(int id) {
        if (id == -1)
            return true;

        foreach (Perk i in perkDatabase.Perks)
        {
            if (i.PerkID == id)
                return false;
        }

        return true;
    }

    // Check whether the unique ID is taken BY ANOTHER PERK...
    bool RequirementsMet(int id, Perk self) {
        if (id == -1 || self == null)
            return true;

        foreach (Perk i in perkDatabase.Perks)
        {
            if (i != self && i.PerkID == id)
                return false;
        }

        return true;
    }
}
