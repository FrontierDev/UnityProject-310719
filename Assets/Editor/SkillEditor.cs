using UnityEngine;
using UnityEditor;
using LitJson;
using GameUtilities;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class SkillEditor : EditorWindow {
    private SkillDatabase skillDatabase;
    private string skillAssetPath = "Assets/skillsdb.asset";

    #region Auxillary Databases
    private PerkDatabase _perkDatabase;
    private string perkAssetPath = "Assets/perkdb.asset";
    #endregion

    private EditorState editorState;
    private Skill selectedSkill;
    enum EditorState { Home, Create, Edit }
    Vector2 listScrollPos;
    Vector2 editScrollPos;
    Vector2 addedPerkScrollPos;
    Vector2 perkScrollPos;

    #region Skill Properties
    string skillName;
    int skillID;
    string skillShortDesc;
    string skillLongDesc;
    Texture2D skillIcon;
    string skillIconPath;
    List<int> skillPerkIDs = new List<int>();
    int perkID;
    #endregion


    // Add menu named "Skill Editor" to the Window menu
    [MenuItem("Window/Skill Editor")]
    static void Init() {
        // Get existing open window or if none, make a new one:
        SkillEditor editor = (SkillEditor)EditorWindow.GetWindow(typeof(SkillEditor));
        editor.minSize = new Vector2(1000, 600);
        editor.Show();
    }

    void Awake() {
        
    }

    void LoadSkillDatabase() {
        skillDatabase = AssetDatabase.LoadAssetAtPath<SkillDatabase>(skillAssetPath);

        if (skillDatabase == null)
            CreateSkillDatabase();

        if (skillDatabase.Skills == null)
            skillDatabase.ReloadDatabase();
        else
        {
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = skillDatabase;
        }
    }

    void LoadAuxillaryDatabases() {
        // PERK database
        _perkDatabase = AssetDatabase.LoadAssetAtPath<PerkDatabase>(perkAssetPath);
        _perkDatabase.ReloadDatabase();
    }

    void CreateSkillDatabase() {
        Debug.Log("Creating skill database...");

        skillDatabase = ScriptableObject.CreateInstance<SkillDatabase>();
        Debug.Log(skillDatabase);

        AssetDatabase.CreateAsset(skillDatabase, skillAssetPath);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = skillDatabase;
    }

    void OnEnable() {
        LoadSkillDatabase();
        LoadAuxillaryDatabases();
    }

    void OnGUI() {
        /*
         * Editor toolbar
         */
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create New Skill", GUILayout.Width(300)))
        {
            editorState = EditorState.Create;
            return;
        }
        if (GUILayout.Button("Reload Database", GUILayout.Width(300)))
        {
            skillDatabase.ReloadDatabase();
            return;
        }
        if (GUILayout.Button("Save to JSON", GUILayout.Width(300)))
        {
            // Delete this item from the database.
            skillDatabase.SaveDatabase();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        EditorGUILayout.EndHorizontal();

        if (skillDatabase == null || skillDatabase.Skills == null)
        {
            EditorGUILayout.LabelField("The database may need reloading.");
            return;
        }

        EditorGUILayout.BeginHorizontal();
        // List all of the items on the left hand side.
        listScrollPos = EditorGUILayout.BeginScrollView(listScrollPos, false, false, GUILayout.Width(450), GUILayout.MinHeight(550));
        foreach (Skill i in skillDatabase.Skills)
        {
            // Horizontal group per condition.
            EditorGUILayout.BeginHorizontal(GUILayout.Width(400.0f));

            if (GUILayout.Button("X", GUILayout.Width(50.0f)))
            {
                // Delete this item from the database.
                skillDatabase.RemoveSkill(i);
                EditorUtility.SetDirty(skillDatabase);
                AssetDatabase.SaveAssets();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = skillDatabase;
                return;
            }

            if (GUILayout.Button(i.SkillName.ToString(), GUILayout.Width(350.0f)))
            {
                if (editorState == EditorState.Edit)
                    SaveExistingSkill();
                else if (editorState == EditorState.Create)
                    SaveNewSkill();

                //Get the new item and its associated data.
                selectedSkill = i;
                GetConditionData();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = skillDatabase;

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
        editScrollPos = EditorGUILayout.BeginScrollView(editScrollPos, false, false, GUILayout.MinWidth(600), GUILayout.MinHeight(550));

        skillName = EditorGUILayout.TextField("Name: ", skillName, GUILayout.Width(300));
        skillID = EditorGUILayout.IntField("ID: ", skillID, GUILayout.Width(300));
        skillShortDesc = EditorGUILayout.TextField("Description: ", skillShortDesc, GUILayout.Width(450));
        EditorGUILayout.LabelField("Long description:");
        skillLongDesc = EditorGUILayout.TextArea(skillLongDesc, GUILayout.Width(450), GUILayout.MinHeight(100));
        skillIcon = EditorGUILayout.ObjectField("Icon: ", skillIcon, typeof(Texture2D), true, GUILayout.Width(450)) as Texture2D;

        EditorGUILayout.Space();

        DisplayPerks();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save", GUILayout.Width(150.0f)))
        {
            // Save this skill to the database, either as a new skill
            // or as an existing skill.
            if (editorState == EditorState.Create)
                SaveNewSkill();
            else
                SaveExistingSkill();

            EditorUtility.SetDirty(skillDatabase);
            editorState = EditorState.Home;
        }
        if (GUILayout.Button("Cancel", GUILayout.Width(150.0f)))
        {
            EditorUtility.SetDirty(skillDatabase);
            editorState = EditorState.Home;
        }
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.EndScrollView();
    }

    void SaveNewSkill() {
        Skill newSkill = new Skill();

        newSkill.SkillName = skillName;
        newSkill.SkillID = skillID;
        newSkill.SkillShortDesc = skillShortDesc;
        newSkill.SkillLongDesc = skillLongDesc;

        // Set the model and icon
        newSkill.SetIcon(skillIcon);

        // Find model path
        skillIconPath = AssetDatabase.GetAssetPath(skillIcon);
        skillIconPath = GameUtility.CleanItemResourcePath(skillIconPath, "Assets/Resources/");
        skillIconPath = GameUtility.CleanItemResourcePath(skillIconPath, ".png");
        newSkill.SkillIconPath = skillIconPath;

        newSkill.perkIDs = skillPerkIDs;

        // Check that the given ID isn't already in the database.
        if (RequirementsMet(skillID))
            skillDatabase.AddSkill(newSkill);
        else
            Debug.LogError("A skill with that ID (" + newSkill.SkillID + ") already exists.");

    }

    void SaveExistingSkill() {
        // Check that the given ID isn't already in the database.
        if (!RequirementsMet(skillID, selectedSkill))
        {
            Debug.LogError("A skill with that ID (" + skillID + ") already exists.");
            return;
        }

        selectedSkill.SkillName = skillName;
        selectedSkill.SkillID = skillID;
        selectedSkill.SkillShortDesc = skillShortDesc;
        selectedSkill.SkillLongDesc = skillLongDesc;

        // Set the model and icon
        selectedSkill.SetIcon(skillIcon);

        // Find model path
        skillIconPath = AssetDatabase.GetAssetPath(skillIcon);
        skillIconPath = GameUtility.CleanItemResourcePath(skillIconPath, "Assets/Resources/");
        skillIconPath = GameUtility.CleanItemResourcePath(skillIconPath, ".png");
        selectedSkill.SkillIconPath = skillIconPath;

        selectedSkill.perkIDs = skillPerkIDs;

    }

    void GetConditionData() {
        skillName = EditorGUILayout.TextField("Name: ", selectedSkill.SkillName);
        skillID = EditorGUILayout.IntField("ID: ", selectedSkill.SkillID);
        skillShortDesc = EditorGUILayout.TextField("Description: ", selectedSkill.SkillShortDesc);
        skillLongDesc = EditorGUILayout.TextField("Description: ", selectedSkill.SkillLongDesc);
        skillIcon = EditorGUILayout.ObjectField("Icon: ", selectedSkill.GetIcon(), typeof(Texture2D), true) as Texture2D;

        skillPerkIDs = selectedSkill.perkIDs;
    }

    void DisplayPerks() {
        EditorGUILayout.LabelField("Assigned Perks", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        // Display the currently-added perks
        addedPerkScrollPos = EditorGUILayout.BeginScrollView(addedPerkScrollPos, false, false, GUILayout.MinWidth(270), GUILayout.Height(180));
        for (int i = 0; i < skillPerkIDs.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("X", GUILayout.Width(50)))
            {
                skillPerkIDs.RemoveAt(i);
                break;
            }
            EditorGUILayout.LabelField("Level " + _perkDatabase.perk(skillPerkIDs[i]).PerkUnlockLevel + " -- " + _perkDatabase.perk(skillPerkIDs[i]).PerkName);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        // Search for perks in the perk database
        perkScrollPos = EditorGUILayout.BeginScrollView(perkScrollPos, false, false, GUILayout.MinWidth(270), GUILayout.Height(180));
        for(int j = 0; j < _perkDatabase.Perks.Count; j++)
        {
            if (skillPerkIDs.Contains(_perkDatabase.Perks[j].PerkID))
                continue;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+", GUILayout.Width(50)))
            {
                perkID = _perkDatabase.Perks[j].PerkID;

                if(PerkRequirementsMet())
                {
                    skillPerkIDs.Add(perkID);
                    SortPerklist();
                }

            }
            EditorGUILayout.LabelField(_perkDatabase.Perks[j].PerkName + " (ID " + _perkDatabase.Perks[j].PerkID + ")");
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // Display a box which allows the user to add new conditions by their IDs
        perkID = EditorGUILayout.IntField("Perk ID: ", perkID, GUILayout.Width(300));
        if (GUILayout.Button("Add New Perk", GUILayout.Width(300)))
        {
            if(PerkRequirementsMet())
            {
                skillPerkIDs.Add(perkID);
                SortPerklist();
            }
        }
    }

    bool PerkRequirementsMet () {
        if (!_perkDatabase.Contains(perkID))
        {
            Debug.LogError("Perk with ID " + perkID + " does not exist in the database.");
            return false;
        }

        if (skillPerkIDs.Contains(perkID))
        {
            Debug.LogError("This perk has already been added.");
            return false;
        }

        return true;
    }

    // Check that the unique ID is not taken.
    bool RequirementsMet(int id) {
        if (id == -1)
            return true;

        foreach (Skill i in skillDatabase.Skills)
        {
            if (i.SkillID == id)
                return false;
        }

        return true;
    }

    // Check whether the unique ID is taken BY ANOTHER ITEM...
    bool RequirementsMet(int id, Skill self) {
        if (id == -1 || self == null)
            return true;

        foreach (Skill i in skillDatabase.Skills)
        {
            if (i != self && i.SkillID == id)
                return false;
        }

        return true;
    }

    void SortPerklist() {
        skillPerkIDs.Sort();
    }
}
