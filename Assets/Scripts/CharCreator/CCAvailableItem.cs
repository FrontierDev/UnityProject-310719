using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CCAvailableItem : MonoBehaviour {
    GameItem gameItem;
    CCInventoryController controller;
    Inventory Inventory { get { return controller.inventory; } }

    public Text itemName;
    public InputField quantityField;
    public int ItemQuantity { get { return int.Parse(quantityField.text); } }                       // IF THE TEXT IS BLANK, IT'LL NOT ADD THE ITEM + GIVE FORMAT ERROR.

    public void SetGameItem(GameItem item, CCInventoryController _controller) {
        gameItem = item;
        controller = _controller;
        itemName.text = string.Format("{0} ({1}kg)", gameItem.ItemName, gameItem.ItemWeight);
    }

	// When clicked...
    public void AddActorItem() {
		if (Inventory == null)
			Debug.LogError ("No inventory found.");

        if(controller.WeightCheckPassed(gameItem, ItemQuantity))
        {
            Inventory.AddItem(gameItem.ItemID, ItemQuantity);
            controller.ShowStartingInventory();
        }
        else
        {
            Debug.Log("<< TOO HEAVY >>");
        }
    }
}
