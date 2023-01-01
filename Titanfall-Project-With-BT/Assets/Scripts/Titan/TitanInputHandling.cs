using Fusion;

public class TitanInputHandling : NetworkBehaviour
{
    [Networked] public NetworkButtons ButtonsPrevious { get; set; }

    VanguardMovement moveScript;
    VanguardCamera titanCamera;
    FireTitanRifle fireTitanRifle;
    
    private void Start()
    {
        titanCamera = GetComponentInParent<VanguardCamera>();
        moveScript = GetComponentInParent<VanguardMovement>();
        fireTitanRifle = GetComponentInParent<FireTitanRifle>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput<NetworkPlayerInput>(out var input) == false) return;

        var pressed = input.Buttons.GetPressed(ButtonsPrevious);
        
        moveScript.shouldSprint = pressed.IsSet(NetworkPlayerButtons.SPRINT);
        moveScript.moveData = input.move;
        titanCamera.look = input.look * titanCamera.sensitivity;


        if (pressed.IsSet(NetworkPlayerButtons.SHOOT))
        {
            if (!moveScript.isSprinting)
            {
                fireTitanRifle.canShoot = true;
            }
        }
        else
        {
            fireTitanRifle.canShoot = false;
        }

        fireTitanRifle.shouldReload = pressed.IsSet(NetworkPlayerButtons.RELOAD);
    }
}