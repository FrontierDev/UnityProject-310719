using UnityEngine;
using UnityEditor;
using LitJson;
using GameUtilities;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SpellEditor : EditorWindow {
	private SpellDatabase spellDatabase;
	//private List<Spell> conditions = new List<Spell>();
	private string spellAssetPath = "Assets/spelldb.asset";

	#region Auxillary Databases
	private AuraDatabase _auraDatabase;
	private string auraPath = "Assets/auradb.asset";
	#endregion

	private EditorState editorState;
	private Spell selectedSpell;
	enum EditorState { Home, Create, Edit }
	Vector2 listScrollPos;
	Vector2 editScrollPos;
    Vector2 addedCasterAuraScrollPos = new Vector2();
	Vector2 casterAuraScrollPos = new Vector2();
	Vector2 addedTargetAuraScrollPos = new Vector2();
	Vector2 targetAuraScrollPos = new Vector2();

	#region Spell Properties
	string spellName;
	int spellID;
	string spellDesc;
	double spellCastTime;
	double spellCooldown;
	SpellCastType spellCastType;
	string spellIconPath;
	Texture2D spellIcon;
	List<int> targetAuras = new List<int>();
	List<int> casterAuras = new List<int>();
	#endregion

	int auraID;


	// Add menu named "Spell Editor" to the Window menu
	[MenuItem("Window/Spell Editor")]
	static void Init() {
		// Get existing open window or if none, make a new one:
		SpellEditor editor = (SpellEditor)EditorWindow.GetWindow(typeof(SpellEditor));
		editor.minSize = new Vector2(1000, 600);
		editor.Show();
	}

	void Awake() {
		LoadSpellDatabase();
		LoadAuxillaryDatabases ();
	}

	void LoadSpellDatabase() {
		spellDatabase = AssetDatabase.LoadAssetAtPath<SpellDatabase>(spellAssetPath);

		if (spellDatabase == null)
			CreateSpellDatabase();
		else
		{
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = spellDatabase;
		}
	}

	void LoadAuxillaryDatabases() {
		// CONDITION database
		_auraDatabase = AssetDatabase.LoadAssetAtPath<AuraDatabase>(auraPath);
		_auraDatabase.ReloadDatabase();
	}

	void CreateSpellDatabase() {
		Debug.Log("Creating spell database...");

		spellDatabase = ScriptableObject.CreateInstance<SpellDatabase>();
		Debug.Log(spellDatabase);

		AssetDatabase.CreateAsset(spellDatabase, "Assets/spelldb.asset");
		AssetDatabase.SaveAssets();
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = spellDatabase;
	}

	void OnEnable() {

	}

	void OnGUI() {
		/*
         * Editor toolbar
         */
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Create New Spell", GUILayout.Width(300)))
		{
			editorState = EditorState.Create;
			return;
		}
		if (GUILayout.Button("Reload Database", GUILayout.Width(300)))
		{
			spellDatabase.ReloadDatabase();
			return;
		}
		if (GUILayout.Button("Save to JSON", GUILayout.Width(300)))
		{
			// Delete this item from the database.
			spellDatabase.SaveDatabase();
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
		EditorGUILayout.EndHorizontal();

		if (spellDatabase == null || spellDatabase.Spells == null)
		{
			EditorGUILayout.LabelField("The database may need reloading.");
			return;
		}

		EditorGUILayout.BeginHorizontal();
		// List all of the items on the left hand side.
		listScrollPos = EditorGUILayout.BeginScrollView(listScrollPos, false, false, GUILayout.Width(450), GUILayout.MinHeight(550));
		foreach (Spell i in spellDatabase.Spells)
		{
			// Horizontal group per condition.
			EditorGUILayout.BeginHorizontal(GUILayout.Width(400.0f));

			if (GUILayout.Button("X", GUILayout.Width(50.0f)))
			{
				// Delete this item from the database.
				spellDatabase.RemoveSpell(i);
				EditorUtility.SetDirty(spellDatabase);
				AssetDatabase.SaveAssets();
				EditorUtility.FocusProjectWindow();
				Selection.activeObject = spellDatabase;
				return;
			}

			if (GUILayout.Button("C", GUILayout.Width(50.0f)))
			{
				// Duplicate this item.
				spellDatabase.DuplicateSpell(i);
				EditorUtility.SetDirty(spellDatabase);
				AssetDatabase.SaveAssets();
				EditorUtility.FocusProjectWindow();
				Selection.activeObject = spellDatabase;
				return;
			}

			if (GUILayout.Button(i.SpellName.ToString(), GUILayout.Width(300)))
			{
				if (editorState == EditorState.Edit)
					SaveExistingSpell();
				else if (editorState == EditorState.Create)
					SaveNewSpell();

				//Get the new item and its associated data.
				selectedSpell = i;
				GetSpellData();
				EditorUtility.FocusProjectWindow();
				Selection.activeObject = spellDatabase;

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

		spellName = EditorGUILayout.TextField("Name: ", spellName, GUILayout.Width(300));
		spellID = EditorGUILayout.IntField("ID: ", spellID, GUILayout.Width(300));
		spellDesc = EditorGUILayout.TextField("Description: ", spellDesc, GUILayout.Width(450));
        spellCastTime = EditorGUILayout.DoubleField("Cast Time: ", spellCastTime, GUILayout.Width(300));
        spellCooldown = EditorGUILayout.DoubleField("Cooldown: ", spellCooldown, GUILayout.Width(300));
        spellIcon = EditorGUILayout.ObjectField("Icon: ", spellIcon, typeof(Texture2D), true, GUILayout.Width(450)) as Texture2D;
        spellCastType = (SpellCastType)EditorGUILayout.EnumPopup("Cast Type: ", spellCastType, GUILayout.Width(450));
		
		DisplayAuraList (targetAuras, addedTargetAuraScrollPos, targetAuraScrollPos, "TARGET");
		DisplayAuraList (casterAuras, addedCasterAuraScrollPos, casterAuraScrollPos, "CASTER");

		EditorGUILayout.Space();

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Save", GUILayout.Width(150.0f)))
		{
			// Save this item to the database, either as a new item
			// or as an existing item.
			if (editorState == EditorState.Create)
				SaveNewSpell();
			else
				SaveExistingSpell();

			EditorUtility.SetDirty(spellDatabase);
			editorState = EditorState.Home;
		}
		if (GUILayout.Button("Cancel", GUILayout.Width(150.0f)))
		{
			EditorUtility.SetDirty(spellDatabase);
			editorState = EditorState.Home;
		}
		EditorGUILayout.EndHorizontal();


		EditorGUILayout.EndScrollView();
	}

	void SaveNewSpell() {
		Spell newSpell = new Spell();

		newSpell.SpellName = spellName;
		newSpell.SpellID = spellID;
		newSpell.SpellDesc = spellDesc;
        newSpell.SpellCastTime = spellCastTime;
        newSpell.SpellCooldown = spellCooldown;
        newSpell.TargetAuras = targetAuras;
        newSpell.CasterAuras = casterAuras;

        // Set icon
        newSpell.SetIcon(spellIcon);

        // Find icon path
        spellIconPath = AssetDatabase.GetAssetPath(spellIcon);
        spellIconPath = GameUtility.CleanItemResourcePath(spellIconPath, "Assets/Resources/");
        spellIconPath = GameUtility.CleanItemResourcePath(spellIconPath, ".png");
        newSpell.SpellIconPath = spellIconPath;

        // Check that the given ID isn't already in the database.
        if (RequirementsMet(spellID))
			spellDatabase.AddSpell(newSpell);
		else
			Debug.LogError("A spell with that ID (" + newSpell.SpellID + ") already exists.");

	}

	void SaveExistingSpell() {
		// Check that the given ID isn't already in the database.
		if (!RequirementsMet(spellID, selectedSpell))
		{
			Debug.LogError("A spell with that ID (" + spellID + ") already exists.");
			return;
		}

		selectedSpell.SpellName = spellName;
		selectedSpell.SpellID = spellID;
		selectedSpell.SpellDesc = spellDesc;
        selectedSpell.SpellCastTime = spellCastTime;
        selectedSpell.SpellCooldown = spellCooldown;
        selectedSpell.TargetAuras = targetAuras;
        selectedSpell.CasterAuras = casterAuras;

        // Set icon
        selectedSpell.SetIcon(spellIcon);

        // Find icon path
        spellIconPath = AssetDatabase.GetAssetPath(spellIcon);
        spellIconPath = GameUtility.CleanItemResourcePath(spellIconPath, "Assets/Resources/");
        spellIconPath = GameUtility.CleanItemResourcePath(spellIconPath, ".png");
        selectedSpell.SpellIconPath = spellIconPath;
    }

	void GetSpellData() {
		spellName = EditorGUILayout.TextField("Name: ", selectedSpell.SpellName);
		spellID = EditorGUILayout.IntField("ID: ", selectedSpell.SpellID);
		spellDesc = EditorGUILayout.TextField("Description: ", selectedSpell.SpellDesc);
        spellCastTime = EditorGUILayout.DoubleField("Cast Time: ", selectedSpell.SpellCastTime);
        spellCooldown = EditorGUILayout.DoubleField("Cooldown: ", selectedSpell.SpellCooldown);
        spellIcon = EditorGUILayout.ObjectField("Icon: ", selectedSpell.GetIcon(), typeof(Texture2D), true) as Texture2D;
        spellCastType = (SpellCastType)EditorGUILayout.EnumPopup("Cast Type: ", selectedSpell.SpellCastType);
        casterAuras = selectedSpell.CasterAuras;
        CheckAuras(casterAuras);
        targetAuras = selectedSpell.TargetAuras;
        CheckAuras(targetAuras);
    }

    void CheckAuras(List<int> auraList)
    {
        List<int> indexToRemove = new List<int>();
        for (int i = 0; i < auraList.Count; i++)
        {
            if (!_auraDatabase.Contains(auraList[i]))
            {
                indexToRemove.Add(i);
            }
        }

        if (indexToRemove.Count == 0)
            return;

        for (int j = 0; j < indexToRemove.Count; j++)
        {
            auraList.RemoveAt(j);
            Debug.LogWarning("A perk was removed from this entry as it no longer exists in the perks database.");
        }
        EditorUtility.SetDirty(spellDatabase);
        AssetDatabase.SaveAssets();
    }

    void DisplayAuraList(List<int> auraList, Vector2 addedScrollPos, Vector2 availScrollPos, string header) {
		EditorGUILayout.LabelField("Assigned " + header + " Auras", EditorStyles.boldLabel);
		EditorGUILayout.BeginHorizontal();
		// Display the currently-added conditions
		addedScrollPos = EditorGUILayout.BeginScrollView(addedScrollPos, false, false, GUILayout.MinWidth(250), GUILayout.Height(180));
		for (int i = 0; i < auraList.Count; i++)
		{
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("X", GUILayout.Width(50)))
			{
				auraList.RemoveAt(i);
				break;
			}
			EditorGUILayout.LabelField(_auraDatabase.Aura(auraList[i]).AuraName + " (ID " + _auraDatabase.Aura(auraList[i]).AuraID + ")");
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndScrollView();
		// Search for perks in the perk database
		availScrollPos = EditorGUILayout.BeginScrollView(availScrollPos, false, false, GUILayout.MinWidth(250), GUILayout.Height(180));
		for (int j = 0; j < _auraDatabase.Auras.Count; j++)
		{
			int _id = _auraDatabase.Auras[j].AuraID;
			string _name = _auraDatabase.Auras[j].AuraName;

			if (auraList.Contains(_id))
				continue;

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("+", GUILayout.Width(50)))
			{
				auraID = _id;

				if (AuraRequirementsMet(auraList))
				{
					auraList.Add(auraID);
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
		auraID = EditorGUILayout.IntField("Aura ID: ", auraID, GUILayout.Width(350.0f));
		if (GUILayout.Button("Add", GUILayout.Width(350.0f)))
		{
			if (AuraRequirementsMet(auraList))
				auraList.Add(auraID);
		}
	}

	bool AuraRequirementsMet(List<int> auraList) {
		if (!_auraDatabase.Contains(auraID))
		{
			Debug.LogError("Aura with ID " + auraID + " does not exist in the database.");
			return false;
		}

		if (auraList.Contains(auraID))
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

		foreach (Spell i in spellDatabase.Spells)
		{
			if (i.SpellID == id)
				return false;
		}

		return true;
	}

	// Check whether the unique ID is taken BY ANOTHER ITEM...
	bool RequirementsMet(int id, Spell self) {
		if (id == -1 || self == null)
			return true;

		foreach (Spell i in spellDatabase.Spells)
		{
			if (i != self && i.SpellID == id)
				return false;
		}

		return true;
	}
}
