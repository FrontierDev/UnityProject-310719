using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorObject {
    // Contains the instantiate player character object in the game world.
    public BoltEntity actor;

    // Contains the connection IF A CONNECTION EXISTS. Will be null for the server!
    public BoltConnection connection;

    // The following properties check if the connection is null (or not)
    public bool isServer { get { return connection == null; } }
    public bool isClient { get { return connection != null; } }

    /// <summary>
    /// Spawns a player prefab with the given actor token.
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="token"></param>
    public void Spawn(Bolt.PrefabId prefab, ActorSpawnToken token)
    {
        if (!actor)
        {
            //actor = BoltNetwork.Instantiate(BoltPrefabs.Player, token);
            //actor = BoltNetwork.Instantiate(token.actorPrefab, token, token.actorPos, Quaternion.identity);
            actor = BoltNetwork.Instantiate(prefab, token, token.actorPos, Quaternion.identity);
        }

        // Server takes control of the char it spawns;
        // client is GIVEN control of the char it spawns.
        if (isServer)
        {
            Debug.Log("Server taking control of " + actor.prefabId);
            actor.TakeControl();
        }
        else
        {
            BoltConsole.Write("Server has NOT taken control of the actor.");
        }
    }

}
