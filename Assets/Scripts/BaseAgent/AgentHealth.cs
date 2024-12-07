using UnityEngine;
using System;

public class AgentHealth : MonoBehaviour
{

    [SerializeField] private float _maxHealth = 100f;
    private float _currentHealth;

    public event Action<float> OnHealthChanged;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public bool IsDead()
    {
        return _currentHealth <= 0;
    }

    public void TakeDamage(float damage)
    {
        _currentHealth = Mathf.Max(0f, _currentHealth - damage);
        OnHealthChanged?.Invoke(_currentHealth);
    }

    public void Heal(float amount)
    {
        _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
        OnHealthChanged?.Invoke(_currentHealth);
    }

    public void Kill() => TakeDamage(_currentHealth);

    public float GetHealthPercentage()
    {
        return _currentHealth / _maxHealth;
    }
}