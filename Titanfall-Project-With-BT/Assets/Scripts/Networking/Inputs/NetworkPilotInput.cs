using Fusion;
using UnityEngine;

enum NetworkPilotButtons
{
    CROUCH,
    SPRINT,
    JUMP,
    SHOOT,
    RELOAD,
    SWITCH_PRIMARY,
    SWITCH_SECONDARY,
    SWITCH_ANTI,
}

public struct NetworkPilotInput : INetworkInput
{
    public NetworkButtons Buttons;
    public Vector2 move;
    public Vector2 look;
}