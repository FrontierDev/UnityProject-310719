using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles all the player's Skills and their updates on the server.
/// </summary>
public class PlayerSkillset : Bolt.EntityEventListener<IPlayerState>
{
    public List<PlayerSkill> PlayerSkills { get; set; }

    string skillToIncrease = "";
    int newSkillLevel = -1;

    void Awake() {
        PlayerSkills = new List<PlayerSkill>();

        GameDatabase db = GameObject.FindGameObjectWithTag("Database").GetComponent<GameDatabase>();
        List<Skill> skills = db.skilldb.Skills;

        for (int i = 0; i < skills.Count; i++)
        {
            PlayerSkill newSkill = new PlayerSkill(skills[i]);
            newSkill.SetLevel(1);
            PlayerSkills.Add(newSkill);
            
            //Debug.LogWarning(newSkill.GetSkill().SkillName + " added to skillset.");
        }
    }

    void Start() {

    }

    public override void SimulateController() {
        if (skillToIncrease != "" && newSkillLevel != -1)
        {
            var evnt = IncreaseSkillEvent.Create (entity);
            evnt.SkillName = skillToIncrease;
            evnt.SkillLevel = newSkillLevel;
            evnt.Send ();

            skillToIncrease = "";
            newSkillLevel = -1;
        }
    }

    public void IncreaseSkill(string _skill, int _val) {
        PlayerSkill target = PlayerSkills.Find(skill => skill.GetSkill().SkillName.Equals(_skill));
        target.IncreaseLevel(_val);

        skillToIncrease = _skill;
        newSkillLevel = target.GetLevel();
    }

    /*
     * EVENTS
     */
    public override void OnEvent(IncreaseSkillEvent evnt) {
        for (int i = 0; i < state.BoltPlayerSkillset.Skillset.Length; i++)
        {
            if (state.BoltPlayerSkillset.Skillset[i].SkillName == evnt.SkillName)
            {
                state.BoltPlayerSkillset.Skillset[i].SkillLevel = evnt.SkillLevel;
            }
        }
    }
}
