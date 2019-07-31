using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Visually displays the player's health, energy etc. NYI: Heat, hunger, thirst.
/// </summary>
public class UnitPlayerInterface : MonoBehaviour {
    public static UnitPlayerInterface instance;

    ActorStats actorStats;

    public GameObject healthBar;
    public GameObject energyBar;

    void Awake() {
        instance = this;
    }

    public void Configure(ActorStats _stats) {
        actorStats = _stats;

        LoadHealthBarText();
        LoadEnergyBarText();
    }

    public void SetActorStats(ActorStats _stats) {
        actorStats = _stats;

        LoadHealthBarText();
        LoadEnergyBarText();
    }

    public void UpdateBars() {
        LoadHealthBarText();
        LoadEnergyBarText();
    }

    public void LoadHealthBarText() {
        TMPro.TextMeshProUGUI barText = healthBar.GetComponentInChildren<TMPro.TextMeshProUGUI>();

        try
        {
            barText.text = string.Format("{0}/{1}", actorStats.Health, actorStats.MaxHealth);
        }
        catch (System.NullReferenceException e)
        {
            Debug.LogWarning(e.Message);
            barText.text = "Stats not found.";
        }
    }

    void LoadEnergyBarText() {
        TMPro.TextMeshProUGUI barText = energyBar.GetComponentInChildren<TMPro.TextMeshProUGUI>();

        try
        {
            barText.text = string.Format("{0}/{1}", actorStats.Energy, actorStats.MaxEnergy);
        }
        catch (System.NullReferenceException e)
        {
            Debug.LogWarning(e.Message);
            barText.text = "Stats not found.";
        }
    }
}
