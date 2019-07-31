using UnityEngine;
using System.Collections;

/// <summary>
/// This component will load the game's databases when the game first starts.
/// </summary>
public class GameDatabase : MonoBehaviour {
    public ItemDatabase itemdb;
	public AuraDatabase conddb;
    public PerkDatabase perkdb;
    public SkillDatabase skilldb;
    public FactionDatabase factiondb;
    public NPCDatabase npcdb;

    void Awake() {
        DontDestroyOnLoad(transform.gameObject);
        LoadDatabase();
    }

    void LoadDatabase() {
        itemdb = ScriptableObject.CreateInstance<ItemDatabase>();
        conddb = ScriptableObject.CreateInstance<AuraDatabase>();
        perkdb = ScriptableObject.CreateInstance<PerkDatabase>();
        skilldb = ScriptableObject.CreateInstance<SkillDatabase>();
        npcdb = ScriptableObject.CreateInstance<NPCDatabase>();
        factiondb = ScriptableObject.CreateInstance<FactionDatabase>();

        itemdb.ReloadDatabase();
        conddb.ReloadDatabase();
        perkdb.ReloadDatabase();
        skilldb.ReloadDatabase();
        npcdb.ReloadDatabase();
        factiondb.ReloadDatabase();
    }

    public GameItem GetItem(int id) {
        for(int i = 0; i < itemdb.Items.Count; i++)
        {
            if(itemdb.Items[i].ItemID == id)
            {
                return itemdb.Items[i];
            }
        }

        Debug.LogWarning("Item ID " + id + " not found.");
        return new GameItem();
    }

    /// <summary>
    /// Get faction by ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Faction GetFaction(int id) {
        for (int i = 0; i < factiondb.Factions.Count; i++)
        {
            if (factiondb.Factions[i].FactionID == id)
            {
                return factiondb.Factions[i];
            }
        }

        Debug.LogWarning("Faction ID " + id + " not found.");
        return new Faction();
    }

    /// <summary>
    /// Get faction by name
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Faction GetFaction(string name) {
        for (int i = 0; i < factiondb.Factions.Count; i++)
        {
            if (factiondb.Factions[i].FactionName.Equals(name))
            {
                return factiondb.Factions[i];
            }
        }

        Debug.LogWarning("Faction " + name + " not found.");
        return new Faction();
    }
}
