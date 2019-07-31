using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

// NYI: Hunger /Thirst /Heat.

public class ActorStats : Bolt.EntityEventListener<IPlayerState>
{
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int HealthRegen { get; set; }
    public int Energy { get; set; }
    public int MaxEnergy { get; set; }
    public int EnergyRegen { get; set; }
    public int Thirst { get; set; }
    public int MaxThirst { get; set; }
    public int ThirstRegen { get; set; }
    public int Hunger { get; set; }
    public int MaxHunger { get; set; }
    public int HungerRegen { get; set; }
    public int Heat { get; set; }
    public int HeatRegen { get; set; }
    public int MeleeDamage { get; set; }
    public int RangedDamage { get; set; }
    public int MeleeSpeed { get; set; }
    public int RangedSpeed { get; set; }
    public int MovementSpeed { get; set; }

    bool isRegeneratingHealth = false;
    bool isRegeneratingEnergy = false;

	public string actorRaceName = "";
	public int actorStartingFaction = -1;
    public List<ActorAura> actorAuras = new List<ActorAura>();
    bool isPlayer;

    List<Perk> perks = new List<Perk>();
    PlayerSkillset skillset;
    GameDatabase db;

    void Awake() {
        db = GameObject.FindGameObjectWithTag("Database").GetComponent<GameDatabase>();
        skillset = GetComponent<PlayerSkillset>();
        isPlayer = false;
    }

    public override void Attached() {
        SetStat("Health", 1);
        SetStat("Energy", 2);
        SetStat("MaxHealth", 11);
        SetStat("MaxEnergy", 9);
        SetStat("HealthRegen", 1);
        SetStat("EnergyRegen", 2); 
    }

    public override void SimulateOwner() {

        // Only regenerate a stat if it can be regenerated!
        if (!isRegeneratingHealth && Health < MaxHealth)
            StartCoroutine(RegenerateHealth());

        if (!isRegeneratingEnergy && Energy < MaxEnergy)
            StartCoroutine(RegenerateEnergy());
    }

#region Setup
    public void LoadCustomisation(CustomisationToken customisation) {
        actorStartingFaction = customisation.actorFactionID;
        actorRaceName = customisation.actorRaceName;
        isPlayer = true;
    }

    public bool IsPlayer() {
        return isPlayer;
    }

    public void LoadInterface() {
        GameObject ui = GameObject.FindGameObjectWithTag("User Interface");
        if (ui.activeInHierarchy == false)
            ui.SetActive(true);

        GameObject ui_UnitPlayer = ui.transform.Find("Unit Player Panel").gameObject;
        ui_UnitPlayer.GetComponent<UnitPlayerInterface>().SetActorStats(this);
    }
    #endregion

#region Perks
    /// <summary>
    /// Adds a perk to the actor according to the perk's ID no.
    /// </summary>
    /// <param name="perkID"></param>    
    public void AddPerk(int perkID) {
        //Debug.LogWarning(db.perkdb);
        //Debug.LogWarning(db.perkdb.perk(perkID));

        Perk perk = db.perkdb.perk(perkID);
        perks.Add(perk);
        AddActorAuras(perk.PerkAuras);

        //Debug.LogWarning("Added perk: " + db.perkdb.perk(perkID).PerkName);
    }
    #endregion

    #region Auras
    /// <summary>
    /// Adds an aura to the actor according to the aura's ID no.
    /// </summary>
    /// <param name="auraID"></param>
    public void AddActorAura(int auraID) {
        Aura aura = db.conddb.Aura(auraID);
        ActorAura newActorAura = new ActorAura(aura);

        actorAuras.Add(newActorAura);
        newActorAura.ApplyActorAura(this);

        // Start the aura's timer if it exists.
        if (aura.HasDuration)
            StartCoroutine(AuraDurationTimer(aura.AuraDuration,newActorAura));
    }

    /// <summary>
    /// Adds a list of auras (by ID) to the actor, for instance the entire PerkAuras property of a Perk.
    /// </summary>
    /// <param name="auraList"></param>
    public void AddActorAuras(List<int> auraList) {
        for(int i = 0; i < auraList.Count; i++)
            AddActorAura(auraList[i]);
    }
#endregion

#region Modify Stat and Skill etc.
    /// <summary>
    /// Adds or subtracts a value from a given stat belonging to the actor. This triggers StatChangedEvent on the server.
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_val"></param>
    public void ModStat(string _name, int _val) {
        if(entity.isOwner)
        {
            //PropertyInfo property = GetType().GetProperty(_name);
            Debug.Log(_name + " modified by " + _val);

            //Get the actor's stat set. Then create a Bolt Event with the name of the stat to be change and the value to change it by.
            // Will catch NulLReferenceException if an invalid _name is given.
            try
            {
                //int oldValue = (System.Int32)property.GetValue(this, null);
                //property.SetValue(this, oldValue + _val, null);
                Bolt.NetworkArray_Objects<BoltActorStat> statset = state.BoltActorStatset.Statset;

                IEnumerable<BoltActorStat> query = from stat in statset
                                                   where stat.StatName == _name
                                                   select stat;

                query.First().StatValue += _val;

                StatChangedEvent evnt = StatChangedEvent.Create(entity);
                evnt.StatName = _name;
                evnt.StatMod = _val;
                evnt.Send();
            }
            catch (System.NullReferenceException nre)
            {
                Debug.LogError(nre);
            }
        }
    }

    /// <summary>
    /// Sets a given stat belonging to the actor to a specific value. This triggers StatChangedEvent on the server.
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_val"></param>
    public void SetStat(string _name, int _val) {
        if (entity.isOwner)
        {
            //PropertyInfo property = GetType().GetProperty(_name);
            Debug.Log(_name + " set to " + _val);

            // Get the actor's stat set. Then create a Bolt Event with the name of the stat to be change and the value to change it by.
            // Will catch NulLReferenceException if an invalid _name is given.
            try
            {
                Bolt.NetworkArray_Objects<BoltActorStat> statset = state.BoltActorStatset.Statset;

                IEnumerable<BoltActorStat> query = from stat in statset
                                                   where stat.StatName == _name
                                                   select stat;

                query.First().StatValue += _val;

                StatChangedEvent evnt = StatChangedEvent.Create(entity);
                evnt.StatName = _name;
                evnt.StatMod = _val;
                evnt.Send();
            }
            catch (System.NullReferenceException nre)
            {
                Debug.LogError(nre);
            }
        }
    }

    /// <summary>
    /// Increases a skill by a specific amount. **NYI: SkillChangedEvent.**
    /// </summary>
    /// <param name="_skill"></param>
    /// <param name="_val"></param>
    public void ModSkill(string _skill, int _val) {
        if (db.skilldb.Contains(_skill))
        {
            //Debug.LogWarning("Increasing skill...");
            skillset.IncreaseSkill(_skill, _val);
        }
        else
        {
            Debug.LogWarning("Skill not found");
        }
    }

    /*
     * This needs to be reworked when we want to see the
     * time remaining on the UI, but it works as of
     * 3rd Sept 2018.
     */
    IEnumerator AuraDurationTimer(double _dur, ActorAura aura)
    {
        yield return new WaitForSeconds((float)_dur);
        RemoveActorAura(aura);
    }

    public void RemoveActorAura(ActorAura _actorAura) {
        actorAuras.Remove(_actorAura);
        _actorAura.RemoveActorAura(this);
    }
    #endregion

#region Regeneration
    bool HasFullHealthAndEnergy() {
        if (Energy == MaxEnergy && Health == MaxHealth)
            return true;
        else
            return false;
    }

    IEnumerator RegenerateEnergy() {
        if (!isRegeneratingEnergy)
            isRegeneratingEnergy = true;

        yield return new WaitForSeconds(5f);
        ModStat("Energy", EnergyRegen);

        isRegeneratingEnergy = false;
    }

    IEnumerator RegenerateHealth() {
        if (!isRegeneratingHealth)
            isRegeneratingHealth = true;

        yield return new WaitForSeconds(5f);
        ModStat("Health", HealthRegen);

        isRegeneratingHealth = false;
    } 
    #endregion

    /*
     * NETWORK EVENTS
     */
    public override void OnEvent(StatChangedEvent evnt) {
        Debug.LogWarning("Stat changed event called.");

    }
}
