using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Building : MonoBehaviour
{
    public BuildingType buildingType;
    private ResourceController resourceController;
    private UnitSpawner unitSpawner;

    public bool isBarrack;
    public bool isFarm;
    public bool isStable;
    public bool isWorkshop;

    [Header("Farm")]
    private Coroutine farmCoroutine;
    public int farmLevel;
    public int goldIncrement;
    public float incrementCooldown;
    public float farmGoldMultiplier;
    public float farmFoodMultiplier;
    public int foodIncrement;

    [Header("Barrack")]
    public int barrackLevel;

    [Header("Stable")]
    public int stableLevel;

    [Header("Workshop")]
    public int workshopLevel;

    void Awake()
    {
        resourceController = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ResourceController>();
        unitSpawner = GameObject.FindGameObjectWithTag("GameManager").GetComponent<UnitSpawner>();
        string tag = gameObject.tag;
        switch (tag)
        {
            case "LeftFarm":
            case "RightFarm":
                gameObject.GetComponent<Building>().buildingType = BuildingType.Farm;
                this.farmLevel = 1;
                farmGoldMultiplier = 1;
                break;
            case "LeftBarrack":
            case "RightBarrack":
                gameObject.GetComponent<Building>().buildingType = BuildingType.Barrack;
                this.barrackLevel = 0;
                break;
            case "LeftStable":
            case "RightStable":
                gameObject.GetComponent<Building>().buildingType = BuildingType.Stable;
                this.stableLevel = 0;
                break;
            case "LeftWorkshop":
            case "RightWorkshop":
                gameObject.GetComponent<Building>().buildingType = BuildingType.Workshop;
                this.workshopLevel = 0;
                break;
        }
    }

    // Update is called once per frame
    void Start()
    {
        gameObject.name = GUID.Generate().ToString();
        GameManager.Instance.buildingsDictionary.Add(gameObject.name, this);
        switch (buildingType)
        {
            case BuildingType.Farm:
                StartFarmFunction();
                break;
            case BuildingType.Barrack:
                break;
            case BuildingType.Stable:
                break;
            case BuildingType.Workshop:
                break;
        }
    }

    //------------------------------------------FARM----------------------------------------------------------
    IEnumerator FarmCoroutine(float multiplier = 1, float foodMultiplier = 1)
    {
        while (true)
        {
            yield return new WaitForSeconds(incrementCooldown);
            resourceController.IncreaseGold((int)(goldIncrement * multiplier));
            resourceController.IncreaseFood((int)(foodIncrement * foodMultiplier));
        }
    }
    public void ChangeFarmMultiplier(float newMultiplier)
    {
        farmGoldMultiplier = newMultiplier;
        StartFarmFunction();
    }
    void StartFarmFunction()
    {
        if (farmCoroutine != null)
        {
            // Stop the previous coroutine if it's running
            StopCoroutine(farmCoroutine);
        }

        // Start a new coroutine with the updated multiplier
        farmCoroutine = StartCoroutine(FarmCoroutine(farmGoldMultiplier, farmFoodMultiplier));
    }

    //--------------------------------------------------------------------------------------------------------
    public void LevelUp()
    {
        GameManager.Instance.CurrentStrategy.HandleBuildingLevelUp(unitSpawner, resourceController, this);
    }
}

[System.Serializable]
public class BuildingData
{
    public string id;
    public string type;
    public string side;
    public int level;
}