/*  WEAPON CLASS (ScriptableObject)
 *  To-do list:
 *              - Add skill, faction, quest requirements to weapons.
 */ 

using UnityEngine;
using GameUtilities;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class WeaponStats {
    // Weapon type: either melee, ranged or thrown.
    [SerializeField]
    public WeaponType WeaponType { get; set; }

    // Weapon-specific attributes:
    // BASE DAMAGE
    [SerializeField]
    public int BaseDamage { get; set; }
    // BLUNT damage
    [SerializeField]
    public int BluntDamage { get; set; }
    // PIERCE damage
    [SerializeField]
    public int PierceDamage { get; set; }
    // SLASH damage
    [SerializeField]
    public int SlashDamage { get; set; }
    // ATTACK SPEED: delay in seconds between attacks.
    [SerializeField]
    public int AttackSpeed { get; set; }

    // Conditions *BY ID* which this weapon is equipped.
    [SerializeField]
    public List<int> Auras { get; set; }

    /*
     *  TO BE ADDED
     *  Skill, faction, quest requirement(s) 
     */


    // Empty constructor; will invariably cause problems.
    public WeaponStats() {
        Auras = new List<int>();
    }
}
