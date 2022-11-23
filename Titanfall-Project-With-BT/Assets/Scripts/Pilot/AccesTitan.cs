using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AccesTitan : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabRef _vanguardTitanPrefab;

    public NetworkObject TitanObject { get; set; }

    public EnterVanguardTitan TitanScript { get; set; }

    GameObject[] titanDropPoints;
    float shortestDistance = 0f;
    Transform chosenPoint;

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
        if (!HasInputAuthority) return;

        if (Input.GetKeyDown(KeyCode.V))
        {
            SpawnToDropLocation();
            if (TitanScript != null)
                TitanScript.StartFall();
        }
    }

    private void SpawnToDropLocation()
    {
        if (Runner.TryGetPlayerObject(Object.InputAuthority, out NetworkObject networkPlayerObject))
        {
            
            for (int i = 0; i < titanDropPoints.Length; i++)
            {
                float distance = Vector3.Distance(titanDropPoints[i].transform.position, this.transform.position);
                if (distance < shortestDistance || shortestDistance == 0f)
                {
                    shortestDistance = distance;
                    chosenPoint = titanDropPoints[i].transform;
                }
            }

            NetworkObject networkPlayerTitanObject =
                Runner.Spawn(_vanguardTitanPrefab, chosenPoint.transform.position, Quaternion.identity,
                    Runner.LocalPlayer);
            networkPlayerTitanObject.gameObject.layer = 6;
            SetLayerRecrusivly(networkPlayerTitanObject.transform);
            TitanObject = networkPlayerTitanObject;

            EnterVanguardTitan enterVanguardTitan = TitanObject.GetComponent<EnterVanguardTitan>();

            enterVanguardTitan.player = networkPlayerObject.gameObject;

            enterVanguardTitan.playerCamera = enterVanguardTitan.player.GetComponentInChildren<Camera>().gameObject;

            TitanScript = enterVanguardTitan;
        }
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