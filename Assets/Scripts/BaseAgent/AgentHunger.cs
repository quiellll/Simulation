using UnityEngine;
using System;

public class AgentHunger : MonoBehaviour
{
    public  float _timeSinceLastMeal;
    
    public float TimeSinceLastMeal() => _timeSinceLastMeal;

    private void Update()
    {
        _timeSinceLastMeal += Time.deltaTime;
    }

    public void ResetHunger()
    {
        _timeSinceLastMeal = 0;
    }


}