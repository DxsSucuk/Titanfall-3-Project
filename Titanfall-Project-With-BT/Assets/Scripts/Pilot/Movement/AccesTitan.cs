using Fusion;
using UnityEngine;

public class AccesTitan : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabRef _vanguardTitanPrefab;

    public NetworkObject TitanObject { get; set; }

    public EnterVanguardTitan TitanScript { get; set; }

    GameObject[] titanDropPoints;
    float shortestDistance = 0f;
    Vector3 chosenPoint;

    private void Start()
    {
        titanDropPoints = GameObject.FindGameObjectsWithTag("DropPoint");
    }

    void Update()
    {
        StartTitanFall();
        EmbarkWithTitan();
    }

    void StartTitanFall()
    {
        if (!HasStateAuthority) return;

        if (Input.GetKeyDown(KeyCode.V) && (TitanObject == null || !TitanObject.isActiveAndEnabled))
        {
            if (TitanObject != null)
                Runner.Despawn(TitanObject);
            
            SpawnToDropLocationRPC();
            
            if (TitanScript != null)
                TitanScript.StartFall();
        }
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void SpawnToDropLocationRPC()
    {
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
                for (int i = 0; i < titanDropPoints.Length; i++)
                {
                    float distance = Vector3.Distance(titanDropPoints[i].transform.position, this.transform.position);
                    if (distance < shortestDistance || shortestDistance == 0f)
                    {
                        shortestDistance = distance;
                        chosenPoint = titanDropPoints[i].transform.position;
                    }
                }
    
            }

            Vector3 spawnPosition = chosenPoint + new Vector3(0, 150, 0);
            NetworkObject networkPlayerTitanObject =
                Runner.Spawn(_vanguardTitanPrefab, spawnPosition, Quaternion.identity,
                    Runner.LocalPlayer);
            networkPlayerTitanObject.gameObject.layer = 6;
            SetLayerRecrusivly(networkPlayerTitanObject.transform);
            TitanObject = networkPlayerTitanObject;

            EnterVanguardTitan enterVanguardTitan = TitanObject.GetComponent<EnterVanguardTitan>();

            enterVanguardTitan.player = networkPlayerObject.gameObject;

            enterVanguardTitan.playerCamera = pilotCamera.gameObject;

            TitanScript = enterVanguardTitan;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(chosenPoint, 5);
    }
    
    private void SetLayerRecrusivly(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.gameObject.layer == 9)
            {
                child.gameObject.layer = 6;
            }

            if (child.childCount > 0)
            {
                SetLayerRecrusivly(child);
            }
        }
    }

    void EmbarkWithTitan()
    {
        if (!HasInputAuthority) return;

        if (TitanScript != null && Input.GetKeyDown(KeyCode.F) && TitanScript.inRangeForEmbark)
        {
            StartCoroutine(TitanScript.Embark());
        }
    }
}