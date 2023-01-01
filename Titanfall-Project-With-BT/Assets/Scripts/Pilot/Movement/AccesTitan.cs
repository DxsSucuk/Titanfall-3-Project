using Fusion;

using UnityEngine;
using Utilities;

public class AccesTitan : NetworkBehaviour
{
    public NetworkObject TitanObject;

    public EnterVanguardTitan TitanScript;
    
    [SerializeField] private NetworkPrefabRef _vanguardTitanPrefab;

    PilotMovement moveScript;

    Animator animator;

    private void Start()
    {
        moveScript = GetComponent<PilotMovement>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        StartTitanFall();
        EmbarkWithTitan();
    }
 
    void StartTitanFall()
    {
        if (!HasInputAuthority) return;
        
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (TitanObject != null)
                Runner.Despawn(TitanObject);
            
            GetDropPointRPC();
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, InvokeLocal = false)]
    private void GetDropPointRPC()
    {
        Vector3 chosenPoint = Vector3.zero;
        
        if (Runner.TryGetPlayerObject(Object.InputAuthority, out NetworkObject networkPlayerObject))
        {
            Camera pilotCamera = networkPlayerObject.GetComponentInChildren<Camera>();

            Vector3 direction = pilotCamera.transform.forward;

            if (Physics.Raycast(pilotCamera.transform.position, direction, out RaycastHit hit))
            {
                chosenPoint = hit.point;
            }
            else
            {
                // Fallback just in case.
                chosenPoint = networkPlayerObject.transform.position;
            }
        }
        
        chosenPoint += new Vector3(0, 150, 0);
        
        SpawnToDropLocation(chosenPoint);
    }
    
    private void SpawnToDropLocation(Vector3 vectorPosition)
    {
        if (Runner.TryGetPlayerObject(Object.InputAuthority, out NetworkObject networkPlayerObject))
        {
            TitanObject =
                Runner.Spawn(_vanguardTitanPrefab, vectorPosition, Quaternion.identity,
                    Object.InputAuthority);
            
            SetTitanDataRPC(TitanObject.Id);
        }
    }
    
    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.InputAuthority, InvokeLocal = true)]
    private void SetTitanDataRPC(NetworkId networkPlayerTitanId)
    {
        TitanScript = Runner.TryGetNetworkedBehaviourFromNetworkedObjectRef<EnterVanguardTitan>(networkPlayerTitanId);
        TitanObject = TitanScript.Object;

        if (Runner.TryGetPlayerObject(Object.InputAuthority, out NetworkObject networkPlayerObject))
        {
            TitanScript.player = networkPlayerObject.gameObject;

            if (HasInputAuthority)
            {
                TitanScript.playerCamera = networkPlayerObject.GetComponentInChildren<Camera>().gameObject;
                TitanObject.gameObject.layer = 6;
                LayerUtility.ReplaceLayerRecursively(TitanObject.transform, 9, 6);
            }
        }
    }

    void EmbarkWithTitan()
    {
        if (!HasInputAuthority) return;
        
        if (Input.GetKeyDown(KeyCode.F) && TitanScript.inRangeForEmbark)
        {
            StartCoroutine(TitanScript.Embark());

            //moveScript.EmbarkLookDirection(TitanScript.embarkLookTarget.position);

            moveScript.lookTarget = TitanScript.embarkLookTarget.position;
            moveScript.canMove = false;
            moveScript.embarking = true;
            moveScript.embarkPos = TitanScript.embarkPos.position;
            PlayAnimationRPC();
        }
    }

    [Rpc]
    public void PlayAnimationRPC()
    {
        animator.SetTrigger("embark");
    }

    public void ExitTitan()
    {
        moveScript.embarking = false;
        moveScript.canMove = true;
        animator.SetTrigger("exitTitan");
    }

}
