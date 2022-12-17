using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using Networking.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkPilotInputProvider : SimulationBehaviour, INetworkRunnerCallbacks
{
    public PlayerInput playerInput;
    public NetworkManager NetworkManager;

    bool shouldCrouch;
    bool shouldSprint;
    bool canShoot;
    bool shouldReload;
    bool shouldSwitchPrimary;
    bool shouldSwitchSecondary;
    
    Vector2 moveData;
    Vector2 look;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        if (NetworkManager._runner != null)
        {
            NetworkManager._runner.AddCallbacks(this);
        }
    }

    private void OnDisable()
    {
        if (NetworkManager._runner != null)
        {
            NetworkManager._runner.RemoveCallbacks(this);
        }
    }

    public void OnMove(InputValue value)
    {
        moveData = value.Get<Vector2>();
    }

    public void OnSprint(InputValue value)
    {
        shouldSprint = value.isPressed;
    }

    public void OnCrouch(InputValue value)
    {
        shouldCrouch = value.isPressed;
    }
    
    public void OnLook(InputValue value)
    {
        look = value.Get<Vector2>();
    }
    
    public void OnFire(InputValue value)
    {
        canShoot = value.isPressed;
    }
    
    public void OnReload(InputValue value)
    {
        shouldReload = value.isPressed;
    }
    
    public void OnPrimary()
    {
        shouldSwitchPrimary = true;
    }
    public void OnSecondary()
    {
        shouldSwitchSecondary = true;
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        // NOT NEEDED BUT FORCED TO SINCE INTERFACE IMPLEMENTATION!
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        // NOT NEEDED BUT FORCED TO SINCE INTERFACE IMPLEMENTATION!
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        NetworkPilotInput networkPilotInput = new NetworkPilotInput();

        networkPilotInput.Buttons.Set(PilotButtons.Sprint, shouldSprint);
        networkPilotInput.Buttons.Set(PilotButtons.Crouch, shouldCrouch);
        networkPilotInput.Buttons.Set(PilotButtons.Jump, playerInput.actions["Jump"].triggered);
        networkPilotInput.Buttons.Set(PilotButtons.Reload, shouldReload);
        networkPilotInput.Buttons.Set(PilotButtons.Shoot, canShoot);
        networkPilotInput.Buttons.Set(PilotButtons.SwitchPrimary, shouldSwitchPrimary);
        networkPilotInput.Buttons.Set(PilotButtons.SwitchSecondary, shouldSwitchSecondary);

        networkPilotInput.look = look;
        networkPilotInput.moveData = moveData;

        input.Set(networkPilotInput);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        // NOT NEEDED BUT FORCED TO SINCE INTERFACE IMPLEMENTATION!
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        // NOT NEEDED BUT FORCED TO SINCE INTERFACE IMPLEMENTATION!
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        // NOT NEEDED BUT FORCED TO SINCE INTERFACE IMPLEMENTATION!
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        // NOT NEEDED BUT FORCED TO SINCE INTERFACE IMPLEMENTATION!
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        // NOT NEEDED BUT FORCED TO SINCE INTERFACE IMPLEMENTATION!
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        // NOT NEEDED BUT FORCED TO SINCE INTERFACE IMPLEMENTATION!
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        // NOT NEEDED BUT FORCED TO SINCE INTERFACE IMPLEMENTATION!
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        // NOT NEEDED BUT FORCED TO SINCE INTERFACE IMPLEMENTATION!
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        // NOT NEEDED BUT FORCED TO SINCE INTERFACE IMPLEMENTATION!
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        // NOT NEEDED BUT FORCED TO SINCE INTERFACE IMPLEMENTATION!
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        // NOT NEEDED BUT FORCED TO SINCE INTERFACE IMPLEMENTATION!
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        // NOT NEEDED BUT FORCED TO SINCE INTERFACE IMPLEMENTATION!
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        // NOT NEEDED BUT FORCED TO SINCE INTERFACE IMPLEMENTATION!
    }
}