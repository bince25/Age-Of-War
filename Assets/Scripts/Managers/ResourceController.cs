using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceController : MonoBehaviour
{

    [SerializeField]
    public TextMeshProUGUI goldText, ageText, ageProgressionText, ageGoalText, happinessText;

    [Header("Unit Costs")]
    public int clubmanCost;
    public int slingerCost;
    public int stoneTankCost;

    UnitSpawner unitSpawner;

    [Space(15)]
    public SpawnSide playerSide;
    int multiplier = 1;
    public int gold, ageProgression, happiness;
    private bool readyForIronAge, readyForMedievalAge, readyForRenaissanceAge, readyForModernAge, readyForFutureAge;
    void Start()
    {
        unitSpawner = GameObject.FindGameObjectWithTag("GameManager").GetComponent<UnitSpawner>();
        playerSide = SpawnSide.Left;
        ageText.text = Ages.Stone.ToString() + " Age";
        ageProgressionText.text = ageProgression.ToString();
        ageGoalText.text = Constants.TO_IRON_AGE.ToString();
        goldText.text = gold.ToString();
        happinessText.text = happiness.ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UnitDied(string unitSide, string unitType)
    {
        if (unitSide == playerSide.ToString())
        {
            Debug.Log("Player died");
        }
        else
        {
            Debug.Log("Enemy died");
            IncreaseGold(100);
            IncreaseAgeProgression(10);
            IncreaseHappiness(10);
        }
    }

    public void nextAge()
    {
        if (readyForIronAge)
        {
            ageText.text = Ages.Iron.ToString() + " Age";
        }
        if (readyForMedievalAge)
        {
            ageText.text = Ages.Medieval.ToString() + " Age";
        }
        if (readyForRenaissanceAge)
        {
            ageText.text = Ages.Renaissance.ToString() + " Age";
        }
        if (readyForModernAge)
        {
            ageText.text = Ages.Modern.ToString() + " Age";
        }
        unitSpawner.canSpawnMountedUnit = false;
        unitSpawner.canSpawnTankUnit = false;
        unitSpawner.canSpawnSiegeUnit = false;
        if (readyForFutureAge)
        {
            ageText.text = Ages.Future.ToString() + " Age";
        }

    }

    public void IncreaseGold(int amount)
    {
        gold += amount;
        goldText.text = gold.ToString();
    }
    public void DecreaseGold(int amount)
    {
        gold -= amount;
        goldText.text = gold.ToString();
    }
    public void IncreaseAgeProgression(int amount)
    {
        ageProgression += amount;
        ageProgressionText.text = ageProgression.ToString();
        if (ageProgression >= Constants.TO_IRON_AGE)
        {
            ageGoalText.text = Constants.TO_MEDIEVAL_AGE.ToString();
            readyForIronAge = true;
        }
        if (ageProgression >= Constants.TO_MEDIEVAL_AGE)
        {
            ageGoalText.text = Constants.TO_RENAISSANCE_AGE.ToString();
            readyForMedievalAge = true;
        }
        if (ageProgression >= Constants.TO_RENAISSANCE_AGE)
        {
            ageGoalText.text = Constants.TO_MODERN_AGE.ToString();
            readyForRenaissanceAge = true;
        }
        if (ageProgression >= Constants.TO_MODERN_AGE)
        {
            ageGoalText.text = Constants.TO_FUTURE_AGE.ToString();
            readyForModernAge = true;
        }
        if (ageProgression >= Constants.TO_FUTURE_AGE)
        {
            ageGoalText.text = "----";
            readyForFutureAge = true;
        }
    }
    public void IncreaseHappiness(int amount)
    {
        happiness += amount;
        happinessText.text = happiness.ToString();
    }
}
