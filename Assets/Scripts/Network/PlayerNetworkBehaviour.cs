using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Reflection;
using System.Linq;
using LitJson;

/// <summary>
/// Handles how the player interacts with the server.
/// </summary>
public class PlayerNetworkBehaviour : Bolt.EntityEventListener<IPlayerState> {

	float resetColourTime;  // for flash colour testing
	Renderer renderer;

	ActorStats stats;
	Inventory inventory;
    PlayerSkillset skillset;

    GameObject ui_UnitPlayer;
    readonly List<string> ui_UpdateStats = new List<string>()
        {"Health", "MaxHealth", "Energy", "MaxEnergy"}; // list of stats that are tracted on the GUI.

	GameDatabase db;

	/*
	 * NORMAL UNITY METHODS
	 */ 
	void Update() {

        // for testing
		if (resetColourTime < Time.time && renderer != null) {
			renderer.material.color = state.DebugColour;
		}
	}

	/*
	 * NETWORK-SPECIFIC
	 */ 
	public override void Attached() {
		// SETUP
		db = GameObject.FindGameObjectWithTag("Database").GetComponent<GameDatabase>();

		renderer = GetComponent<Renderer> ();
		stats = GetComponent<ActorStats> ();
		inventory = GetComponent<Inventory> ();
        skillset = GetComponent<PlayerSkillset>();

        // CUSTOMISATION TOKEN
        // Loads the player's inventory, chosen faction and race etc., 
        // which were selected in the character creation screen.
		var customisation = (CustomisationToken)entity.attachToken;
		stats.LoadCustomisation (customisation);
        //stats.LoadInterface();
		inventory.LoadInventory ();
        inventory.LoadInterface ();


		if (entity.isOwner) {
			// Sync customisation data to server...
			// BACKGROUND DATA (faction, race)
			state.BackgroundData.StartingFaction = stats.actorStartingFaction;
			state.BackgroundData.CharacterRace = stats.actorRaceName;


            // STATS
            // Gets each property in the actor's stats component. 
            // For each one, sync with the player's stats on the server.
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            PropertyInfo[] properties = stats.GetType().GetProperties(flags);
            BoltActorStatset statset = state.BoltActorStatset;
            for(int h = 0; h < properties.Length; h++)
            {
                try
                {
                    statset.Statset[h].StatName = properties[h].Name;
                    statset.Statset[h].StatValue = (System.Int32)properties[h].GetValue(stats, null);
                }
                catch (System.InvalidCastException cast)
                {
                    Debug.LogWarning(cast.Message);
                    Debug.LogWarning(properties[h].Name);
                }
            }

            // SKILLSET
            for (int i = 0; i < skillset.PlayerSkills.Count; i++)
            {
                state.BoltPlayerSkillset.Skillset[i].SkillName = skillset.PlayerSkills[i].GetSkill().SkillName;
                state.BoltPlayerSkillset.Skillset[i].SkillLevel = skillset.PlayerSkills[i].GetLevel();
            }

            SyncInventory();    
            AddRacePerks(stats.actorRaceName);  // adds the perks related to the player's race.

            // debug: give the player a random colour.
			state.DebugColour = new Color (Random.value, Random.value, Random.value);
		}

        state.AddCallback("BoltActorStatset", StatsChanged);
        state.AddCallback("BoltPlayerSkillset", SkillsetChanged);
        state.AddCallback("DebugColour", ColourChanged);
        state.AddCallback("BackgroundData", BackgroundDataChanged);
    }

	public override void SimulateController() {
		// This should go to a script in PlayerController (NYI)...

		if (Input.GetKeyDown (KeyCode.F)) {
			Debug.Log ("Colour flashed.");

			var flash = FlashColourEvent.Create (entity);
			flash.FlashColour = Color.red;
			flash.Send ();
		}
	}

	/*
	 * NETWORK CALLBACKS
	 */
	void ColourChanged () {
		renderer.material.color = state.DebugColour;
	}

	void BackgroundDataChanged () {
		stats.actorRaceName = state.BackgroundData.CharacterRace;
		stats.actorStartingFaction = state.BackgroundData.StartingFaction;
	}

    void SkillsetChanged() {
        for (int i = 0; i < skillset.PlayerSkills.Count; i++)
        {
            skillset.PlayerSkills[i].GetSkill().SkillName = state.BoltPlayerSkillset.Skillset[i].SkillName;
            skillset.PlayerSkills[i].SetLevel(state.BoltPlayerSkillset.Skillset[i].SkillLevel); 
        }
    }

    /// <summary>
    /// This is called when the player's stats are changed. It will ensure that the current val of the stat does not exceed the maximum value of the stat. 
    /// NYI: Hunger, thirst etc; will need to change to a switch 
    /// </summary>
    void StatsChanged () {
        //Debug.Log("Stat changed callback triggered");

        BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        PropertyInfo[] properties = stats.GetType().GetProperties(flags);
        Bolt.NetworkArray_Objects<BoltActorStat> statset = state.BoltActorStatset.Statset;

        try
        {
            // Find the stats which do not have the same value on the server side, so that ONLY
            // these values are updated, and the client doesn't need to compare every single stat
            // in the stat set.
            IEnumerable<PropertyInfo> query = from property in properties
                                              where (System.Int32)property.GetValue(stats, null) != statset.Where(x => x.StatName == property.Name).First().StatValue
                                              select property;

            // For the stats found in the query (which HAVE changed since StatsChanged() was called),
            // update the values but ensure they do not go about the stat's maximum value.
            foreach(PropertyInfo p in query)
            {
                // Find the ActorStat in the set of stats belonging to the actor.
                BoltActorStat targetStat = statset.Where(x => x.StatName == p.Name).First(); 
                int newValue = targetStat.StatValue;

                if (p.Name == "Health")
                {
                    // Finds the maximum value of the HEALTH stat.
                    int maxHealth = statset.Where(x => x.StatName == "MaxHealth").First().StatValue;

                    if (newValue > maxHealth)
                    {
                        newValue = maxHealth;
                        targetStat.StatValue = newValue;
                    }
                }
                else if (p.Name == "Energy")
                {
                    // Find the maximum value of the ENERGY stat.
                    int maxEnergy = statset.Where(x => x.StatName == "MaxEnergy").First().StatValue;

                    if (newValue > maxEnergy)
                    {
                        newValue = maxEnergy;
                        targetStat.StatValue = newValue;
                    }
                }

                p.SetValue(stats, newValue, null);

                // Update the health or energy bar if either has changed.
                if (ui_UpdateStats.Contains(p.Name))
                    UnitPlayerInterface.instance.UpdateBars();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    /*
     * SETUP
     */

    void SyncInventory() {
        Debug.Log("Syncing inventory to server...");
        
        // By this stage, the player prefab has loaded its inventory from the token carrier.
        // Now it needs syncing to the server: take from the LOCAL copy and transfer to the SERVER copy.

        for(int i = 0; i < inventory.inventory.Count; i++) {
            ActorItem actorItem = inventory.ItemAtIndex(i);

            state.PlayerInventory.InventoryItems[i].ItemID = actorItem.Item.ItemID;
            state.PlayerInventory.InventoryItems[i].ItemQuantity = actorItem.StackQuantity;
        }

        Debug.Log("Sync done!");

    }

    /// <summary>
    /// Adds the perks belonging to selected race to the player character.
    /// </summary>
    /// <param name="_race"></param>
    void AddRacePerks(string _race) {
        Debug.Log("Syncing actor perks...");

        // The race's perks are directly saved and editted in the Races.json file.
        JsonData raceData;
        raceData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Races.json"));

        for (int i = 0; i < raceData["Races"].Count; i++)
        {
            if(raceData["Races"][i]["RaceName"].ToString().Equals(_race))
            {
                // Add the race's trait (perk IDs):
                for (int j = 0; j < raceData["Races"][i]["RacePerks"].Count; j++)
                {
                    stats.AddPerk((int)raceData["Races"][i]["RacePerks"][j]);
                }
            }
        }

        Debug.Log("Sync done!");
    }

    /// <summary>
    /// This is called when an actor's inventory is modified.
    /// NYI: Query for only the indecies that have changed. Sync to the server.
    /// </summary>
	void InventoryChanged () {
        Debug.Log("Inventory change triggered.");
		for (int i = 0; i < inventory.inventory.Count; i++) {
			int _id = state.PlayerInventory.InventoryItems [i].ItemID;
			int _quant = state.PlayerInventory.InventoryItems [i].ItemQuantity;

			if (inventory.inventory [i].Item.ItemID != _id && inventory.inventory [i].StackQuantity != _quant)
            {
				inventory.RemoveItem (i);
				inventory.AddItem (_id, _quant);
			}
            else
            {
				inventory.inventory [i].StackQuantity = state.PlayerInventory.InventoryItems [i].ItemQuantity;
			}
		}
	}

	/*
	 * EVENTS
	 */

        // This is literally just an example event from the tutorial.
	public override void OnEvent (FlashColourEvent evnt) {
		renderer.material.color = evnt.FlashColour;
		resetColourTime = Time.time + 0.25f;
	}
}
