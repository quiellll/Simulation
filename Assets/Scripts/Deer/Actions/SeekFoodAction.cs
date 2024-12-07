using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviourAPI.UnityToolkit;
using BehaviourAPI.Core;

public class SeekFoodAction : MonoBehaviour
{
    private AgentInfo _info;
    private AgentHunger _hunger;
    private AgentMovementWrapper _movement;
    private AgentAnimator _agentAnimator;
    private Food _selectedFood;
    private bool _hasEatenFood;
    private bool _isEating;

    public void Awake()
    {
        _info = GetComponent<AgentInfo>();
        _movement = GetComponent<AgentMovementWrapper>();
        _agentAnimator = GetComponent<AgentAnimator>();
        _hunger = GetComponent<AgentHunger>();
    }

    public float IsEating() => _isEating ? 1 : 0;


    public Status SearchForFoodNearby()
    {
        Debug.Log("Searching for food");

        return FoodManager.Instance.TryGetFood(transform.position, out _selectedFood, _info.SeekFoodRange) 
            ? Status.Success
            : Status.Failure;
    }


    public void GoToFoodStart()
    {

        _movement.Walk();

        _movement.NavMeshAgentMovement.SetTarget(_selectedFood.transform.position);
       
    }
    public Status GoToFoodUpdate()
    {
        if(_selectedFood == null || !_selectedFood.IsAvailable())
        {
            _selectedFood = null;
            _movement.NavMeshAgentMovement.CancelMove();
            return Status.Failure;
        }
        if (Vector3.Distance(transform.position, _selectedFood.transform.position) < 5f)
        {
            _movement.NavMeshAgentMovement.CancelMove();
            return Status.Success;
        }
        return Status.Running;
    }

    public void EatFoodStart()
    {
        _agentAnimator.SetEat();
        _hasEatenFood = false;
        _isEating = true;
        _selectedFood.Eat(() => _hasEatenFood = true);
        //_hunger.ResetHunger();
    }

    public Status EatFoodUpdate()
    {
        if(_hasEatenFood)
        {
            _isEating = false;
            _hunger.ResetHunger();
            _agentAnimator.SetIdle();
            return Status.Success;
        }
        if (_selectedFood == null || !_selectedFood.IsAvailable())
        {
            _isEating = false;
            _agentAnimator.SetIdle();
            _selectedFood = null;
            return Status.Failure;
        }
        return Status.Running;
    }

    public void SeekFoodExit()
    {
        _selectedFood = null;
        _movement.NavMeshAgentMovement.CancelMove();
        _isEating = false;
    }

    public void GoToFoodExit()
    {
        _movement.NavMeshAgentMovement.CancelMove();
        _isEating = false;
    }

}
