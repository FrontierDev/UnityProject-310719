using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class CCFactionInfoController : MonoBehaviour {
    public Image icon;
    public Text header;
    public Text leader;
    public Text description;

	public GameObject[] factionIcons;
	GameObject activeIcon;

	void Start() {
		
	}

    /// <summary>
    /// Updates the text and icon etc. for the selected faction.
    /// </summary>
    /// <param name="faction"></param>
    /// <param name="newActiveIcon"></param>
	public void UpdateInfo(Faction faction, GameObject newActiveIcon) {
		icon.sprite = faction.GetIconAsSprite ();
        header.text = faction.FactionName;
        leader.text = faction.FactionLeader;
		description.text = File.ReadAllText(Application.dataPath + "/StreamingAssets" + faction.FactionDescPath);


		activeIcon = newActiveIcon;
		activeIcon.GetComponent<Image> ().canvasRenderer.SetAlpha (2.0f);
		DisableInactiveIcons ();
    }

    /// <summary>
    /// Fades out the icons for the factions which are not selected.
    /// </summary>
	void DisableInactiveIcons () {
		for (int i = 0; i < factionIcons.Length; i++) {
			if (!factionIcons [i].Equals (activeIcon))
				factionIcons [i].GetComponent<Image> ().canvasRenderer.SetAlpha (0.5f);
		}
	}
}
