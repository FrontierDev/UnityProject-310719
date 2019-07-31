using LitJson;
using UnityEngine;
using UnityEngine.UI;
using GameUtilities;
using System.Collections;
using System.Collections.Generic;

public class CCInventoryController : MonoBehaviour {
	public CharacterCreatorManager manager;
    public Inventory inventory;
    public int maxWeight = 50;                                  // EDITABLE BY SERVER HOST.
    int Weight { get { return inventory.InventoryWeight; } }
    public Text weightLabel;

    public GameObject equipmentPane;
    public GameObject suppliesPane;
    public GameObject miscPane;
    public GameObject availablePane;
    public GameObject loadoutPrefab;
    public GameObject availablePrefab;

    public ItemType showType;

    List<GameItem> serverLoadoutItems = new List<GameItem>();
    List<GameObject> shownAvailableObjects = new List<GameObject>();
    List<GameObject> shownInventoryObjects = new List<GameObject>();
    string url;
    ItemDatabase itemdb;
    WWW serverLoadout;

	void Start () {
        // The URL is hardcoded here but should be editable by the server host.
		url = "https://raw.githubusercontent.com/FrontierDev/UnityDatabase/master/Database%20Project/Assets/StreamingAssets/TestInventory.json";
		itemdb = manager.gamedb.itemdb;
		inventory = manager.inventory;

		Debug.LogWarning ("Started inventory controller...");
		StartCoroutine (GetAvailableItems());
	}

    // Loads the server's custom loadout into the serverLoadoutIDs list.
    IEnumerator GetAvailableItems() {
        serverLoadout = new WWW(url);
        yield return serverLoadout;

        JsonData listData = JsonMapper.ToObject(serverLoadout.text);
        for(int i = 0; i < listData["id"].Count; i++)
        {
            // Pull the item ID from the LoadoutItems json, 
            // and then add the corresponding item.
			Debug.Log("Loading item with ID: " + (int)listData["id"][i]);

            int id = (int)listData["id"][i];

			GameItem gameItem = itemdb.Item (id);
			if(gameItem.ItemID != -1)
            	serverLoadoutItems.Add(itemdb.Item(id));

        }

        ShowAvailableItems();
    }

    public void ShowStartingInventory() {
        // Destroy all existing objects in the list.
        for (int j = 0; j < shownInventoryObjects.Count; j++)
            DestroyImmediate(shownInventoryObjects[j]);

        // Create a list object for everything in the inventory.
        for (int i = 0; i < inventory.Count; i++)
        {
            // Get the actor item
            ActorItem item = inventory.ItemAtIndex(i);

            // Create the loadout prefab object
            GameObject go = Instantiate(loadoutPrefab);
            go.GetComponent<CCLoadoutItem>().SetActorItem(item, this);
            go.transform.localScale = Vector3.one;

            // Change the object's parentage:
			switch(item.Item.ItemType)
            {
                case (ItemType.Weapon):
                case (ItemType.Armour):
                    go.transform.SetParent(equipmentPane.transform);
                    break;
                case (ItemType.Consumable):
                case (ItemType.Ingredient):
                    go.transform.SetParent(suppliesPane.transform);
                    break;
                case (ItemType.Misc):
                    go.transform.SetParent(miscPane.transform);
                    break;
            }
            shownInventoryObjects.Add(go);
        }

        weightLabel.text = string.Format("Weight: {0} / {1}kg", Weight, maxWeight);
    }

    public void ShowAvailableItems() {
		Debug.Log ("Showing Available Items...");

        if (serverLoadoutItems.Count == 0)
            return;

        // Destroy all existing objects in the list.
        for (int j = 0; j < shownAvailableObjects.Count; j++)
            DestroyImmediate(shownAvailableObjects[j]);

        // Repopulate the list with new items.
        for (int i = 0; i < serverLoadoutItems.Count; i++)
        {
            if(serverLoadoutItems[i].ItemType.Equals(showType))
            {
                // Create the available item prefab object
                GameObject go = Instantiate(availablePrefab);
                go.transform.SetParent(availablePane.transform);
                go.GetComponent<CCAvailableItem>().SetGameItem(serverLoadoutItems[i], this);
                shownAvailableObjects.Add(go);
            }
        }

        Debug.LogFormat("Server allows a total of {0} items. Showing the selected {1}.",
            serverLoadoutItems.Count, shownAvailableObjects);
    }

    public bool WeightCheckPassed(GameItem item, int quant) {
        if (item.ItemWeight*quant + Weight <= maxWeight)
            return true;
        else
            return false;
    }
}
