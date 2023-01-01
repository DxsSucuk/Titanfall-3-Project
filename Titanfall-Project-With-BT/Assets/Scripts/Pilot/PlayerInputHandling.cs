using Fusion;

public class PlayerInputHandling : NetworkBehaviour
{
    [Networked] public NetworkButtons ButtonsPrevious { get; set; }

    public bool canShoot, shouldReload;
    public int weapon;
    
    WeaponSwitching weaponSwitch;
    PilotMovement moveScript;
    PilotCamera pilotCamera;

    private void Start()
    {
        pilotCamera = GetComponentInParent<PilotCamera>();
        weaponSwitch = GetComponentInChildren<WeaponSwitching>();
        moveScript = GetComponentInParent<PilotMovement>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput<NetworkPlayerInput>(out var input) == false) return;

        var pressed = input.Buttons.GetPressed(ButtonsPrevious);

        moveScript.shouldCrouch = pressed.IsSet(NetworkPlayerButtons.CROUCH);
        moveScript.shouldSprint = pressed.IsSet(NetworkPlayerButtons.SPRINT);
        moveScript.shouldJump = pressed.IsSet(NetworkPlayerButtons.JUMP);
        moveScript.moveData = input.move;
        pilotCamera.look = input.look * pilotCamera.sensitivity;


        if (pressed.IsSet(NetworkPlayerButtons.SHOOT))
        {
            if (!moveScript.isSprinting)
            {
                canShoot = true;
            }
        }
        else
        {
            canShoot = false;
        }

        shouldReload = pressed.IsSet(NetworkPlayerButtons.RELOAD);

        if (pressed.IsSet(NetworkPlayerButtons.SWITCH_PRIMARY))
        {
            weapon = 1;
            weaponSwitch.Select();
            weaponSwitch.HandleRig();
        }

        if (pressed.IsSet(NetworkPlayerButtons.SWITCH_SECONDARY))
        {
            weapon = 2;
            weaponSwitch.Select();
            weaponSwitch.HandleRig();
        }

        if (pressed.IsSet(NetworkPlayerButtons.SWITCH_ANTI))
        {
            weapon = 3;
            weaponSwitch.Select();
            weaponSwitch.HandleRig();
        }
    }
}