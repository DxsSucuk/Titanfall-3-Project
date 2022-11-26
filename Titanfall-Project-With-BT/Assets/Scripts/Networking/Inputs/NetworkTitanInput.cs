using Fusion;
using UnityEngine;

namespace Networking.Inputs
{
    public enum TitanButtons
    {
        Sprint = 1,
        Walk = 2,
        Dash = 3,
    }

    public struct NetworkTitanInput : INetworkInput
    {
        public NetworkButtons Buttons { get; set; }

        public Vector2 moveData;
    }
}