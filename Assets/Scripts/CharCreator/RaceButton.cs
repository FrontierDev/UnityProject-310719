using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class RaceButton : MonoBehaviour, IPointerClickHandler {
    public Race race;
    public CharacterCreatorManager manager;
    public string raceName;

    public void SetRace(Race _race) {
        race = _race;
        raceName = race.RaceName;
    }

    public void OnPointerClick(PointerEventData data) {
        manager.selectedRace = race;
        manager.UpdateRaceInfo();
    }
}
