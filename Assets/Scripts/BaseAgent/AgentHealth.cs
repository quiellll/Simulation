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
    
    public float IsDeadFloat() => IsDead() ? 1 : 0;

    public void TakeDamage(float damage)
    {
        if (IsDead()) return;
        _currentHealth = Mathf.Max(0f, _currentHealth - damage);
        OnHealthChanged?.Invoke(_currentHealth);
    }

    public void Heal(float amount)
    {
        if (IsDead()) return;
        _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
        OnHealthChanged?.Invoke(_currentHealth);
    }

    public void Kill() => TakeDamage(_currentHealth);

    public float GetHealthPercentage()
    {
        return _currentHealth / _maxHealth;
    }
}