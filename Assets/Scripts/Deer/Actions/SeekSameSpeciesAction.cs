using System;
using BehaviourAPI.Core;
using UnityEngine;

public class SeekSameSpeciesAction : MonoBehaviour
{
    private AgentSocial _social;
    private AgentInfo _info;
    private AgentMovementWrapper _movement;
    private AgentAnimator _agentAnimator;

    private Vector3 _leaderPos;

    private void Awake()
    {
        _social = GetComponent<AgentSocial>();
        _info = GetComponent<AgentInfo>();
        _movement = GetComponent<AgentMovementWrapper>();
        _agentAnimator = GetComponent<AgentAnimator>();
    }
    
    public Status SearchForSameSpecies()
    {
        return _social.FindAndSetLeader() ? Status.Success : Status.Failure;
    }

    public void GoToSameSpeciesStart()
    {
        _movement.Walk();
        _leaderPos = _social.GetLeader().transform.position;
        _movement.NavMeshAgentMovement.SetTarget(_leaderPos);
    }

    public Status GoToSameSpeciesUpdate()
    {
        if (!_social.HasLeader())
        {
            _movement.NavMeshAgentMovement.CancelMove();
            return Status.Failure;
        }
        

        if (Vector3.Distance(transform.position, _leaderPos) < 5f)
        {
            _movement.NavMeshAgentMovement.CancelMove();
            return Status.Success;
        }
        
        if (Vector3.Distance(transform.position, _leaderPos) < _info.CloseToLeaderRange)
        {
            return Status.Success;
        }

        var newLeaderPos = _social.GetLeader().transform.position; 
        if (Vector3.Distance(newLeaderPos, _leaderPos) > _info.LeaderMoveThreshold)
        {
            _leaderPos = newLeaderPos;
            _movement.NavMeshAgentMovement.SetTarget(_leaderPos);
        }

        return Status.Running;

    }
    
    public void GoToSameSpeciesExit()
    {
        _agentAnimator.SetIdle();
        _movement.NavMeshAgentMovement.CancelMove();
    }
}

