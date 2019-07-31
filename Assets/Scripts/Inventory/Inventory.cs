using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Inventory : MonoBehaviour {
    [SerializeField] public List<ActorItem> inventory = new List<ActorItem>();
    public int InventoryWeight { get { return CalculateWeight(); } }
    internal int Count { get { return inventory.Count; } }

    GameDatabase db;
    List<GameItem> items;

    void Start() {
        db = GameObject.FindGameObjectWithTag("Database").GetComponent<GameDatabase>();
        items = db.itemdb.Items;
    }

    /// <summary>
    /// Returns the weight of the actor's inventory.
    /// </summary>
    /// <returns></returns>
    public int CalculateWeight() {
        int w = 0;

        for(int i = 0; i < inventory.Count; i++)
        {
            w += inventory[i].Item.ItemWeight*inventory[i].StackQuantity;
        }

        return w;
    }
	
    /// <summary>
    /// Add an item to the inventory according to its ID no. and its quantity.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="quantity"></param>
    public void AddItem(int id, int quantity) {
		if (db == null)
			db = GameObject.FindGameObjectWithTag ("Database").GetComponent<GameDatabase> ();
		
		ActorItem item = new ActorItem (db.GetItem (id), quantity);
		inventory.Add (item);
    }

    /// <summary>
    /// Add an item to the inventory according to a pre-existing item.
    /// </summary>
    /// <param name="item"></param>
	public void AddItem(ActorItem item) {
		inventory.Add (item);
	}
	
    /// <summary>
    /// NYI: Remove an item at a given inventory slot.
    /// </summary>
    /// <param name="slot"></param>
    public void RemoveItem(int slot) {
		
    }

    /// <summary>
    /// Removing an item from the inventory according to a pre-existing item.
    /// </summary>
    /// <param name="item"></param>
    public void RemoveItem(ActorItem item) {
        inventory.Remove(item);
    }

	public void LoadInventory() {
        /* Finds the Token Carrier. This is a persistent gob that has an inventory component, which spawns when the player connects
         * to the character creation screen. The inventory component is added to during character creation. Here, LoadInventory() takes
         * the items in the Token Carrier's inventory and adds them to this one.
         * 
         * This can be used for trading, i.e., the token could contain a temporary inventory whose contents are to be transferred over.
         * This is probably not advisable.
         */

		GameObject go = GameObject.Find ("Token Carrier");
		Inventory _inv = go.GetComponent<Inventory> ();

		for (int i = 0; i < _inv.Count; i++) {
			AddItem (_inv.ItemAtIndex (i));
		}

        /* For testing if the inventory is loaded.
         * Last test: triggered when player spawned into world (correct).
         */
        //Debug.Log("Inventory Loaded:");
        //PrintInventory();
    }

    public void LoadInterface() {
        GameObject ui = GameObject.FindGameObjectWithTag("User Interface");
        if (ui.activeInHierarchy == false)
            ui.SetActive(true);

        GameObject ui_Inventory = ui.transform.Find("Inventory Panel").gameObject;


        ui_Inventory.GetComponent<InventoryInterface>().SetInventory(this);
    }

    /// <summary>
    /// Returns the ActorItem at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public ActorItem ItemAtIndex(int index) {
        return inventory[index];
    }

    #region Testing Utilities

    public void PrintInventory()    {
        string print = "";

        for(int i = 0; i < inventory.Count; i++)
        {
            print += string.Format(i.ToString() + " " + inventory[i].Item.ItemName.ToString());
        }

        Debug.Log(print);
    }

    #endregion
}
