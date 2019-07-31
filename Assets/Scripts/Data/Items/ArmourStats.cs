using UnityEngine;
using GameUtilities;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ArmourStats {
    // Armour type: the slot which the armour goes in.
    [SerializeField]
    public ArmourType ArmourType { get; set; }
    // Armour material: the material that the armour is made of.
    [SerializeField]
    public ArmourMaterial ArmourMaterial { get; set; }

    // Armour-specific attributes:
    // BASE DEFENCE
    [SerializeField]
    public int BaseDefence { get; set; }
    // BLUNT defence
    [SerializeField]
    public int BluntDefence { get; set; }
    // PIERCE defence
    [SerializeField]
    public int PierceDefence { get; set; }
    // SLASH defence
    [SerializeField]
    public int SlashDefence { get; set; }
    // NATURE defence
    [SerializeField]
    public int NatureDefence { get; set; }
    // THERMAL defence
    [SerializeField]
    public int ThermalDefence { get; set; }

    // Auras *BY ID* which this armour is equipped.
    [SerializeField]
    public List<int> Auras { get; set; }


    /*
     *  TO BE ADDED
     *  Skill, faction, quest requirement(s) 
     */


    // Empty constructor; will invariably cause problems.
    public ArmourStats() {
        Auras = new List<int>();
    }
}
