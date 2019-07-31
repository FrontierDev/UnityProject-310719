using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharacterCreatorManager : Bolt.GlobalEventListener {
    public GameDatabase gamedb;
    public GameObject player;

    public string CharacterName { get; set; }
    public Faction selectedFaction;
    public Race selectedRace;
    public Inventory inventory;
    bool inventoryLoaded = false;

    public CCFactionInfoController factionInfo;
    public CCRaceInfoController raceInfo;
    public CCInventoryController inventoryInfo;
    string url;

    // The active UI group (ie. <X> Stage GUI)
    public GameObject activeGroup;
	public GameObject activeGroupIcon;
    public int stage;
    public GameObject[] stages;
	public GameObject[] stageIcons;

	void Start() {
		for (int i = 0; i < stageIcons.Length; i++) {
			if(!stageIcons[i].Equals(activeGroupIcon))
				stageIcons[i].GetComponent<Image> ().canvasRenderer.SetAlpha (0.5f);
		}
	}

    public void GoToStage(int index) {
        if (index > stages.Length || index < 0)
            return;

        DisableActiveGroup();
		SetActiveGroup(stages[index], stageIcons[index]);
        stage = index;
    }

	public void SetActiveGroup(GameObject group, GameObject icon) {
        activeGroup = group;
		activeGroupIcon = icon;

        activeGroup.SetActive(true);
		activeGroupIcon.GetComponent<Image> ().canvasRenderer.SetAlpha (2.0f);
    }

    public void DisableActiveGroup() {
        activeGroup.SetActive(false);
		activeGroupIcon.GetComponent<Image> ().canvasRenderer.SetAlpha (0.5f);
    }

    public void ChangeRaceList() {
        raceInfo.ChangeFactionRaces(selectedFaction.FactionRaceList);
    }

	public void UpdateFactionInfo(GameObject iconButton) {
		factionInfo.UpdateInfo(selectedFaction, iconButton);
    }

    public void UpdateRaceInfo() {
        raceInfo.UpdateInfo(selectedRace, gamedb.perkdb);
    }
}
