using UnityEngine;
using UnityEditor;
using LitJson;
using GameUtilities;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class AuraEditor : EditorWindow {
    private AuraDatabase auraDatabase;
    //private List<Aura> conditions = new List<Aura>();
    private string auraAssetPath = "Assets/auradb.asset";

    private EditorState editorState;
    private Aura selectedAura;
    enum EditorState { Home, Create, Edit }
    Vector2 listScrollPos;
    Vector2 editScrollPos;

    #region Aura Properties
    string conditionName;
    int conditionID;
    string conditionDesc;
    bool isHarmful;
    AuraStat conditionStat;
    int conditionValue;
    bool hasDuration;
    double conditionDuration;
    string auraSkill;
    #endregion


    // Add menu named "Aura Editor" to the Window menu
    [MenuItem("Window/Aura Editor")]
    static void Init() {
        // Get existing open window or if none, make a new one:
		AuraEditor editor = (AuraEditor)EditorWindow.GetWindow(typeof(AuraEditor));
        editor.minSize = new Vector2(1000, 600);
        editor.Show();
    }

    void Awake() {
        LoadAuraDatabase();
    }

    void LoadAuraDatabase() {
		auraDatabase = AssetDatabase.LoadAssetAtPath<AuraDatabase>(auraAssetPath);

        if (auraDatabase == null)
            CreateAuraDatabase();
        else
        {
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = auraDatabase;
        }
    }

    void CreateAuraDatabase() {
        Debug.Log("Creating aura database...");

        auraDatabase = ScriptableObject.CreateInstance<AuraDatabase>();
        Debug.Log(auraDatabase);

        AssetDatabase.CreateAsset(auraDatabase, "Assets/auradb.asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = auraDatabase;
    }

    void OnEnable() {

    }

    void OnGUI() {
        /*
         * Editor toolbar
         */
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create New Aura", GUILayout.Width(300)))
        {
            editorState = EditorState.Create;
            return;
        }
        if (GUILayout.Button("Reload Database", GUILayout.Width(300)))
        {
            auraDatabase.ReloadDatabase();
            return;
        }
        if (GUILayout.Button("Save to JSON", GUILayout.Width(300)))
        {
            // Delete this item from the database.
            auraDatabase.SaveDatabase();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        EditorGUILayout.EndHorizontal();

        if (auraDatabase == null || auraDatabase.Auras == null)
        {
            EditorGUILayout.LabelField("The database may need reloading.");
            return;
        }

        EditorGUILayout.BeginHorizontal();
        // List all of the items on the left hand side.
        listScrollPos = EditorGUILayout.BeginScrollView(listScrollPos, false, false, GUILayout.Width(450), GUILayout.MinHeight(550));
		foreach (Aura i in auraDatabase.Auras)
        {
            // Horizontal group per condition.
            EditorGUILayout.BeginHorizontal(GUILayout.Width(400.0f));

            if (GUILayout.Button("X", GUILayout.Width(50.0f)))
            {
                // Delete this item from the database.
                auraDatabase.RemoveAura(i);
                EditorUtility.SetDirty(auraDatabase);
                AssetDatabase.SaveAssets();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = auraDatabase;
                return;
            }

            if (GUILayout.Button("C", GUILayout.Width(50.0f)))
            {
                // Duplicate this item.
                auraDatabase.DuplicateAura(i);
                EditorUtility.SetDirty(auraDatabase);
                AssetDatabase.SaveAssets();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = auraDatabase;
                return;
            }

            if (GUILayout.Button(i.AuraName.ToString(), GUILayout.Width(300)))
            {
                if (editorState == EditorState.Edit)
                    SaveExistingAura();
                else if (editorState == EditorState.Create)
                    SaveNewAura();

                //Get the new item and its associated data.
				selectedAura = i;
                GetAuraData();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = auraDatabase;

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

        conditionName = EditorGUILayout.TextField("Name: ", conditionName, GUILayout.Width(300));
        conditionID = EditorGUILayout.IntField("ID: ", conditionID, GUILayout.Width(300));
        conditionDesc = EditorGUILayout.TextField("Description: ", conditionDesc, GUILayout.Width(450));
        isHarmful = EditorGUILayout.Toggle("Harmful", isHarmful);
        conditionStat = (AuraStat)EditorGUILayout.EnumPopup("Affected Stat: ", conditionStat, GUILayout.Width(450));

        if (conditionStat == AuraStat.Skill)
            auraSkill = EditorGUILayout.TextField("Skill: ", auraSkill, GUILayout.Width(300));

        conditionValue = EditorGUILayout.IntField("Value: ", conditionValue, GUILayout.Width(300));
        hasDuration = EditorGUILayout.Toggle("Timed", hasDuration);
        conditionDuration = EditorGUILayout.DoubleField("Duration: ", conditionDuration, GUILayout.Width(300));

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save", GUILayout.Width(150.0f)))
        {
            // Save this item to the database, either as a new item
            // or as an existing item.
            if (editorState == EditorState.Create)
                SaveNewAura();
            else
                SaveExistingAura();

            EditorUtility.SetDirty(auraDatabase);
            editorState = EditorState.Home;
        }
        if (GUILayout.Button("Cancel", GUILayout.Width(150.0f)))
        {
            EditorUtility.SetDirty(auraDatabase);
            editorState = EditorState.Home;
        }
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.EndScrollView();
    }

    void SaveNewAura() {
		Aura newAura = new Aura();

        newAura.AuraName = conditionName;
        newAura.AuraID = conditionID;
        newAura.AuraDesc = conditionDesc;
        newAura.AuraDuration = conditionDuration;
        newAura.AuraStat = conditionStat;
        newAura.AuraValue = conditionValue;
        newAura.IsHarmful = isHarmful;
        newAura.HasDuration = hasDuration;
        newAura.AuraSkill = auraSkill;

        // Check that the given ID isn't already in the database.
        if (RequirementsMet(conditionID))
			auraDatabase.AddAura(newAura);
        else
            Debug.LogError("An item with that ID (" + newAura.AuraID + ") already exists.");

    }

    void SaveExistingAura() {
        // Check that the given ID isn't already in the database.
        if (!RequirementsMet(conditionID, selectedAura))
        {
            Debug.LogError("An item with that ID (" + conditionID + ") already exists.");
            return;
        }

        selectedAura.AuraName = conditionName;
        selectedAura.AuraID = conditionID;
        selectedAura.AuraDesc = conditionDesc;
        selectedAura.AuraDuration = conditionDuration;
        selectedAura.AuraStat = conditionStat;
        selectedAura.AuraValue = conditionValue;
        selectedAura.IsHarmful = isHarmful;
        selectedAura.HasDuration = hasDuration;
        selectedAura.AuraSkill = auraSkill;
    }

    void GetAuraData() {
        conditionName = EditorGUILayout.TextField("Name: ", selectedAura.AuraName);
        conditionID = EditorGUILayout.IntField("ID: ", selectedAura.AuraID);
        conditionDesc = EditorGUILayout.TextField("Description: ", selectedAura.AuraDesc);
        isHarmful = EditorGUILayout.Toggle("Harmful", selectedAura.IsHarmful);
        conditionStat = (AuraStat)EditorGUILayout.EnumPopup("Affected Stat: ", selectedAura.AuraStat);

        if (conditionStat == AuraStat.Skill)
            auraSkill = EditorGUILayout.TextField("Skill: ", selectedAura.AuraSkill, GUILayout.Width(300));


        conditionValue = EditorGUILayout.IntField("Value: ", selectedAura.AuraValue);
        hasDuration = EditorGUILayout.Toggle("Timed", selectedAura.HasDuration);
        conditionDuration = EditorGUILayout.DoubleField("Duration: ", selectedAura.AuraDuration);
    }

    // Check that the unique ID is not taken.
    bool RequirementsMet(int id) {
        if (id == -1)
            return true;

        foreach (Aura i in auraDatabase.Auras)
        {
            if (i.AuraID == id)
                return false;
        }

        return true;
    }

    // Check whether the unique ID is taken BY ANOTHER ITEM...
	bool RequirementsMet(int id, Aura self) {
        if (id == -1 || self == null)
            return true;

		foreach (Aura i in auraDatabase.Auras)
        {
            if (i != self && i.AuraID == id)
                return false;
        }

        return true;
    }
}
