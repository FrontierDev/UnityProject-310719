using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryToken : Bolt.IProtocolToken {
	public int itemID;
	public int itemQuantity;

	public void Write(UdpKit.UdpPacket packet) {
		packet.WriteInt (itemID);
		packet.WriteInt (itemQuantity);
	}

	public void Read(UdpKit.UdpPacket packet) {
		itemID = packet.ReadInt ();
		itemQuantity = packet.ReadInt ();
	}
}
