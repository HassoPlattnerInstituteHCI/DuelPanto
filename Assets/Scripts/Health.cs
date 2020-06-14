using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class DefeatEvent : UnityEvent<GameObject> { }


public class Health : MonoBehaviour
{
    [HideInInspector]
    public int healthPoints = 100;
    public Slider healthSlider;
    public Image sliderImage;
    public Color fullHealthColor = Color.green;
    public Color zeroHealthColor = Color.red;
    public int maxHealth = 100;
    public DefeatEvent notifyDefeat;

    void Start()
    {
        healthSlider.minValue = 0;
        healthSlider.maxValue = maxHealth;
        UpdateUI();
    }

    void OnEnable()
    {
        healthPoints = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(int amount)
    {
        healthPoints -= amount;

        UpdateUI();

        if (healthPoints <= 0)
        {
            notifyDefeat.Invoke(gameObject);
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
