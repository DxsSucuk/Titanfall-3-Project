using Fusion;
using UnityEngine;

namespace Networking.Inputs
{
    public enum PilotButtons
    {
        Sprint = 1,
        Crouch = 2,
        Jump = 3,
    }

    public struct NetworkPilotInput : INetworkInput
    {
        public Vector2 moveData;
        public Vector2 look;

        public NetworkButtons Buttons { get; set; }
    }
}