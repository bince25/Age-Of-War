using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    public BuildingType buildingType;
    private ResourceController resourceController;
    private UnitSpawner unitSpawner;

    public bool built = false;

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
                gameObject.GetComponent<BuildingController>().buildingType = BuildingType.Farm;
                this.farmLevel = 1;
                farmGoldMultiplier = 1;
                break;
            case "LeftBarrack":
            case "RightBarrack":
                gameObject.GetComponent<BuildingController>().buildingType = BuildingType.Barrack;
                this.barrackLevel = 0;
                break;
            case "LeftStable":
            case "RightStable":
                gameObject.GetComponent<BuildingController>().buildingType = BuildingType.Stable;
                this.stableLevel = 0;
                break;
            case "LeftWorkshop":
            case "RightWorkshop":
                gameObject.GetComponent<BuildingController>().buildingType = BuildingType.Workshop;
                this.workshopLevel = 0;
                break;
        }
    }

    // Update is called once per frame
    void Start()
    {
        gameObject.name = GUID.Generate().ToString();
        GameManager.Instance.buildingsDictionary.Add(gameObject.tag, this);
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
        if (!built)
        {
            built = true;
            GameManager.Instance.CurrentStrategy.HandleBuildingCreation(this);
        }
        else GameManager.Instance.CurrentStrategy.HandleBuildingLevelUp(unitSpawner, resourceController, this);
    }

    public void HandleLevelUp()
    {
        if (buildingType == BuildingType.Farm)
        {
            farmLevel++;
            farmGoldMultiplier *= 1.2f;
            farmFoodMultiplier *= 1.4f;
            ChangeFarmMultiplier(farmGoldMultiplier);
        }
        else if (buildingType == BuildingType.Barrack)
        {
            barrackLevel++;
            if (barrackLevel == 1 && resourceController.ageText.text == Ages.Stone.ToString() + " Age")
            {
                unitSpawner.canSpawnTankUnit = true;
            }
            else if (barrackLevel == 2 && resourceController.ageText.text == Ages.Iron.ToString() + " Age")
            {
                unitSpawner.canSpawnTankUnit = true;
            }
            else if (barrackLevel == 3 && resourceController.ageText.text == Ages.Medieval.ToString() + " Age")
            {
                unitSpawner.canSpawnTankUnit = true;
            }
            else if (barrackLevel == 4 && resourceController.ageText.text == Ages.Renaissance.ToString() + " Age")
            {
                unitSpawner.canSpawnTankUnit = true;
            }
            else if (barrackLevel == 5 && resourceController.ageText.text == Ages.Modern.ToString() + " Age")
            {
                unitSpawner.canSpawnTankUnit = true;
            }
            else if (barrackLevel == 6 && resourceController.ageText.text == Ages.Future.ToString() + " Age")
            {
                unitSpawner.canSpawnTankUnit = true;
            }
            else
            {
                unitSpawner.canSpawnTankUnit = false;
            }
        }
        else if (buildingType == BuildingType.Stable)
        {
            stableLevel++;
            if (stableLevel == 1 && resourceController.ageText.text == Ages.Iron.ToString() + " Age")
            {
                unitSpawner.canSpawnMountedUnit = true;
            }
            else if (stableLevel == 2 && resourceController.ageText.text == Ages.Medieval.ToString() + " Age")
            {
                unitSpawner.canSpawnMountedUnit = true;
            }
            else if (stableLevel == 3 && resourceController.ageText.text == Ages.Renaissance.ToString() + " Age")
            {
                unitSpawner.canSpawnMountedUnit = true;
            }
            else if (stableLevel == 4 && resourceController.ageText.text == Ages.Modern.ToString() + " Age")
            {
                unitSpawner.canSpawnMountedUnit = true;
            }
            else if (stableLevel == 5 && resourceController.ageText.text == Ages.Future.ToString() + " Age")
            {
                unitSpawner.canSpawnMountedUnit = true;
            }
            else
            {
                unitSpawner.canSpawnMountedUnit = false;
            }
        }
        else if (buildingType == BuildingType.Workshop)
        {
            workshopLevel++;
            if (workshopLevel == 1 && resourceController.ageText.text == Ages.Iron.ToString() + " Age")
            {
                unitSpawner.canSpawnSiegeUnit = true;
            }
            else if (workshopLevel == 2 && resourceController.ageText.text == Ages.Medieval.ToString() + " Age")
            {
                unitSpawner.canSpawnSiegeUnit = true;
            }
            else if (workshopLevel == 3 && resourceController.ageText.text == Ages.Renaissance.ToString() + " Age")
            {
                unitSpawner.canSpawnSiegeUnit = true;
            }
            else if (workshopLevel == 4 && resourceController.ageText.text == Ages.Modern.ToString() + " Age")
            {
                unitSpawner.canSpawnSiegeUnit = true;
            }
            else if (workshopLevel == 5 && resourceController.ageText.text == Ages.Future.ToString() + " Age")
            {
                unitSpawner.canSpawnSiegeUnit = true;
            }
            else
            {
                unitSpawner.canSpawnSiegeUnit = false;
            }
        }
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