using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviourAPI.UnityToolkit;
using BehaviourAPI.Core;

public class SeekPreyAction : MonoBehaviour
{
    private AgentInfo _info;
    private AgentHunger _hunger;
    private AgentMovementWrapper _movement;
    private AgentAnimator _agentAnimator;
    
    private Prey _selectedPrey;
    private Action _callback;
    
    private bool _hasEatenPrey;
    private bool _isEating;

    public void Awake()
    {
        _info = GetComponent<AgentInfo>();
        _movement = GetComponent<AgentMovementWrapper>();
        _agentAnimator = GetComponent<AgentAnimator>();
        _hunger = GetComponent<AgentHunger>();
    }

    public float IsEating() => _isEating ? 1 : 0;


    public Status SearchForPreyNearby()
    {
        Debug.Log("Searching for food");

        return FoodManager.Instance.TryGetPrey(transform.position, out _selectedPrey, _info.SeekFoodRange) 
            ? Status.Success
            : Status.Failure;
    }
    
    
    public void SeekPreyExit()
    {
        _selectedPrey = null;
        _movement.NavMeshAgentMovement.CancelMove();
        _isEating = false;
    }
    
    public void ChasePreyStart()
    {

        _movement.Run();
        _movement.NavMeshAgentMovement.SetTarget(_selectedPrey.transform.position);
       
    }

    public Status ChasePreyUpdate()
    {
        if (_selectedPrey == null || !_selectedPrey.gameObject.activeSelf)
        {
            _selectedPrey = null;
            _movement.NavMeshAgentMovement.CancelMove();
            return Status.Failure;
        }

        if (Vector3.Distance(transform.position, _selectedPrey.transform.position) < 3f)
        {
            _movement.NavMeshAgentMovement.CancelMove();
            return Status.Success;
        }
        
        // ir updateando la posicion de la presa
        _movement.NavMeshAgentMovement.SetTarget(_selectedPrey.transform.position);
        return Status.Running;
    }

    public void ChasePreyExit()
    {
        _agentAnimator.SetAttack();
        _movement.NavMeshAgentMovement.CancelMove();
        _isEating = false;
    }

    public void EatPreyStart()
    {
        _agentAnimator.SetEat();
        _hasEatenPrey = false;
        _isEating = true;
        _selectedPrey.Eat(() => _hasEatenPrey = true);
        //Eat(() => _hasEatenPrey = true, _selectedPrey);
        
        //_hunger.ResetHunger();
    }

    public Status EatPreyUpdate()
    {
        if(_hasEatenPrey)
        {
            _isEating = false;
            _hunger.ResetHunger();
            _agentAnimator.SetIdle();
            return Status.Success;
        }
        if (_selectedPrey == null || !_selectedPrey.gameObject.activeSelf)
        {
            _isEating = false;
            _agentAnimator.SetIdle();
            _selectedPrey = null;
            return Status.Failure;
        }
        return Status.Running;
    }
}
