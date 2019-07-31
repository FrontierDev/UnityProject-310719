/*  WEAPON CLASS (ScriptableObject)
 *  To-do list:
 *              - Add skill, faction, quest requirements to weapons.
 */

using UnityEngine;
using GameUtilities;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ContainerStats {
    // Container-specific attributes:
    // CONTENTS.
    [SerializeField]
    public List<int> ContentItems = new List<int>();
    [SerializeField]
    public List<int> ContentQuantities = new List<int>();

    private Dictionary<int, int> Contents = new Dictionary<int, int>();


    /*
     *  TO BE ADDED
     *  Skill, faction, quest requirement(s) 
     */


    // Empty constructor; will invariably cause problems.
    public ContainerStats() {
        Contents = new Dictionary<int, int>();
    }

    public void Open() {
        Debug.LogWarning("Container Open() NYI");
    }

    public void AddItem(int itemID, int itemQuantity) {
        Contents.Add(itemID, itemQuantity);
    }

    public Dictionary<int, int> GetContents() {
        return Contents;
    }

    public void CombineContents() {
        // Refresh the dictionary; compile from scratch.
        Contents = new Dictionary<int, int>();

        for (int i = 0; i < ContentItems.Count; i++)
        {
            AddItem(ContentItems[i], ContentQuantities[i]);
        }

        Debug.Log("Successfully combined contents of the container.");
    }
}
