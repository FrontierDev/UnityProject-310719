using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component handles input for the PC.
/// </summary>
public class PlayerController : Bolt.EntityBehaviour<IPlayerState> {
    const float MOUSE_SENSITIVITY = 2f;

    bool _forward;
    bool _backward;
    bool _leftStrafe;
    bool _rightStrafe;
    bool _jump;

    float _yaw;
    float _pitch;

    PlayerMotor _motor;

    void Awake()
    {
        _motor = GetComponent<PlayerMotor>();
    }

    void Update()
    {
        PollKeys(true);

        if(Input.GetKeyDown(KeyCode.E))
        {
            GameObject.FindGameObjectWithTag("Actor").GetComponent<ActorNPC>().Interact();
        }
    }

    // Only called on the PC that owns this Entity.
    // We do NOT want to read mouse data; this would double our mouse movement.
    public override void SimulateController()
    {
        PollKeys(false);

        IPlayerCommandInput input = PlayerCommand.Create();

        // Copies all the local variables to the input command.
        input.Forward = _forward;
        input.Backward = _backward;
        input.LeftStrafe = _leftStrafe;
        input.RightStrafe = _rightStrafe;
        input.Jump = _jump;
        input.Yaw = _yaw;
        input.Pitch = _pitch;

        // Queues the input for processing; client prediction.
        entity.QueueInput(input);
    }

    // Executes on both the controller (usually client) and owner (usually server)
    public override void ExecuteCommand(Bolt.Command command, bool resetState)
    {
        PlayerCommand cmd = (PlayerCommand)command;

        if (resetState)
        {
            // There was a correction from the server; reset. This only runs on the client.
            //_motor.SetState (cmd.Result.Position, cmd.Result.Velocity, cmd.Result.IsGrounded, cmd.Result.JumpFrames);
        }
        else
        {
            // apply movement (both server + client)
            PlayerMotor.State motorState = _motor.Move(cmd.Input.Forward, cmd.Input.Backward, cmd.Input.LeftStrafe, cmd.Input.RightStrafe, cmd.Input.Jump, cmd.Input.Yaw, cmd.Input.Pitch);

            // copy motor state to the command's result (client)
            cmd.Result.Position = motorState.position;
            cmd.Result.Velocity = motorState.velocity;
            cmd.Result.IsGrounded = motorState.isGrounded;
            cmd.Result.JumpFrames = motorState.jumpFrames;
        }
    }

    public override void Attached()
    {
        state.SetTransforms(state.PlayerTransform, transform);
    }

    void PollKeys(bool mouse)
    {
        _forward = Input.GetKey(KeyCode.W);
        _backward = Input.GetKey(KeyCode.S);
        _leftStrafe = Input.GetKey(KeyCode.A);
        _rightStrafe = Input.GetKey(KeyCode.D);

        if (mouse)
        {
            _yaw += (Input.GetAxisRaw("Mouse X") * MOUSE_SENSITIVITY);
            _yaw %= 360f;

            _pitch += (-Input.GetAxisRaw("Mouse Y") * MOUSE_SENSITIVITY);
            _pitch = Mathf.Clamp(_pitch, -85f, 85f);
        }
    }

}
