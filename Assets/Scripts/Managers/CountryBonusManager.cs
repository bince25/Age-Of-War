using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountryBonusManager : MonoBehaviour
{
    public List<CountryBonus> countryBonuses;

    public float GetBonusValue(string countryName, BonusType bonusType)
    {
        CountryBonus bonus = countryBonuses.Find(b => b.countryName == countryName && b.bonusType == bonusType);
        return bonus?.bonusValue ?? 0f;
    }
}
