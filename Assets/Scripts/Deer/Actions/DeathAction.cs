using BehaviourAPI.UnityToolkit.GUIDesigner.Runtime;
using UnityEngine;

public class DeathAction : MonoBehaviour
{
    private AgentAnimator _agentAnimator;
    private AgentHealth _health;

    public void Awake()
    {
        _agentAnimator = GetComponent<AgentAnimator>();
        _health = GetComponent<AgentHealth>();
    }

    public void Die()
    {
        GetComponent<AgentMovementWrapper>().NavMeshAgentMovement.CancelMove();
        Destroy(GetComponent<EditorBehaviourRunner>());
        _health.Kill();
        _agentAnimator.SetDeath();
        Destroy(gameObject, 2.5f);
    }
}