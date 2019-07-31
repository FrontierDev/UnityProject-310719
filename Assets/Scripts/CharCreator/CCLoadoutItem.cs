using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CCLoadoutItem : MonoBehaviour {
    // The Inventory Item which this loadout label shows.
    ActorItem actorItem;
    CCInventoryController controller;
    Inventory Inventory { get { return controller.inventory; } }

    // The loadout GUI elements.
    public Text itemName;
    public Text itemQuantity;

    public void SetActorItem (ActorItem item, CCInventoryController _controller) {
        actorItem = item;
        controller = _controller;

        itemName.text = actorItem.Item.ItemName;
        itemQuantity.text = "x" + actorItem.StackQuantity;
    }

    public void RemoveActorItem() {
        Debug.Log("Adding item with ID " + actorItem.Item.ItemID);
        Inventory.RemoveItem(actorItem);
        controller.ShowStartingInventory();
    }
}
