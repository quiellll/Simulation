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
        _health.Kill();
        _agentAnimator.SetDeath();
        Destroy(gameObject, 1.5f);
    }
}