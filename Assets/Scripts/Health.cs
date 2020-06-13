using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DefeatEvent : UnityEvent<GameObject> { }


public class Health : MonoBehaviour
{
    public int health = 100;
    public int maxHealth = 100;
    public DefeatEvent notifyDefeat;

    void OnEnable()
    {
        health = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        health -= amount;

        if (health <= 0)
        {
            notifyDefeat.Invoke(gameObject);
        }
    }

    public void Heal(int amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
    }
}
