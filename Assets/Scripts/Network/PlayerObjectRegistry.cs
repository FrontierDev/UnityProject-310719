using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


// This class manages the instances of TutorialPlayerObject.cs that are in the game.
public class PlayerObjectRegistry : MonoBehaviour {
	// Keep a list of all the players
	static List<PlayerObject> players = new List<PlayerObject>();

	// create a player connection
	// note: connection can be null
	static PlayerObject CreatePlayer(BoltConnection connection) {
		PlayerObject p;

		// create a new player object and assign the connection property in it
		p = new PlayerObject();
		p.connection = connection;

		// if there is a connection...
		// the player is assigned as the USER DATA for the connection
		// so that we can associated connection to player
		if (p.connection != null) {
			p.connection.UserData = p;
		}

		// add to list of players
		players.Add(p);

		return p;
	}

	// Returns the player list in a way that cannot be modified from the outside.
	static IEnumerable<PlayerObject> allPlayers {
		get { return players; }
	}

	// find the server player by checking isServer for every player
	public static PlayerObject serverPlayer {
		get { return players.First (x => x.isServer); }
	}

	// utility function which creates a server player or a client player...
	public static PlayerObject CreateServerPlayer() {
		return CreatePlayer (null);
	}

	public static PlayerObject CreateClientPlayer(BoltConnection connection) {
		return CreatePlayer (connection);
	}

	// passes a connection and returns the proper player object.
	// Very important for reloading a character!
	public static PlayerObject GetPlayer(BoltConnection connection) {
		if (connection == null) {
			return serverPlayer;
		}

	return (PlayerObject)connection.UserData;

	}
}
