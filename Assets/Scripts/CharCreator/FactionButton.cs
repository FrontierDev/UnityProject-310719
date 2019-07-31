using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class FactionButton : MonoBehaviour, IPointerClickHandler {
    public int factionID = 1;
    public CharacterCreatorManager manager;

	Faction faction;

	void Start () {
		faction = manager.gamedb.GetFaction(factionID);
		GetComponent<Image> ().sprite = faction.GetIconAsSprite ();
	}

    public void OnPointerClick(PointerEventData data) {
        manager.selectedFaction = faction;
		manager.UpdateFactionInfo(this.gameObject);
        manager.ChangeRaceList();
    }
}
