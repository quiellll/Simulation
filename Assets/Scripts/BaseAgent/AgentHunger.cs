using UnityEngine;
using System;

[RequireComponent(typeof(AgentInfo))]
public class AgentHunger : MonoBehaviour
{
    public float _timeSinceLastMeal;

    public event Action<float> onHungerUpdated;

    [SerializeField] private float _updateRate = 0.5f;
    private float _updateCountdown;

    private void Awake()
    {
        _updateCountdown = _updateRate;
    }

    public float TimeSinceLastMeal() => _timeSinceLastMeal;

    private void Update()
    {
        _timeSinceLastMeal += Time.deltaTime;
        _updateCountdown -= Time.deltaTime;
        
        if (_updateCountdown <= 0)
        {
            _updateCountdown = _updateRate;
            onHungerUpdated?.Invoke(_timeSinceLastMeal);
        }
    }

    public void ResetHunger()
    {
        _timeSinceLastMeal = 0;
    }

    public void IncreaseHunger()
    {
        _timeSinceLastMeal += 10;
    }

    public void DecreaseHunger()
    {
        _timeSinceLastMeal -= 10;
    }

}