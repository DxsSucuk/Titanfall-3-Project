using Fusion;
using UnityEngine;

namespace Networking.Inputs
{
    public enum TitanButtons
    {
        Sprint = 1,
        Walk = 2,
        Dash = 3,
        Shoot = 4,
        Reload = 5,
    }

    public struct NetworkTitanInput : INetworkInput
    {
        public Vector2 moveData;
        public Vector2 look;
        
        public NetworkButtons Buttons { get; set; }
    }
}