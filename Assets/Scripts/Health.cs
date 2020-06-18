using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class DefeatEvent : UnityEvent<GameObject> { }

[System.Serializable]
public class DamageEvent : UnityEvent<GameObject> { }

public class Health : MonoBehaviour
{
    // TODO: 5. Make the levels way longer. Just give players 4x more health.
    [HideInInspector]
    public int healthPoints = 100;
    public Slider healthSlider;
    public Image sliderImage;
    public Color fullHealthColor = Color.green;
    public Color zeroHealthColor = Color.red;
    public int maxHealth = 100;
    public DefeatEvent notifyDefeat;
    public DamageEvent notifyDamage;

    void OnEnable()
    {
        healthSlider.minValue = 0;
        healthSlider.maxValue = maxHealth;
        healthPoints = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(int amount, GameObject from)
    {
        healthPoints -= amount;

        UpdateUI();

        if (healthPoints <= 0)
        {
            notifyDefeat.Invoke(gameObject);
        } else
        {
            notifyDamage.Invoke(from);
        }
    }

    public void Heal(int amount)
    {
        healthPoints = Mathf.Min(healthPoints + amount, maxHealth);
    }

    void UpdateUI()
    {
        healthSlider.value = healthPoints;
        sliderImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, healthPoints / (float)maxHealth);
    }
}
