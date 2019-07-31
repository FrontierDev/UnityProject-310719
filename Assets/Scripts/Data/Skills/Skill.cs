using UnityEngine;
using GameUtilities;
using System.Collections;
using System.Collections.Generic;

public class Skill {
    // Skill name
    [SerializeField]
    public string SkillName { get; set; }
    // Skill unique id
    [SerializeField]
    public int SkillID { get; set; }
    // Skill descriptions. The short description is shown on a tooltip.
    // The long description is shown in the skill window.
    [SerializeField]
    public string SkillShortDesc { get; set; }
    [SerializeField]
    public string SkillLongDesc { get; set; }
    // Skill icon to show in the character window.
    private Texture2D SkillIcon { get; set; }
    [SerializeField]
    public string SkillIconPath { get; set; }

    // Assigned perks.
    [SerializeField]
    public List<int> perkIDs = new List<int>();

    public Skill() {
    }

    public void LoadIcon() {
        SkillIconPath = GameUtility.CleanItemResourcePath(SkillIconPath, "Assets/Resources/");
        SkillIconPath = GameUtility.CleanItemResourcePath(SkillIconPath, ".png");

        SkillIcon = (Texture2D)Resources.Load(SkillIconPath);
    }

    public Texture2D GetIcon() {
        return SkillIcon;
    }

    public void SetIcon(Texture2D icon) {
        SkillIcon = icon;
    }
}
