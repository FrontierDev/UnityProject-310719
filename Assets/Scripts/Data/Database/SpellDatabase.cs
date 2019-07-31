using UnityEngine;
using GameUtilities;
using LitJson;
using System.IO;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SpellDatabase : ScriptableObject {
	// The list which contains the actual spells.
	[SerializeField]
	public List<Spell> Spells = new List<Spell>();

	// Holds spell data that is pulled in from the JSON string
	JsonData spellData;

	void Start() {
		ReloadDatabase();
	}

    /// <summary>
    /// Reloads the database, or creates the JSON file if it does not exist.
    /// </summary>
	public void ReloadDatabase() {
		Debug.Log("(Re)loading spell database...");

		if (Spells == null)
			Spells = new List<Spell>();

		spellData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Spells.json"));

        // Creates the JSON file if it does not exist (should only happen when first loading the database in the editor).
		if (spellData == null)
			CreateJSONFile();

		CreateSpellDatabase();
	}

	void CreateJSONFile() {
		File.CreateText(Application.dataPath + "/StreamingAssets/Spells.json");
		ReloadDatabase();
	}

    /// <summary>
    /// Saves the database to a JSON file.
    /// </summary>
    public void SaveDatabase() {
		spellData = JsonMapper.ToJson(this);
		File.WriteAllText(Application.dataPath + "/StreamingAssets/Spells.json", spellData.ToString());
	}

    /// <summary>
    /// Reads the JSON file and creates the ScriptableObject in the Editor.
    /// </summary>
    void CreateSpellDatabase() {
		for (int i = 0; i < spellData["Spells"].Count; i++)
		{
			if(!Contains((int)spellData["Spells"][i]["SpellID"]))
			{
				Spell newSpell = new Spell();

				// Map each line in the ith JSON entry to a variable:
				newSpell.SpellName = (string)spellData["Spells"][i]["SpellName"];
				newSpell.SpellID = (int)spellData["Spells"][i]["SpellID"];
				newSpell.SpellDesc = (string)spellData["Spells"][i]["SpellDesc"];
				newSpell.SpellIconPath = (string)spellData["Spells"][i]["SpellIconpath"];
				newSpell.SpellCooldown = (double)spellData["Spells"][i]["SpellCooldown"];
				newSpell.SpellCastTime = (double)spellData ["Spells"] [i] ["SpellCastTime"];
				newSpell.SpellCastType = (SpellCastType)((int)spellData ["Spells"] [i] ["SpellCastType"]);

				for (int j = 0; j < spellData["Spells"][i]["TargetAuras"].Count; j++)
				{
					newSpell.TargetAuras.Add((int)spellData["Spells"][i]["TargetAuras"][j]);
				}
				for (int k = 0; k < spellData["Spells"][k]["CasterAuras"].Count; k++)
				{
					newSpell.CasterAuras.Add((int)spellData["Spells"][i]["CasterAuras"][k]);
				}


				// Load the spell icon.
				newSpell.LoadIcon();

				// Add this condition to the database.
				AddSpell(newSpell);
				Debug.Log("(SpellDB) " + newSpell.SpellName + " loaded.");
			}
		}
	}

	public void AddSpell(Spell i) {
		Spells.Add(i);
	}

	public void DuplicateSpell(Spell i) {
		Spell duplicate = new Spell(i);

		duplicate.SpellName = duplicate.SpellName + "copy";

        // Increases the spell ID until an unassigned ID number is found.
		while (Contains(duplicate.SpellID))
		{
			duplicate.SpellID++;
		}

		AddSpell(duplicate);
	}

	public void RemoveSpell(Spell i) {
		Spells.Remove(i);
	}

	/// <summary>
    /// Returns the reference for the spell with the given ID no.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
	public Spell spell(int id) {
		foreach (Spell i in Spells)
		{
			if (i.SpellID == id)
			{
				return i;
			}
		}

		Debug.LogError("Spell with ID " + id + " does not exist in the database.");
		return null;
	}

    /// <summary>
    /// Returns 'true' if the Spell Database contains a spell with the ID no. provided.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
	public bool Contains(int id) {
		foreach (Spell i in Spells)
		{
			if (i.SpellID == id)
			{
				return true;
			}
		}
		return false;
	}
}
