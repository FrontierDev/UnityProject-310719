using UnityEngine;
using GameUtilities;
using LitJson;
using System.IO;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ItemDatabase : ScriptableObject {
    // The list which contains the actual items.
    [SerializeField] public List<GameItem> Items { get; set; }

    // Holds item data that is pulled in from the JSON string
    JsonData itemData;

    void Start() {
        ReloadDatabase();
    }

    /// <summary>
    /// Reloads the database, or creates the JSON file if it does not exist.
    /// </summary>
    public void ReloadDatabase() {
        Debug.Log("(Re)loading item database...");

        if (Items == null)
            Items = new List<GameItem>();

        itemData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Items.json"));

        if (itemData == null)
            CreateJSONFile();

        CreateItemDatabase();
    }

    void CreateJSONFile() {
        File.CreateText(Application.dataPath + "/StreamingAssets/Items.json");
        ReloadDatabase();
    }

    /// <summary>
    /// Saves the database to a JSON file.
    /// </summary>
    public void SaveDatabase() {
        itemData = JsonMapper.ToJson(this);
        File.WriteAllText(Application.dataPath + "/StreamingAssets/Items.json", itemData.ToString());
    }

    // This extracts information from the JSON database (through itemData)
    void CreateItemDatabase() {
        for(int i = 0; i < itemData["Items"].Count; i++)
        {
            if(!Contains((int)itemData["Items"][i]["ItemID"]))
            {
                GameItem newItem = new GameItem();

                // Map each line in the ith JSON entry to a variable:
                newItem.ItemName = (string)itemData["Items"][i]["ItemName"];
                newItem.ItemID = (int)itemData["Items"][i]["ItemID"];
                newItem.ItemShortDesc = (string)itemData["Items"][i]["ItemShortDesc"];
                newItem.ItemLongDesc = (string)itemData["Items"][i]["ItemLongDesc"];
                newItem.ItemWeight = (int)itemData["Items"][i]["ItemWeight"];
                newItem.ItemCost = (int)itemData["Items"][i]["ItemCost"];
                newItem.ItemIconPath = (string)itemData["Items"][i]["ItemIconPath"];
                newItem.ItemModelPath = (string)itemData["Items"][i]["ItemModelPath"];
                newItem.ItemQuality = (ItemQuality)((int)itemData["Items"][i]["ItemQuality"]);
                newItem.ItemType = (ItemType)((int)itemData["Items"][i]["ItemType"]);

                // PERK requirements
                if (itemData["Items"][i]["ItemPerkReqIDs"].Count != 0)
                {
                    for (int p = 0; p < itemData["Items"][i]["ItemPerkReqIDs"].Count; p++)
                    {
                        newItem.ItemPerkReqIDs.Add((int)itemData["Items"][i]["ItemPerkReqIDs"][p]);
                    }
                }

                // Get the model and icon from the given paths
                newItem.LoadIcon();
                newItem.LoadModel();

                switch (newItem.ItemType)
                {
                    // If the items is WEAPON...
                    case (ItemType.Weapon):
                        WeaponStats newWStats = new WeaponStats();

                        // Map each line in the weapons array to a weapon-only variable
                        newWStats.AttackSpeed = (int)itemData["Items"][i]["WStats"]["AttackSpeed"];
                        newWStats.BaseDamage = (int)itemData["Items"][i]["WStats"]["BaseDamage"];
                        newWStats.BluntDamage = (int)itemData["Items"][i]["WStats"]["BluntDamage"];
                        newWStats.PierceDamage = (int)itemData["Items"][i]["WStats"]["PierceDamage"];
                        newWStats.SlashDamage = (int)itemData["Items"][i]["WStats"]["SlashDamage"];
                        newWStats.WeaponType = (WeaponType)((int)itemData["Items"][i]["WStats"]["WeaponType"]);
                        for (int m = 0; m < itemData["Items"][i]["WStats"]["Auras"].Count; m++)
                        {
                            newWStats.Auras.Add((int)itemData["Items"][i]["WStats"]["Auras"][m]);
                        }

                        // Add this to the new item.
                        newItem.WStats = newWStats;
                        break;

                    // If the item is ARMOUR...
                    case (ItemType.Armour):
                        ArmourStats newAStats = new ArmourStats();

                        // Map each line in the armour array to a armour-only variable
                        newAStats.BaseDefence = (int)itemData["Items"][i]["AStats"]["BaseDefence"];
                        newAStats.BluntDefence = (int)itemData["Items"][i]["AStats"]["BluntDefence"];
                        newAStats.PierceDefence = (int)itemData["Items"][i]["AStats"]["PierceDefence"];
                        newAStats.SlashDefence = (int)itemData["Items"][i]["AStats"]["SlashDefence"];
                        newAStats.ThermalDefence = (int)itemData["Items"][i]["AStats"]["ThermalDefence"];
                        newAStats.NatureDefence = (int)itemData["Items"][i]["AStats"]["NatureDefence"];
                        newAStats.ArmourMaterial = (ArmourMaterial)((int)itemData["Items"][i]["AStats"]["ArmourMaterial"]);
                        newAStats.ArmourType = (ArmourType)((int)itemData["Items"][i]["AStats"]["ArmourType"]);
                        for (int n = 0; n < itemData["Items"][i]["AStats"]["Auras"].Count; n++)
                        {
                            newAStats.Auras.Add((int)itemData["Items"][i]["AStats"]["Auras"][n]);
                        }

                        // Add this to the new item.
                        newItem.AStats = newAStats;
                        break;

                    // If the item is CONSUMABLE...
                    case (ItemType.Consumable):
                        ConsumableStats newConStats = new ConsumableStats();

                        // Map each line in the consumable array to a consumable-only variable
                        newConStats.ConsumableType = (ConsumableType)((int)itemData["Items"][i]["ConStats"]["ConsumableType"]);
                        newConStats.Charges = (int)itemData["Items"][i]["ConStats"]["Charges"];
                        for (int j = 0; j < itemData["Items"][i]["ConStats"]["Auras"].Count; j++)
                        {
                            newConStats.Auras.Add((int)itemData["Items"][i]["ConStats"]["Auras"][j]);
                        }

                        // Add this to the new item.
                        newItem.ConStats = newConStats;
                        break;

                    // If the item is CONTAINER...
                    case (ItemType.Container):
                        ContainerStats newCtnStats = new ContainerStats();

                        // Map each line in the consumable array to a consumable-only variable
                        for (int k = 0; k < itemData["Items"][i]["CtnStats"]["ContentItems"].Count; k++)
                        {
                            newCtnStats.ContentItems.Add((int)itemData["Items"][i]["CtnStats"]["ContentItems"][k]);
                            newCtnStats.ContentQuantities.Add((int)itemData["Items"][i]["CtnStats"]["ContentQuantities"][k]);

                        }

                        newCtnStats.CombineContents();

                        // Add this to the new item.
                        newItem.CtnStats = newCtnStats;
                        break;

                    // If the item is CONTAINER...
                    case (ItemType.Ingredient):
                        IngredientStats newIngStats = new IngredientStats();

                        // Map each line in the container array to a container-only variable
                        newIngStats.IsStackable = (bool)itemData["Items"][i]["IngStats"]["IsStackable"];

                        // Add this to the new item.
                        newItem.IngStats = newIngStats;
                        break;

                    // If the item is MISC...
                    case (ItemType.Misc):
                        MiscStats newMiscStats = new MiscStats();

                        // Map each line in the container array to a container-only variable
                        newMiscStats.IsStackable = (bool)itemData["Items"][i]["MiscStats"]["IsStackable"];

                        // Add this to the new item.
                        newItem.MiscStats = newMiscStats;
                        break;
                }


                // Add this item to the database.
                AddItem(newItem);



                //Debug.Log("(ItemDB) " + newItem.ItemName + " loaded.");
            }
        }
    }

    public void AddItem(GameItem i) {
        Items.Add(i);
    }

    public void DuplicateItem(GameItem i) {
        GameItem duplicate = new GameItem(i);

        duplicate.ItemName = duplicate.ItemName + "copy";
        while(Contains(duplicate.ItemID))
        {
            duplicate.ItemID++;
        }

        AddItem(duplicate);
    }

    public void RemoveItem(GameItem i) {
        Items.Remove(i);
    }

    // Get GameItem reference by ID
    public GameItem Item(int id) {
        foreach(GameItem i in Items)
        {
            if(i.ItemID == id)
            {
                return i;
            }
        }

		Debug.LogWarning("GameItem with ID " + id + " does not exist in the database.");
		return new GameItem();
    }

    public bool Contains(int id) {
        foreach (GameItem i in Items)
        {
            if (i.ItemID == id)
            {
                return true;
            }
        }

        return false;
    }
}
