using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used for spawning NON-PLAYER CHARACTERS.
/// The token sends the actor's ID number and it's position, which is given by the transform of a game object.
/// </summary>
public class ActorSpawnToken : Bolt.IProtocolToken
{
    public int actorID;
    //public int actorPrefab;
    public Vector3 actorPos;

    public void Write(UdpKit.UdpPacket packet)
    {
        packet.WriteInt(actorID);
        //packet.WriteInt(actorPrefab);
        packet.WriteVector3(actorPos);
    }

    public void Read(UdpKit.UdpPacket packet)
    {
        actorID = packet.ReadInt();
        //actorPrefab = packet.ReadInt();
        actorPos = packet.ReadVector3();
    }
}
