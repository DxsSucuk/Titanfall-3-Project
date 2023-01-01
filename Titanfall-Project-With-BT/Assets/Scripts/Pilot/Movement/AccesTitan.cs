using Fusion;

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AccesTitan : NetworkBehaviour
{
    [Networked]
    public NetworkObject TitanObject { get; set; }
    
    [Networked]
    public EnterVanguardTitan TitanScript { get; set; }

    PilotMovement moveScript;

    GameObject[] titanDropPoints;
    float shortestDistance = 0f;
    Transform chosenPoint;

    Animator animator;

    private void Start()
    {
        titanDropPoints = GameObject.FindGameObjectsWithTag("DropPoint");
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
            MoveTitanToDropLocation();
            TitanScript.StartFall();
        }
    }

    private void MoveTitanToDropLocation()
    {
        for (int i = 0; i < titanDropPoints.Length; i++)
        {
            float distance = Vector3.Distance(titanDropPoints[i].transform.position, this.transform.position);
            if (distance < shortestDistance || shortestDistance == 0f)
            {
                chosenPoint = hit.point;
            }
            else
            {
                chosenPoint = networkPlayerObject.transform.position;
            }

            Vector3 spawnPosition = chosenPoint + new Vector3(0, 150, 0);
            NetworkObject networkPlayerTitanObject =
                Runner.Spawn(_vanguardTitanPrefab, spawnPosition, Quaternion.identity,
                    Runner.LocalPlayer);
            
            if (HasInputAuthority)
            {
                ////networkPlayerTitanObject.gameObject.layer = 6;
                ////LayerUtility.SetLayerRecrusivly(networkPlayerTitanObject.transform);
            }
        }
        //trying to move the titan to a specific drop point, does not work, would be good if you cold spawn the titan at this point
        TitanObject.transform.position = chosenPoint.transform.position;
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
            animator.SetTrigger("embark");
        }
    }

    public void ExitTitan()
    {
        moveScript.embarking = false;
        moveScript.canMove = true;
        animator.SetTrigger("exitTitan");
    }

}
