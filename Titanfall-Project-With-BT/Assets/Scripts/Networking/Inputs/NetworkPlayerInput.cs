using Fusion;
using UnityEngine;

enum NetworkPlayerButtons
{
    WALK,
    CROUCH,
    SPRINT,
    DASH,
    JUMP,
    SHOOT,
    RELOAD,
    SWITCH_PRIMARY,
    SWITCH_SECONDARY,
    SWITCH_ANTI,
}

public struct NetworkPlayerInput : INetworkInput
{
    public NetworkButtons Buttons;
    public Vector2 move;
    public Vector2 look;
}