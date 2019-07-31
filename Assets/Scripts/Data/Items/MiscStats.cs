/*  MISC CLASS
 *  To-do list:
 *              - Add skill, faction, quest requirements to weapons.
 */

using UnityEngine;
using GameUtilities;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MiscStats {
    // Ingredient-specific attributes:
    // Is the ingredient stackable?
    [SerializeField]
    public bool IsStackable { get; set; }


    /*
     *  TO BE ADDED
     *  Skill, faction, quest requirement(s) 
     */


    // Empty constructor; will invariably cause problems.
    public MiscStats() {

    }
}
