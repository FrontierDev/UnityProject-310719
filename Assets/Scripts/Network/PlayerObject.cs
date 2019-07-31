using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This allows the rest of the code to think in terms of a general
// object rather than dealing with connections specifically.
public class PlayerObject {
	// Contains the instantiate player character object in the game world.
	public BoltEntity character;			

	// Contains the connection IF A CONNECTION EXISTS. Will be null for the server!
	public BoltConnection connection;

	// The following properties check if the connection is null (or not)
	public bool isServer { get { return connection == null; }}
	public bool isClient { get { return connection != null; }}

	// This spawns the player character and 
	// correctly assigns control of it to the connection
	public void Spawn(CustomisationToken token) {
		if (!character) {
			character = BoltNetwork.Instantiate (BoltPrefabs.Player, token);
        }

		// Server takes control of the char it spawns;
		// client is GIVEN control of the char it spawns.
		if (isServer) {
			BoltConsole.Write ("Server taking control...");
            character.TakeControl ();
		}	 else {
			BoltConsole.Write ("Assigning control to " + connection);
            character.AssignControl (connection);
		}

        // Just gives a random position.
        character.transform.position = RandomPosition ();
	}

	Vector3 RandomPosition() {
		float x = Random.Range (-2f, 2f);
		float z = Random.Range (-2f, 2f);
		return new Vector3 (x, 0f, z);
	}

}
