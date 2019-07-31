using UnityEngine;
using GameUtilities;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
    public class GameItem {
        // Item name
        [SerializeField] public string ItemName { get; set; }
        // Unique item ID
        [SerializeField] public int ItemID { get; set; }
        // SHORT description used in tooltip.
        // LONG description gives detailed background information on item, if applicable.
        [SerializeField] public string ItemShortDesc { get; set; }
        [SerializeField] public string ItemLongDesc { get; set; }
        // Item quality
        [SerializeField] public ItemQuality ItemQuality { get; set; }
        // Item weight in kg
        [SerializeField] public int ItemWeight { get; set; }
        // Item basic cost
        [SerializeField] public int ItemCost { get; set; }
        // Item icon
        private Texture2D ItemIcon { get; set; }
        [SerializeField] public string ItemIconPath { get; set; }
        // Item model (as game object). This needs to be converted to a path to be serialised
        // but this will be done in the Item Editor.
        private GameObject ItemModel { get; set; }
        [SerializeField] public string ItemModelPath { get; set; }
        // Item Type
        [SerializeField] public ItemType ItemType { get; set; }

        #region Type-specific stats
        // Weapon Stats
        [SerializeField] public WeaponStats WStats { get; set; }
        // Armour Stats
        [SerializeField] public ArmourStats AStats { get; set; }
        // Consumable Stats
        [SerializeField] public ConsumableStats ConStats { get; set; }
        // Ingredient Stats
        [SerializeField] public IngredientStats IngStats { get; set; }
        // Miscellaneous Stats
        [SerializeField] public MiscStats MiscStats { get; set; }
        // Container Stats
        [SerializeField] public ContainerStats CtnStats { get; set; }
        #endregion
        #region Requirements
        // Perk Requirements
		[SerializeField] public List<int> ItemPerkReqIDs = new List<int>();
        #endregion

        // Full constructor for use inside Unity. Uses Texture2D and GameObject (not their paths!)
        public GameItem(string _iName, int _iID, string _iSDesc, string _iLDesc, ItemQuality _iQual,
            int _iWeight, int _iCost, Texture2D _iIcon, GameObject _iModel, ItemType _iType) {

            this.ItemName = _iName;
            this.ItemID = _iID;
            this.ItemShortDesc = _iSDesc;
            this.ItemLongDesc = _iLDesc;
            this.ItemQuality = _iQual;
            this.ItemWeight = _iWeight;
            this.ItemCost = _iCost;
            this.ItemIcon = _iIcon;
            this.ItemModel = _iModel;
            this.ItemType = _iType;

            // Now find the Icon and Model paths.
        }

        // Full constructor for use with the database. Uses paths for icon /model.
        public GameItem(string _iName, int _iID, string _iSDesc, string _iLDesc, ItemQuality _iQual,
            int _iWeight, int _iCost, string _iIconPath, string _iModelPath, ItemType _iType) {

            this.ItemName = _iName;
            this.ItemID = _iID;
            this.ItemShortDesc = _iSDesc;
            this.ItemLongDesc = _iLDesc;
            this.ItemQuality = _iQual;
            this.ItemWeight = _iWeight;
            this.ItemCost = _iCost;
            this.ItemIconPath = _iIconPath;
            this.ItemModelPath = _iModelPath;
            this.ItemType = _iType;

            // Now find the Icon and Model at the specified paths and fill out the respective fields.
        }

        // Constructor used for duplication
        public GameItem(GameItem item) {
            this.ItemName = item.ItemName;
            this.ItemID = item.ItemID;
            this.ItemShortDesc = item.ItemShortDesc;
            this.ItemLongDesc = item.ItemLongDesc;
            this.ItemQuality = item.ItemQuality;
            this.ItemWeight = item.ItemWeight;
            this.ItemCost = item.ItemCost;
            this.ItemModel = item.ItemModel;
            this.ItemIcon = item.ItemIcon;
            this.ItemIconPath = item.ItemIconPath;
            this.ItemModelPath = item.ItemModelPath;
            this.ItemType = item.ItemType;

            this.AStats = item.AStats;
            this.WStats = item.WStats;
            this.ConStats = item.ConStats;
            this.CtnStats = item.CtnStats;
            this.IngStats = item.IngStats;
            this.MiscStats = item.MiscStats;
        }

        // Empty constructor. Will invariably cause problems.
        public GameItem() {
            this.ItemID = -1;
        }


        public void LoadIcon() {
            ItemIconPath = GameUtility.CleanItemResourcePath(ItemIconPath, "Assets/Resources/");
            ItemIconPath = GameUtility.CleanItemResourcePath(ItemIconPath, ".png");

            ItemIcon = (Texture2D)Resources.Load(ItemIconPath);
        }

        public Texture2D GetIcon() {
            return ItemIcon;
        }

        public void SetIcon(Texture2D icon) {
            ItemIcon = icon;
        }

        public void LoadModel() {
            ItemModelPath = GameUtility.CleanItemResourcePath(ItemModelPath, "Assets/Resources/");
            ItemModelPath = GameUtility.CleanItemResourcePath(ItemModelPath, ".png");

            ItemModel = (GameObject)Resources.Load(ItemModelPath);
        }

        public GameObject GetModel() {
            return ItemModel;
        }

        public void SetModel(GameObject model) {
            ItemModel = model;
        }

    }