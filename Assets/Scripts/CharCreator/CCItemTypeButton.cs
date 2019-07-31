using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class CCItemTypeButton : MonoBehaviour, IPointerClickHandler {
    public GameUtilities.ItemType itemType;
    public CCInventoryController controller;

    public void OnPointerClick (PointerEventData data) {
        controller.showType = itemType;
        Debug.Log(controller.showType.ToString() + itemType.ToString());
        controller.ShowAvailableItems();
    }
}
