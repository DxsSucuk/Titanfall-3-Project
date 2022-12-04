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
                shortestDistance = distance;
                chosenPoint = titanDropPoints[i].transform;
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
        }
    }

}
