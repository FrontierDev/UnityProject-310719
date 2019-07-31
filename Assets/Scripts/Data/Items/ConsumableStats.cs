/*  CONSUMABLE CLASS
 *  To-do list:
 *              - Add skill, faction, quest requirements to weapons.
 */

using UnityEngine;
using GameUtilities;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ConsumableStats {
    // Consumable type: food, drink, medical supply.
    [SerializeField]
    public ConsumableType ConsumableType { get; set; }

    // Auras *BY ID* which this consumable applies.
    [SerializeField]
    public List<int> Auras { get; set; }

    // Maximum number of times this consumable can be used
    [SerializeField]
    public int Charges { get; set; }

    /*
     *  TO BE ADDED
     *  Skill, faction, quest requirement(s) 
     */


    // Empty constructor; will invariably cause problems.
    public ConsumableStats() {
        Auras = new List<int>();
    }
}
