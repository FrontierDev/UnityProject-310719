using LitJson;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class CCRaceInfoController : MonoBehaviour {
    public Image icon;
    public Text header;
    public Text traits;
    public Text description;

    public List<string> factionRaces = new List<string>();
    public List<GameObject> raceButtons = new List<GameObject>();

    List<Race> races = new List<Race>();
    JsonData raceData;

    public void ChangeFactionRaces(List<string> _races) {
        // Change the faction races to equal the list of races defined in the Faction JSON file
        factionRaces = _races;
        GetFactionRaces();

        // Change the string value of every RaceButton component on each of the Race Buttons.
        for(int i = 0; i < races.Count; i++)
        {
            raceButtons[i].GetComponent<RaceButton>().SetRace(races[i]);
        }
    }

    void GetFactionRaces() {
        races.Clear();

        if(raceData == null)
            raceData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Races.json"));

        for (int i = 0; i < raceData["Races"].Count; i++)
        {
            if(factionRaces.Contains(raceData["Races"][i]["RaceName"].ToString()))
            {
                Race newRace = new Race();

                // Assign all of JSON data to the New Race struct:
                newRace.RaceName = (string)raceData["Races"][i]["RaceName"];
                newRace.RaceDescPath = (string)raceData["Races"][i]["RaceDescPath"];
                newRace.RaceIconPath = (string)raceData["Races"][i]["RaceIconPath"];
                
                // Add the race's trait (perk IDs):
                for(int j = 0; j < raceData["Races"][i]["RacePerks"].Count; j++)
                {
                    newRace.RacePerks.Add((int)raceData["Races"][i]["RacePerks"][j]);
                }

                races.Add(newRace);
            }
        }
    }

    public void UpdateInfo(Race race, PerkDatabase perks) {
        icon.sprite = Resources.Load(race.RaceIconPath) as Sprite;
        header.text = race.RaceName;
		description.text = File.ReadAllText(Application.dataPath + "/StreamingAssets" + race.RaceDescPath);

        // Show the race traits:
        traits.text = "Bonus Trait(s): \n";
        if (race.RacePerks == null || race.RacePerks.Count == 0)
            return;

        for(int i = 0; i < race.RacePerks.Count; i++)
        {
            if(perks.Contains(race.RacePerks[i]))
                traits.text += perks.perk(race.RacePerks[i]).PerkName + "\n";
        }
    }
}
