using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorNetworkBehaviour : Bolt.EntityEventListener<INPCState> {

    ActorNPC actorNPC;
    ActorStats actorStats;

    GameDatabase db;

    public override void Attached()
    {
        actorNPC = GetComponent<ActorNPC>();
        actorStats = GetComponent<ActorStats>();
    }
}
