using System;
using DiscordRPC;
using Lachee.Discord.Control;
using UnityEngine;

public class DiscordRPCManager : MonoBehaviour
{
    public DiscordRpcClient DiscordRpcClient;
    public string Application_ID;
    public string Presence_Detail;
    public string Presence_Sate;

    private void Awake()
    {
        if (FindObjectsOfType<DiscordRPCManager>().Length > 1)
        {
            Destroy(this);
        }
        else
        {
            DontDestroyOnLoad(this);
        }
    }

    private void OnEnable()
    {
        DiscordRpcClient = new DiscordRpcClient(Application_ID);
        DiscordRpcClient.Logger = new UnityLogger();

        DiscordRpcClient.OnReady += (sender, e) => Debug.Log($"Received Ready from user {e.User.Username}");
        DiscordRpcClient.OnPresenceUpdate += (sender, e) => Debug.Log($"Received Presence update {e.Presence}");

        DiscordRpcClient.Initialize();

        DiscordRpcClient.SetPresence(new RichPresence
        {
            Details = Presence_Detail,
            State = Presence_Sate,
            Assets = new Assets
            {
                LargeImageKey = "game_logo",
                LargeImageText = "Haralds Titanfall Fan-Game",
                SmallImageKey = "icon_starting",
            },
            Secrets = new Secrets
            {
                JoinSecret = "1234",
            },
            Buttons = new Button[]
            {
                new()
                {
                    Label = "Join!",
                    Url = "tf://join-1234"
                }
            }
        });
    }

    private void Update()
    {
        DiscordRpcClient.Invoke();
    }

    private void OnDisable()
    {
        DiscordRpcClient.Dispose();
    }
}