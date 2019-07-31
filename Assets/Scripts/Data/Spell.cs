/*
 * CURRENTLY HAS NO FUNCTIONALITY!!
 */ 

using UnityEngine;
using GameUtilities;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Spell {
	// Aura name
	public string SpellName { get; set; }
	// Aura ID
	public int SpellID { get; set; }
	// Aura description
	public string SpellDesc { get; set; }
	// Cooldown
	public double SpellCooldown { get; set; }
	// Cast tme
	public double SpellCastTime { get; set; }

	// Skill icon to show in the character window.
	private Texture2D SpellIcon { get; set; }
	[SerializeField]
	public string SpellIconPath { get; set; }
	// The level at which this spell is unlocked (DEFAULT).

	// The cast type of the spell
	public SpellCastType SpellCastType { get; set; }

	// The IDs of the auras that it applies to the CASTER
	[SerializeField]
	public List<int> CasterAuras = new List<int>();
	// The IDs of the auras that it applies to the TARGET(s)
	[SerializeField]
	public List<int> TargetAuras = new List<int>();


	// Constructor used for duplication
	public Spell(Spell spell) {
		this.SpellName = spell.SpellName;
		this.SpellID = spell.SpellID;
		this.SpellDesc = spell.SpellDesc;
		this.SpellCastTime = spell.SpellCastTime;
		this.SpellCooldown = spell.SpellCooldown;
		this.SpellIcon = spell.SpellIcon;
		this.SpellIconPath = spell.SpellIconPath;
		this.SpellCastType = spell.SpellCastType;
		this.CasterAuras = spell.CasterAuras;
		this.TargetAuras = spell.TargetAuras;
	}

	// Empty constructor
	public Spell() {

	}
		
	public void LoadIcon() {
		SpellIconPath = GameUtility.CleanItemResourcePath(SpellIconPath, "Assets/Resources/");
		SpellIconPath = GameUtility.CleanItemResourcePath(SpellIconPath, ".png");

		SpellIcon = (Texture2D)Resources.Load(SpellIconPath);
	}

	public Texture2D GetIcon() {
		return SpellIcon;
	}

	public void SetIcon(Texture2D icon) {
		SpellIcon = icon;
	}
}
