using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtilities;

/// <summary>
/// The "Aura" class is simply a template for an aura's attributes that exists on the database.
/// These templates are not used directly. Instead, an ActorAura stores the reference for that 
/// aura so that its attributes can be read, as well as how much time is remaining on the aura.
/// </summary>
public class ActorAura {
    Aura Aura { get; set; }
    ActorStats actorStats;

    double TimeRemaining { get; set; }

    public ActorAura (Aura _aura)
    {
        Aura = _aura;
    }

    public Aura GetAura()
    {
        return Aura;
    }

    public void ApplyActorAura(ActorStats _actorStats){
        actorStats = _actorStats;
        AuraStat stat = Aura.AuraStat;
        int val = Aura.AuraValue;

        Debug.Log("Applying aura " + Aura.AuraName + " to " + actorStats.gameObject.name);

        if (stat == AuraStat.Skill)
            actorStats.ModSkill(Aura.AuraSkill, val);
        else
            actorStats.ModStat(stat.ToString(), val);
    }

    public void RemoveActorAura(ActorStats _actorStats)
    {
        actorStats = _actorStats;
        AuraStat stat = Aura.AuraStat;
        int val = Aura.AuraValue;

        if (stat == AuraStat.Skill)
            actorStats.ModSkill(Aura.AuraSkill, -val);
        else
            actorStats.ModStat(stat.ToString(), -val);

        Debug.Log("Removing aura " + Aura.AuraName + " to " + actorStats.gameObject.name);
    }
}