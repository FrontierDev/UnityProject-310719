using UnityEngine;
using GameUtilities;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Faction {
    // The faction name.
    [SerializeField]
    public string FactionName { get; set; }

    [SerializeField]
    public int FactionID { get; set; }

    // The faction's leader.
    [SerializeField]
    public string FactionLeader { get; set; }

    [SerializeField]
    public string FactionIconPath { get; set; }
    private Texture2D FactionIcon { get; set; }

    [SerializeField]
    public string FactionDescPath { get; set; } // A text file containing the item description.

    // The list of races belonging to that faction.
    [SerializeField]
    public List<string> FactionRaceList = new List<string>();

    // Empty constructor.
    public Faction() {

    }

    public void LoadIcon() {
        FactionIconPath = GameUtility.CleanItemResourcePath(FactionIconPath, "Assets/Resources/");
        FactionIconPath = GameUtility.CleanItemResourcePath(FactionIconPath, ".png");

        FactionIcon = (Texture2D)Resources.Load(FactionIconPath);
    }

    public Texture2D GetIcon() {
        return FactionIcon;
    }

	public Sprite GetIconAsSprite() {
		return Sprite.Create (FactionIcon, new Rect (0.0f, 0.0f, FactionIcon.width, FactionIcon.height), new Vector2 (0.5f, 0.5f));
	}

    public void SetIcon(Texture2D icon) {
        FactionIcon = icon;
    }
}
