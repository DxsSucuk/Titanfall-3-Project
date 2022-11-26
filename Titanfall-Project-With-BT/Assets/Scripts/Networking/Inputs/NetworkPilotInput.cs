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
        public NetworkButtons Buttons { get; set; }

        public Vector2 moveData;
    }
}