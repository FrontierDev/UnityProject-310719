using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


/// <summary>
/// This class manages the instances of ActorObject.cs that are in the game.
/// </summary>
public class ActorObjectRegistry : MonoBehaviour
{
    // Keep a list of all the players
    static List<ActorObject> actors = new List<ActorObject>();

    // create a player connection
    // note: connection can be null
    static ActorObject CreateActor(BoltConnection connection)
    {
        ActorObject p;

        // create a new player object and assign the connection property in it
        p = new ActorObject();
        p.connection = connection;

        // if there is a connection...
        // the player is assigned as the USER DATA for the connection
        // so that we can associated connection to player
        if (p.connection != null)
        {
            p.connection.UserData = p;
        }

        // add to list of players
        actors.Add(p);

        return p;
    }

    /// <summary>
    /// Returns the player list in a way that cannot be modified externally.
    /// </summary>
    public static IEnumerable<ActorObject> allActors {
        get { return actors; }
    }

    /// <summary>
    /// Find the server player by checking isServer for every player.
    /// </summary>
    public static ActorObject serverActor {
        get { return actors.First(x => x.isServer); }
    }

    /// <summary>
    /// Utility function which creates a server player or a client player...
    /// </summary>
    /// <returns></returns>
    public static ActorObject CreateServerActor()
    {
        Debug.Log("Creating server actor.");
        return CreateActor(null);
    }

    public static ActorObject CreateClientActor(BoltConnection connection)
    {
        return CreateActor(connection);
    }

    // passes a connection and returns the proper player object.
    // Very important for reloading a character!
    public static ActorObject GetActor(BoltConnection connection)
    {
        if (connection == null)
        {
            return serverActor;
        }

        return serverActor;
    }
}
