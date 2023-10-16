using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceController : MonoBehaviour
{

    [SerializeField]
    public TextMeshProUGUI goldText, ageProgressionText, ageGoalText, happinessText;

    public SpawnSide playerSide;
    int multiplier = 1;
    int gold, ageProgression, happiness;
    void Start()
    {
        playerSide = SpawnSide.Left;
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
            IncreaseGold(10);
            IncreaseAgeProgression(10);
            IncreaseHappiness(10);
        }
    }

    public void IncreaseGold(int amount)
    {
        gold += amount;
        goldText.text = gold.ToString();
    }
    public void IncreaseAgeProgression(int amount)
    {
        ageProgression += amount;
        ageProgressionText.text = ageProgression.ToString();
        if (ageProgression >= Constants.TO_IRON_AGE)
        {
            ageGoalText.text = Constants.TO_MEDIEVAL_AGE.ToString();
        }
        if (ageProgression >= Constants.TO_MEDIEVAL_AGE)
        {
            ageGoalText.text = Constants.TO_RENAISSANCE_AGE.ToString();
        }
        if (ageProgression >= Constants.TO_RENAISSANCE_AGE)
        {
            ageGoalText.text = Constants.TO_MODERN_AGE.ToString();
        }
        if (ageProgression >= Constants.TO_MODERN_AGE)
        {
            ageGoalText.text = Constants.TO_FUTURE_AGE.ToString();
        }
        if (ageProgression >= Constants.TO_FUTURE_AGE)
        {
            ageGoalText.text = "----";
        }
    }
    public void IncreaseHappiness(int amount)
    {
        happiness += amount;
        happinessText.text = happiness.ToString();
    }
}
