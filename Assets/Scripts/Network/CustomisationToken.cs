using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used when creating, saving and loading player characters. 
/// The initial faction and the actor's race never change throughout the course of the game.
/// </summary>
public class CustomisationToken : Bolt.IProtocolToken {
	public int actorFactionID;
	public string actorRaceName;

	public void Write(UdpKit.UdpPacket packet) {
		packet.WriteInt (actorFactionID);
		packet.WriteString (actorRaceName);

	}

	public void Read(UdpKit.UdpPacket packet) {
		actorFactionID = packet.ReadInt ();
		actorRaceName = packet.ReadString ();
	}
}
