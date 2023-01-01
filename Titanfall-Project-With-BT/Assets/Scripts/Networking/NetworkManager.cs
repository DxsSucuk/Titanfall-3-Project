using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public NetworkRunner Runner;

    [SerializeField] private NetworkPrefabRef _playerPrefab;

    [SerializeField] private NetworkPrefabRef _vanguardTitanPrefab;

    public GameObject InputProviderGameObject;

    public Transform spawnA;
    public Transform spawnB;

    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    async void StartGame(GameMode gameMode)
    {
        Runner = gameObject.AddComponent<NetworkRunner>();
        Runner.ProvideInput = true;

        await Runner.StartGame(new StartGameArgs()
        {
            GameMode = gameMode,
            SessionName = "WeBallin",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
        });
        
        InputProviderGameObject.SetActive(true);
    }

    private void OnGUI()
    {
        if (Runner == null)
        {
            if (GUI.Button(new Rect(0, 40, 200, 40), "Create"))
            {
                StartGame(GameMode.Host);
            }
            if (GUI.Button(new Rect(0, 100, 200, 40), "Join"))
            {
                StartGame(GameMode.Client);
            }
        }
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Connected to Server -> " + runner.name);
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log("Connection failed to Server -> " + runner.name + " (" + remoteAddress + ") " +
                  reason);
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        // TODO:: for future menu screen.
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        // TODO:: for future authentication.
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        Debug.Log("Disconnected from Server -> " + runner.name);
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        Debug.Log("Host-Migration!");
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        // Is being handled by the NetworkPilotInputProvider!
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        // Is being handled by the NetworkPilotInputProvider!
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("Player joined Server -> " + player.PlayerId);
        if (runner.LocalPlayer == player)
        {
            // Create a unique position for the player
            Vector3 spawnPosition = spawnB.position;
            Vector3 titanPosition = spawnPosition;
            titanPosition.y = 178;
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);

            runner.SetPlayerObject(player, networkPlayerObject);

            NetworkObject networkPlayerTitanObject =
                runner.Spawn(_vanguardTitanPrefab, titanPosition, Quaternion.identity, player);

            networkPlayerTitanObject.gameObject.layer = 6;
            LayerUtility.SetLayerRecrusivly(networkPlayerTitanObject.transform);

            AccesTitan accesTitan = networkPlayerObject.GetComponent<AccesTitan>();
            accesTitan.TitanObject = networkPlayerTitanObject;
            EnterVanguardTitan enterVanguardTitan = networkPlayerTitanObject.GetComponent<EnterVanguardTitan>();
            enterVanguardTitan.player = networkPlayerObject.gameObject;

            enterVanguardTitan.playerCamera = enterVanguardTitan.player.GetComponentInChildren<Camera>().gameObject;
            accesTitan.TitanScript = enterVanguardTitan;

            // Keep track of the player avatars so we can remove it when they disconnect
            _spawnedCharacters.Add(player, networkPlayerObject);
        }
    }
    
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("Player left Server -> " + runner.name);
        // Find and remove the players avatar
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject.GetComponent<AccesTitan>().TitanObject);
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        // No need for this right now.
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        // TODO:: for future menu screen.
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        // TODO:: for future menu screen.
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        // No need for this right now.
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        // TODO:: for future menu screen.
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        // No need for this right now.
    }
}