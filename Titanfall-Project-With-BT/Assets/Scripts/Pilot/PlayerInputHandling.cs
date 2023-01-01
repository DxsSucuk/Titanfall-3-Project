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
        if (GetInput<NetworkPilotInput>(out var input) == false) return;

        var pressed = input.Buttons.GetPressed(ButtonsPrevious);

        moveScript.shouldCrouch = pressed.IsSet(NetworkPilotButtons.CROUCH);
        moveScript.shouldSprint = pressed.IsSet(NetworkPilotButtons.SPRINT);
        moveScript.shouldJump = pressed.IsSet(NetworkPilotButtons.JUMP);
        moveScript.moveData = input.move;
        pilotCamera.look = input.look * pilotCamera.sensitivity;


        if (pressed.IsSet(NetworkPilotButtons.SHOOT))
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

        shouldReload = pressed.IsSet(NetworkPilotButtons.RELOAD);

        if (pressed.IsSet(NetworkPilotButtons.SWITCH_PRIMARY))
        {
            weapon = 1;
            weaponSwitch.Select();
            weaponSwitch.HandleRig();
        }

        if (pressed.IsSet(NetworkPilotButtons.SWITCH_SECONDARY))
        {
            weapon = 2;
            weaponSwitch.Select();
            weaponSwitch.HandleRig();
        }

        if (pressed.IsSet(NetworkPilotButtons.SWITCH_ANTI))
        {
            weapon = 3;
            weaponSwitch.Select();
            weaponSwitch.HandleRig();
        }
    }
}