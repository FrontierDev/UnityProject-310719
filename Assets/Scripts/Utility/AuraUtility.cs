using UnityEngine;
using System.Collections;

namespace GameUtilities {
    public static class AuraUtility {

    }

    /// <summary>
    /// Actor Stats in the game that an Aura may modify.
    /// </summary>
    public enum AuraStat {
        Health, MaxHealth, HealthRegen,
        Energy, MaxEnergy, EnergyRegen,
        Thirst, MaxThirst,ThirstRegen,
        Hunger, MaxHunger, HungerRegen,
        Heat, HeatRegen,
        MeleeDamage, RangedDamage, MeleeSpeed, RangedSpeed,
        MovementSpeed,
        Skill
    }
}
