using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceController : MonoBehaviour
{

    [SerializeField]
    public TextMeshProUGUI goldText, ageText, ageProgressionText, ageGoalText, foodText, happinessText;
    public Image happinessIcon;



    [Header("Unit Costs")]
    public int clubmanCost;
    public int slingerCost;
    public int stoneTankCost;
    public int turretCost;

    [Header("Unit Rewards")]

    public int clubmanReward;
    public int slingerReward;
    public int stoneTankReward;


    public int currentAge = 1;

    UnitSpawner unitSpawner;

    [SerializeField]

    private Sprite[] resourceSprites;

    [Space(15)]
    public SpawnSide playerSide;
    int multiplier = 1;
    public int gold, ageProgression, happiness, food;
    private bool readyForIronAge, readyForMedievalAge, readyForRenaissanceAge, readyForModernAge, readyForFutureAge;
    private bool readyForNextAge = false;
    void Start()
    {
        unitSpawner = GameObject.FindGameObjectWithTag("GameManager").GetComponent<UnitSpawner>();
        playerSide = SpawnSide.Left;
        ageText.text = Ages.Stone.ToString() + " Age";
        ageProgressionText.text = ageProgression.ToString();
        ageGoalText.text = Constants.TO_IRON_AGE.ToString();
        goldText.text = gold.ToString();
        happinessText.text = happiness.ToString();
        foodText.text = food.ToString();
    }

    public void UnitDied(string unitSide, UnitType unitType)
    {
        if (unitSide == playerSide.ToString())
        {
            Debug.Log("Player died");
        }
        else
        {
            Debug.Log(unitType);
            switch (unitType)
            {
                case UnitType.Archer:
                    IncreaseGold(clubmanReward);
                    IncreaseAgeProgression(clubmanReward / 2);
                    IncreaseHappiness(clubmanReward / 10);
                    break;
                case UnitType.Clubman:
                    IncreaseGold(clubmanReward);
                    IncreaseAgeProgression(clubmanReward / 2);
                    IncreaseHappiness(clubmanReward / 10);
                    break;
                case UnitType.Slinger:
                    IncreaseGold(slingerReward);
                    IncreaseAgeProgression(slingerReward / 2);
                    IncreaseHappiness(slingerReward / 10);
                    break;
                case UnitType.StoneTank:
                    IncreaseGold(stoneTankReward);
                    IncreaseAgeProgression(stoneTankReward / 2);
                    IncreaseHappiness(stoneTankReward / 10);
                    break;
                default:
                    break;
            }

            Debug.Log("Enemy died");

        }
    }

    public void nextAge()
    {
        if (readyForNextAge)
        {
            Debug.Log("Next age");
            GameManager.Instance.CurrentStrategy.HandleAdvanceAge(playerSide);
            CheckReadyForNextAge(); // Reset the readyForNextAge flag
        }
    }

    void CheckReadyForNextAge()
    {
        switch (currentAge)
        {
            case 1: // Stone Age
                readyForNextAge = ageProgression >= Constants.TO_IRON_AGE;
                ageGoalText.text = Constants.TO_IRON_AGE.ToString();
                break;
            case 2: // Iron Age
                readyForNextAge = ageProgression >= Constants.TO_MEDIEVAL_AGE;
                ageGoalText.text = Constants.TO_MEDIEVAL_AGE.ToString();
                break;
            case 3: // Medieval Age
                readyForNextAge = ageProgression >= Constants.TO_RENAISSANCE_AGE;
                ageGoalText.text = Constants.TO_RENAISSANCE_AGE.ToString();
                break;
            case 4: // Renaissance Age
                readyForNextAge = ageProgression >= Constants.TO_MODERN_AGE;
                ageGoalText.text = Constants.TO_MODERN_AGE.ToString();
                break;
            case 5: // Modern Age
                readyForNextAge = ageProgression >= Constants.TO_FUTURE_AGE;
                ageGoalText.text = Constants.TO_FUTURE_AGE.ToString();
                break;
            default:
                readyForNextAge = false;
                ageGoalText.text = "----";
                break;
        }
    }

    void UpdateTextColor(TextMeshProUGUI text, int amount)
    {
        if (amount < 0)
        {
            text.color = Color.red;
        }
        else if (amount > 0)
        {
            text.color = Color.green;
        }
        else
        {
            text.color = Color.white;
        }
    }
    public void IncreaseFood(int amount)
    {
        food += amount;
        foodText.text = food.ToString();
        UpdateTextColor(foodText, food);
    }

    public void DecreaseFood(int amount)
    {
        food -= amount;
        foodText.text = food.ToString();
        UpdateTextColor(foodText, food);
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
        Debug.Log("Age progression increased by " + amount);
        ageProgression += amount;
        ageProgressionText.text = ageProgression.ToString();
        CheckReadyForNextAge();
    }
    public void IncreaseHappiness(int amount)
    {
        happiness += amount;
        happinessText.text = happiness.ToString();
        UpdateTextColor(happinessText, happiness);
        UpdateHappinessIcon();
    }
    public void DecreaseHappiness(int amount)
    {
        happiness -= amount;
        happinessText.text = happiness.ToString();
        UpdateTextColor(happinessText, happiness);
        UpdateHappinessIcon();
    }
    void UpdateHappinessIcon()
    {
        if (happiness < 0)
        {
            happinessIcon.sprite = resourceSprites[1];
        }
        else
        {
            happinessIcon.sprite = resourceSprites[0];
        }
    }
}
