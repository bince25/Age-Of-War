using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI maxHealthText, currentHealthText;

    public void SetMaxHealth(float maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
        maxHealthText.text = maxHealth.ToString();
        currentHealthText.text = maxHealth.ToString();
    }

    public void SetHealth(float health)
    {
        slider.value = health;
        currentHealthText.text = health.ToString();
    }
}
