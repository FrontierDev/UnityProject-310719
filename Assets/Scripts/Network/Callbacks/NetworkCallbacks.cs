using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[BoltGlobalBehaviourAttribute]
public class NetworkCallbacks : Bolt.GlobalEventListener {

    /// <summary>
    /// Called when Bolt begins. All tokens used must be registered here.
    /// </summary>
    public override void BoltStartBegin() {
		BoltNetwork.RegisterTokenClass<CustomisationToken> ();
        BoltNetwork.RegisterTokenClass<ActorSpawnToken>();
	}

    /// <summary>
    /// Create the server (if in server mode) or connect to it via IP address (if in client mode). 
    /// Loads the Character Creator and main game world additively.
    /// </summary>
	public override void BoltStartDone() {
		if (BoltNetwork.isServer) {
			UnityEngine.SceneManagement.SceneManager.LoadScene ("CharacterCreator");
			UnityEngine.SceneManagement.SceneManager.LoadScene ("Overworld", UnityEngine.SceneManagement.LoadSceneMode.Additive);
		} else {
			BoltNetwork.Connect (UdpKit.UdpEndPoint.Parse ("127.0.0.1:27000")); // This would realistically connect to the external server via its IP address.

			UnityEngine.SceneManagement.SceneManager.LoadScene ("CharacterCreator");
			UnityEngine.SceneManagement.SceneManager.LoadScene ("Overworld", UnityEngine.SceneManagement.LoadSceneMode.Additive);
		}
	}
}