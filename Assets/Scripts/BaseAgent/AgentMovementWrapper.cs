using UnityEngine;
using UnityEngine.AI;
using BehaviourAPI.UnityToolkit;

public class AgentMovementWrapper : MonoBehaviour
{
    private AgentAnimator _agentAnimator;
    private AgentInfo _info;
    public NavmeshAgentMovement NavMeshAgentMovement {get; private set;}

    public void Start()
    {
        NavMeshAgentMovement = GetComponent<NavmeshAgentMovement>();
        _agentAnimator = GetComponent<AgentAnimator>();
        _info = GetComponent<AgentInfo>();
    }

    public void Run()
    {
        NavMeshAgentMovement.Speed = _info.RunSpeed;
        _agentAnimator.SetRun();
    }

    public void Walk()
    {
        NavMeshAgentMovement.Speed = _info.WalkSpeed;
        _agentAnimator.SetWalk();
    }

}