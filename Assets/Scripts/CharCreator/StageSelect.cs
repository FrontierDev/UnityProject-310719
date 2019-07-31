using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class StageSelect : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    public CharacterCreatorManager manager;
    public GameObject objectToEnable;

    // Calls when the mouse is clicked on this object.
    public void OnPointerClick(PointerEventData data) {
        manager.DisableActiveGroup();
		manager.SetActiveGroup(objectToEnable, this.gameObject);
    }

    // Calls when the mouse enters the object.
    public void OnPointerEnter(PointerEventData data) {
    }

    // Calls when the mouse leaves the object.
    public void OnPointerExit(PointerEventData data) {
    }
}
