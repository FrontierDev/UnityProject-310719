using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[BoltGlobalBehaviourAttribute("BoltTestMenu")]
public class StartCreator : Bolt.GlobalEventListener {
	public void Launch() {
		BoltLauncher.StartClient ();
	}
}
