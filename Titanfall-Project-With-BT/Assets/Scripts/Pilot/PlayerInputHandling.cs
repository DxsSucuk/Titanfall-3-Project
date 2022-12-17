using System;
using System.Collections;
using System.Collections.Generic;
using Networking.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerInputHandling : MonoBehaviour
{
    private PilotMovement PilotMovement;

    public bool CanShoot => PilotMovement.networkPilotInput.Buttons.IsSet(PilotButtons.Shoot);

    public bool ShouldReload => PilotMovement.networkPilotInput.Buttons.IsSet(PilotButtons.Reload);

    public int Weapon => PilotMovement.networkPilotInput.Buttons.IsSet(PilotButtons.SwitchPrimary) ? 1 :
            PilotMovement.networkPilotInput.Buttons.IsSet(PilotButtons.SwitchSecondary) ? 2 : 0;

    private void Awake()
    {
        PilotMovement = GetComponent<PilotMovement>();
    }
}