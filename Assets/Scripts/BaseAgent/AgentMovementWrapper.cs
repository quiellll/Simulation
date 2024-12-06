using UnityEngine;
using UnityEngine.AI;
using BehaviourAPI.UnityToolkit;

public class AgentMovementWrapper : MonoBehaviour
{
    [SerializeField] private float _runSpeedMultiplier = 2f;
    private AgentAnimator _agentAnimator;

    private float _walkSpeed;
    public NavmeshAgentMovement NavMeshAgentMovement {get; private set;}

    public void Start()
    {
        NavMeshAgentMovement = GetComponent<NavmeshAgentMovement>();
        _agentAnimator = GetComponent<AgentAnimator>();
        _walkSpeed = NavMeshAgentMovement.Speed;
    }

    public void Run()
    {
        NavMeshAgentMovement.Speed = _walkSpeed * _runSpeedMultiplier;
        _agentAnimator.SetRun();
    }

    public void Walk()
    {
        NavMeshAgentMovement.Speed = _walkSpeed;
        _agentAnimator.SetWalk();
    }

}