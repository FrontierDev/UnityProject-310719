using UnityEngine;
using GameUtilities;
using System.Collections;
using System.Collections.Generic;

public class Perk {
    // Perk name
    [SerializeField]
    public string PerkName { get; set; }
    // Perk unique id
    [SerializeField]
    public int PerkID { get; set; }
    // Perk descriptions. The short description is shown on a tooltip.
    [SerializeField]
    public string PerkDesc { get; set; }
    // Skill icon to show in the character window.
    private Texture2D PerkIcon { get; set; }
    [SerializeField]
    public string PerkIconpath { get; set; }
    // The level at which this perk is unlocked (DEFAULT).
    [SerializeField]
    public int PerkUnlockLevel { get; set; }
    // The IDs of the conditions which this perk applies when it is unlocked.
    [SerializeField]
    public List<int> PerkAuras = new List<int>();

    // Constructor used for duplicating the perk.
    public Perk(Perk perk) {
        this.PerkName = perk.PerkName;
        this.PerkID = perk.PerkID;
        this.PerkDesc = perk.PerkDesc;
        this.PerkIcon = perk.PerkIcon;
        this.PerkIconpath = perk.PerkIconpath;
        this.PerkUnlockLevel = perk.PerkUnlockLevel;
        this.PerkAuras = perk.PerkAuras;
    }

    // Empty constructor.
    public Perk() {

    }

    public void LoadIcon() {
        PerkIconpath = GameUtility.CleanItemResourcePath(PerkIconpath, "Assets/Resources/");
        PerkIconpath = GameUtility.CleanItemResourcePath(PerkIconpath, ".png");

        PerkIcon = (Texture2D)Resources.Load(PerkIconpath);
    }

    public Texture2D GetIcon() {
        return PerkIcon;
    }

    public void SetIcon(Texture2D icon) {
        PerkIcon = icon;
    }
}
