using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[BoltGlobalBehaviourAttribute("BoltTestMenu")]
public class StartServer : Bolt.GlobalEventListener {
	public void Launch() {
		BoltLauncher.StartServer(UdpKit.UdpEndPoint.Parse("127.0.0.1:27000"));
	}
}
