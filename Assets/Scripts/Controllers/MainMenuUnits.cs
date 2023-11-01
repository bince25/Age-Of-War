using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUnits : MonoBehaviour
{
    private SPUM_Prefabs unitSpumController;

    void Start()
    {
        unitSpumController = GetComponent<SPUM_Prefabs>();
        unitSpumController.PlayAnimation("attack_normal");
    }
    void Update()
    {

    }
}
