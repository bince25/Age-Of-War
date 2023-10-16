using System.Collections;
using UnityEngine;

public class Building : MonoBehaviour
{
    public BuildingType buildingType;
    private ResourceController resourceController;

    public bool isBarrack;
    public bool isFarm;
    public bool isStable;
    public bool isWorkshop;



    [Header("Farm")]
    public int farmLevel;
    public int goldIncrement;
    public float goldIncrementCooldown;

    [Header("Barrack")]
    public int barrackLevel;

    [Header("Stable")]
    public int stableLevel;

    [Header("Workshop")]
    public int workshopLevel;

    void Awake()
    {
        resourceController = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ResourceController>();
        string tag = gameObject.tag;
        switch (tag)
        {
            case "LeftFarm":
            case "RightFarm":
                gameObject.GetComponent<Building>().buildingType = BuildingType.Farm;
                break;
            case "LeftBarrack":
            case "RightBarrack":
                gameObject.GetComponent<Building>().buildingType = BuildingType.Barrack;
                break;
            case "LeftStable":
            case "RightStable":
                gameObject.GetComponent<Building>().buildingType = BuildingType.Stable;
                break;
            case "LeftWorkshop":
            case "RightWorkshop":
                gameObject.GetComponent<Building>().buildingType = BuildingType.Workshop;
                break;
        }
    }

    // Update is called once per frame
    void Start()
    {
        switch (buildingType)
        {
            case BuildingType.Farm:
                StartCoroutine(FarmFunction());
                break;
            case BuildingType.Barrack:
                break;
            case BuildingType.Stable:
                break;
            case BuildingType.Workshop:
                break;
        }
    }

    IEnumerator FarmFunction()
    {
        while (true)
        {
            yield return new WaitForSeconds(goldIncrementCooldown);
            resourceController.IncreaseGold(goldIncrement);
        }
    }

    public void LevelUp()
    {
        Debug.Log(buildingType + " Leveled Up!");
    }

    public enum BuildingType
    {
        Farm,
        Barrack,
        Stable,
        Workshop
    }
}
