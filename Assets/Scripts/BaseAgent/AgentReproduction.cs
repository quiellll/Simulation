using UnityEngine;
using System;
public class AgentReproduction : MonoBehaviour
{
    public float _timeSinceLastMating;

    public event Action<float> OnReproductionUpdated;

    [SerializeField] private float _updateRate = 0.5f;
    private float _updateCountdown;

    private void Awake()
    {
        _updateCountdown = _updateRate;
    }

    public float TimeSinceLastMating() => _timeSinceLastMating;

    private void Update()
    {
        _timeSinceLastMating += Time.deltaTime;
        _updateCountdown -= Time.deltaTime;
        
        if (_updateCountdown <= 0)
        {
            _updateCountdown = _updateRate;
            OnReproductionUpdated?.Invoke(_timeSinceLastMating);
        }
    }

    public void ResetReproduction()
    {
        _timeSinceLastMating = 0;
    }
}
