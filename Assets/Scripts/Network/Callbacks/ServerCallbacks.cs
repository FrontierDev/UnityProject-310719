using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[BoltGlobalBehaviourAttribute(BoltNetworkModes.Host)]
public class ServerCallbacks : Bolt.GlobalEventListener {

	// As soon as this script is awoken on the server, create a server player.
	// This way, even the character loaded on the server must act as a 'client'.
	void Awake() {
		PlayerObjectRegistry.CreateServerPlayer ();
	}
		
	// Creates a client player when connecting to the server.
	public override void Connected(BoltConnection arg) {
		PlayerObjectRegistry.CreateClientPlayer (arg);
		Debug.Log (arg.ConnectionId + " Connected");
	}


	public override void OnEvent(SpawnPlayerEvent evnt) {
		// Spawn the player with this customisation token and assign control to the
		// connection /player who raised the event...
		PlayerObjectRegistry.GetPlayer (evnt.RaisedBy).Spawn ((CustomisationToken)evnt.CustomisationToken);
	}

    public override void OnEvent(SpawnActorEvent evnt)
    {
        //Debug.LogWarning("Actor Spawn raised by " + evnt.RaisedBy);

        if(BoltNetwork.isServer)
            ActorObjectRegistry.CreateServerActor().Spawn(evnt.ActorPrefab, (ActorSpawnToken)evnt.ActorSpawnToken);
    }
}