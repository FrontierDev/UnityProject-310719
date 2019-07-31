using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameUtilities {
	public static class SpellUtility {

	}

    /// <summary>
    /// Types of spells in the game.
    /// </summary>
	public enum SpellCastType {
		Melee, 
		Aimed, 
		Target_Single, Target_Area, 
		Caster_Self, Caster_Area
	}
}