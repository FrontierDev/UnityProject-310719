using UnityEngine;
using System.Collections;

/// <summary>
/// The "GameItem" class is simply a template for an item's attributes that exists on the database.
/// These templates are not used directly. Instead, an ActorItem stores the reference for that 
/// item so that its attributes can be read, as well as how the quantity of that item which is
/// stored in the inventory.
/// </summary>
[System.Serializable]
public class ActorItem {
	[SerializeField] public GameItem Item {get; protected set;}
	[SerializeField] public int StackQuantity {get; set;}

    public ActorItem (GameItem baseItem, int quantity) {
		Item = baseItem;
		StackQuantity = quantity;
    }
}
