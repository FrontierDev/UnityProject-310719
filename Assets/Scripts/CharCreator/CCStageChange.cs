using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class CCStageChange : MonoBehaviour, IPointerClickHandler {
    public bool forwardOne;
    public CharacterCreatorManager manager;

    public void OnPointerClick(PointerEventData data) {
        if (forwardOne)
        {
            manager.GoToStage(manager.stage + 1);
        }
        else
        {
            manager.GoToStage(manager.stage - 1);
        }
    }
}
