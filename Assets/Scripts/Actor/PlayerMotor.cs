using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component handles player movement in the game.
/// </summary>
public class PlayerMotor : MonoBehaviour
{
    // Change this later down the line when speed is a player stat.
    public int movementSpeed = 1;
    public Rigidbody rb;


    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    public void SetState(Vector3 position, Vector3 rotation, Vector3 velocity, bool isGrounded, int jumpFrames)
    {
        transform.position = position;
    }

    // TO DO: Change this to forces on rigid body at some point
    public State Move(bool forward, bool backward, bool leftStrafe, bool rightStrafe, bool jump, float yaw, float pitch)
    {
        State newState = new State();

        var movement = Vector3.zero;

        if (forward)
            movement += transform.forward;

        if (backward)
            movement -= transform.forward;

        if (leftStrafe)
            movement -= transform.right;

        if (rightStrafe)
            movement += transform.right;

        if (movement != Vector3.zero)
        {
            transform.localPosition = transform.localPosition + (movement.normalized * movementSpeed * BoltNetwork.frameDeltaTime);
            newState.position = transform.localPosition;
        }

        // Works fine, will need updating when proper camera controls added.
        if (yaw != 0)
        {
            transform.eulerAngles = new Vector2(0, yaw);
            newState.rotation = transform.rotation;
        }
        return newState;
    }

    public struct State
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 velocity;
        public bool isGrounded;
        public int jumpFrames;
    }

}
