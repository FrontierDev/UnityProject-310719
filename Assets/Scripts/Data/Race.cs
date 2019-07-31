using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Race {
    public string RaceName { get; set; }
    public string RaceDescPath { get; set; }
    public string RaceIconPath { get; set; }
    public List<int> RacePerks { get; set; }

    public Race() {
        RacePerks = new List<int>();
    }
}
