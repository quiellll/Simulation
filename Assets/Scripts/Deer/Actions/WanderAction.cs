using BehaviourAPI.Core;
using UnityEngine;

public class WanderAction : MonoBehaviour
{
    private AgentMovementWrapper _movement;
    private AgentInfo _info;
    private AgentAnimator _animation;

    private float _counter = 0f;

    private void Start()
    {
        _movement = GetComponent<AgentMovementWrapper>();
        _info = GetComponent<AgentInfo>();
        _animation = GetComponent<AgentAnimator>();
    }

    public void WanderStart()
    {
        bool validPosition = false;
        Vector3 position = new Vector3();
        while(!validPosition)
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
        if(_movement.NavMeshAgentMovement.HasArrived())
        {
            return Status.Success;
        }
        return Status.Running;
    }

    public void WanderExit()
    {
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
        if(_counter <= 0f)
        {
            return Status.Success;
        }
        return Status.Running;
    }
}