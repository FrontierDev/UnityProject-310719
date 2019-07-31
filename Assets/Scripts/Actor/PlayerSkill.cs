using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The "Skill" class is simply a template for an skill's attributes/perks etc that exists on the database.
/// These templates are not used directly. Instead, an PlayerSkill stores the reference for that 
/// skill so that its attributes can be read, as well as what level that skill is currently at.
/// </summary>
public class PlayerSkill {
    Skill Skill { get; set; }
    int Level { get; set; }

    public PlayerSkill(Skill _skill) {
        Skill = _skill;

        Level = 1;
    }

    public Skill GetSkill() {
        return Skill;
    }

    public int GetLevel() {
        return Level;
    }

    public void IncreaseLevel(int _val) {
        Level += _val;
    }

    public void SetLevel(int _val) {
        Level = _val;
    }
}
