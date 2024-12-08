using BehaviourAPI.Core;
using UnityEngine;

public class WanderAction : MonoBehaviour
{
    private AgentMovementWrapper _movement;
    private AgentInfo _info;
    private AgentAnimator _animation;
    private AgentSocial _social;

    private float _counter = 0f;

    private Transform _leaderTransform;
    private Vector3 _leaderPos;

    private void Start()
    {
        _movement = GetComponent<AgentMovementWrapper>();
        _info = GetComponent<AgentInfo>();
        _animation = GetComponent<AgentAnimator>();
        _social = GetComponent<AgentSocial>();
    }

    public void WanderStart()
    {
        bool validPosition = false;
        Vector3 position = new Vector3();
        while (!validPosition)
        {
            Vector2 randomPos = Random.insideUnitCircle * _info.WanderRange;
            position = transform.position + new Vector3(randomPos.x, transform.position.y, randomPos.y);
            validPosition = _movement.NavMeshAgentMovement.CanMove(position);
        }

        _movement.Walk();
        _movement.NavMeshAgentMovement.SetTarget(position);
    }

    public Status WanderUpdate()
    {
        if (_movement.NavMeshAgentMovement.HasArrived())
        {
            return Status.Success;
        }

        return Status.Running;
    }

    public void WanderExit()
    {
        _leaderTransform = null;
        _movement.NavMeshAgentMovement.CancelMove();
        _counter = 0f;
    }


    public void DelayStart()
    {
        _animation.SetIdle();

        _counter = Random.Range(0f, 4f);
    }

    public Status DelayUpdate()
    {
        _counter -= Time.deltaTime;
        if (_counter <= 0f)
        {
            return Status.Success;
        }

        return Status.Running;
    }


    public void FollowLeaderStart()
    {
        _leaderTransform = _social.GetLeader().transform;
        _leaderPos = _leaderTransform.position;
        _movement.Walk();
        _movement.NavMeshAgentMovement.SetTarget(_leaderPos);
    }

    public Status FollowLeaderUpdate()
    {
        try
        {
            if (_leaderTransform is null || !_social.HasLeader() || _social.GetLeader().transform != _leaderTransform)
            {
                _movement.NavMeshAgentMovement.CancelMove();
                return Status.Failure;
            }
        }
        catch
        {
            Debug.LogWarning("El lider esta destruido");
            return Status.Failure;
        }

        if (Vector3.Distance(transform.position, _leaderPos) < _info.CloseToLeaderRange)
        {
            return Status.Success;
        }

        if (Vector3.Distance(_leaderTransform.position, _leaderPos) > _info.LeaderMoveThreshold)
        {
            _leaderPos = _leaderTransform.position;
            _movement.NavMeshAgentMovement.SetTarget(_leaderPos);
        }

        return Status.Running;
    }

    public void FollowLeaderExit()
    {
        _animation.SetIdle();
        WanderExit();
    }

    public void Idle()
    {
        _animation.SetIdle();
    }

    public bool IsLeaderMoving()
    {
        try
        {
            if (!_social.HasLeader()) return false;

            var leader = _social.GetLeader();
            return leader is not null &&
                   leader.GetComponent<AgentMovementWrapper>().NavMeshAgentMovement.IsMoving();
        }
        catch
        {
            return false;
        }
    }

    public bool IsLeaderStill() => !IsLeaderMoving();

    public bool HasToFollowLeader()
    {
        if (!IsLeaderMoving()) return false;
        var leader = _social.GetLeader();
        if (leader is null) return false;
        
        return Vector3.Distance(transform.position, leader.transform.position) > _info.CloseToLeaderRange;
    }

}