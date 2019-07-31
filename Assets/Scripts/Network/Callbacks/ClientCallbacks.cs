using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[BoltGlobalBehaviour]
public class ClientCallbacks : Bolt.GlobalEventListener {

    /// <summary>
    /// Called when control of an entity is gained.
    /// If that entity is a player, the camera and GUI will be configured to follow and provide details (respectively) of that player character.
    /// </summary>
    /// <param name="entity"></param>
    public override void ControlOfEntityGained(BoltEntity entity)
    {
        if(entity.prefabId == BoltPrefabs.Player)
        {
            Debug.Log("Control of " + entity.prefabId.Value + " gained.");
            MainCameraController.instance.Configure(entity.transform);
            UnitPlayerInterface.instance.Configure(entity.gameObject.GetComponent<ActorStats>());
        }
    }
}
