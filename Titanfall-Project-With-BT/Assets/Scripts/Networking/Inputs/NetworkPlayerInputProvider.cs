
using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class NetworkPlayerInputProvider : MonoBehaviour, INetworkRunnerCallbacks
{
    private PilotInputMap _pilotInputMap;

    private NetworkManager _networkManager;

    private void Awake()
    {
        _networkManager = GetComponentInParent<NetworkManager>();
        _pilotInputMap = new PilotInputMap();
    }

    public void OnEnable() {
        if(_networkManager.Runner != null){
            // enabling the input map
            _pilotInputMap.Player.Enable();
            _pilotInputMap.Weapons.Enable();
            _pilotInputMap.Pilot.Enable();
            _pilotInputMap.Titan.Enable();

            _networkManager.Runner.AddCallbacks(this);
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        // Ignore this.
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        // Ignore this.
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var pilotInput = new NetworkPlayerInput();
        var playerActions = _pilotInputMap.Player;
        var pilotActions = _pilotInputMap.Pilot;
        var titanActions = _pilotInputMap.Titan;
        var weaponActions = _pilotInputMap.Weapons;

        pilotInput.Buttons.Set(NetworkPlayerButtons.WALK, titanActions.Walk.IsPressed());
        pilotInput.Buttons.Set(NetworkPlayerButtons.CROUCH, pilotActions.Crouch.IsPressed());
        pilotInput.Buttons.Set(NetworkPlayerButtons.SPRINT, playerActions.Sprint.IsPressed());
        pilotInput.Buttons.Set(NetworkPlayerButtons.DASH, titanActions.Dash.triggered);
        pilotInput.Buttons.Set(NetworkPlayerButtons.JUMP, pilotActions.Jump.triggered);
        pilotInput.Buttons.Set(NetworkPlayerButtons.SHOOT, weaponActions.Fire.IsPressed());
        pilotInput.Buttons.Set(NetworkPlayerButtons.RELOAD, weaponActions.Reload.IsPressed());
        pilotInput.Buttons.Set(NetworkPlayerButtons.SWITCH_PRIMARY, pilotActions.Primary.IsPressed());
        pilotInput.Buttons.Set(NetworkPlayerButtons.SWITCH_SECONDARY, pilotActions.Secondary.IsPressed());
        pilotInput.Buttons.Set(NetworkPlayerButtons.SWITCH_ANTI, pilotActions.AntiTitan.IsPressed());
        
        pilotInput.move = playerActions.Move.ReadValue<Vector2>();
        pilotInput.look = playerActions.Look.ReadValue<Vector2>();
        
        input.Set(pilotInput);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        // Debug.Log("Input by " + player.PlayerId + " is missing! (Input -> " + input + ")");
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        // Ignore this.
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        // Ignore this.
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        // Ignore this.
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        // Ignore this.
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        // Ignore this.
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        // Ignore this.
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        // Ignore this.
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        // Ignore this.
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        // Ignore this.
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        // Ignore this.
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        // Ignore this.
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        // Ignore this.
    }

    public void OnDisable(){
        if(_networkManager.Runner != null){
            // disabling the input map
            _pilotInputMap.Player.Disable();
            _pilotInputMap.Weapons.Disable();
            _pilotInputMap.Pilot.Disable();
            _pilotInputMap.Titan.Disable();

            _networkManager.Runner.RemoveCallbacks( this );
        }
    }
}