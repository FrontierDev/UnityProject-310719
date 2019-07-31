using UnityEngine;
using GameUtilities;
using System.Collections;

[System.Serializable]
public class Aura {
    // Aura name
    public string AuraName { get; set; }
    // Aura ID
    public int AuraID { get; set; }
    // Aura description
    public string AuraDesc { get; set; }
    // Harmful flag?
    public bool IsHarmful { get; set; }
    // Which actor stat does the condition effect?
    public AuraStat AuraStat { get; set; }
    public int AuraValue { get; set; }
    // Duration, if any.
    public bool HasDuration { get; set; }
    public double AuraDuration { get; set; }
    // Skill to be modified, if any.
    public string AuraSkill { get; set; }

    // Constructor used for duplication
	public Aura(Aura aura) {
		this.AuraName = aura.AuraName;
		this.AuraID = aura.AuraID;
		this.AuraDesc = aura.AuraDesc;
		this.IsHarmful = aura.IsHarmful;
		this.AuraStat = aura.AuraStat;
		this.AuraValue = aura.AuraValue;
		this.AuraDuration = aura.AuraDuration;
        this.AuraSkill = aura.AuraSkill;
    }

    // Empty constructor
	public Aura() {

    }
}
