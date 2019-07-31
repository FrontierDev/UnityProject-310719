using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[BoltGlobalBehaviourAttribute("CharacterCreation")]
public class CCFinish : Bolt.GlobalEventListener {
	public CharacterCreatorManager manager;
	CustomisationToken token = new CustomisationToken();

	void Awake () {
		DontDestroyOnLoad (this.gameObject);
	}

    /// <summary>
    /// // Copy over all the data from character creator into the newly made character.
    /// </summary>
    public void FinishCharacterCreation() {
		token.actorFactionID = manager.selectedFaction.FactionID;
		token.actorRaceName = manager.selectedRace.RaceName;

		var evnt = SpawnPlayerEvent.Create ();
		evnt.PlayerPrefab = BoltPrefabs.Player;
		evnt.CustomisationToken = token;

        Debug.Log("Spawning Player.");
        evnt.Send ();

		UnityEngine.SceneManagement.SceneManager.LoadScene ("Overworld");
	}
}
