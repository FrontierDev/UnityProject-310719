using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Bolt;

public class ActorSpawner : MonoBehaviour
{
    public GameObject actor;
    bool hasSpawned = false;

    void Awake() {
        if(!hasSpawned && BoltNetwork.isServer && SceneManager.GetActiveScene().name.Equals("Overworld"))
        {
            SpawnActor();
            hasSpawned = true; 
        }
    }

    // Actor ID -> which NPC to load from the database.
    void SpawnActor()
    {
        ActorSpawnToken token = new ActorSpawnToken();

        token.actorID = actor.GetComponent<ActorNPC>().npcID;
        token.actorPos = transform.position;

        var evnt = SpawnActorEvent.Create();

        evnt.ActorSpawnToken = token;
        evnt.ActorPrefab = actor.GetComponent<BoltEntity>().prefabId;

        Debug.Log("Spawning Actor " + actor.name);
        evnt.Send();
    }
}
