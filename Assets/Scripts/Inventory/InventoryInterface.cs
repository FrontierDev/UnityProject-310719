using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryInterface : MonoBehaviour {
    public List<GameObject> slots = new List<GameObject>();
    public Inventory inventory;

    public GameObject slotPrefab;
    public GameObject itemPanel;

    /// <summary>
    /// Sets this component to handle a given inventory.
    /// </summary>
    /// <param name="_inv"></param>
	public void SetInventory(Inventory _inv) {
        inventory = _inv;

        Debug.Log("Interface found; inventory set.");

        LoadItems();
    }

    /// <summary>
    /// Load the items contained in the inventory.
    /// </summary>
    public void LoadItems() {
        if(inventory == null)
        {
            Debug.LogError("Inventory not found.");
            return;
        }

        // Updates the icons and names of the item slots in the inventory on the GUI.
        for(int i = 0; i < inventory.Count; i++)
        {
            GameObject slot = Instantiate(slotPrefab);
            slot.transform.SetParent(itemPanel.transform, false);

            slot.transform.position = itemPanel.transform.position - new Vector3(-15.75f, i * 54.3f, 0);

            // Set icon...
            slot.transform.Find("Item Icon").GetComponent<RawImage>().texture = inventory.ItemAtIndex(i).Item.GetIcon();

            // Set name...
            slot.transform.Find("Item Name").GetComponent<Text>().text = inventory.ItemAtIndex(i).Item.ItemName;
        }
    }
}
