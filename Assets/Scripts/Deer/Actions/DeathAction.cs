using UnityEngine;

public class DeathAction : MonoBehaviour
{
    AgentAnimator _agentAnimator;

    public void Awake()
    {
        _agentAnimator = GetComponent<AgentAnimator>();
    }

    public void Die()
    {
        _agentAnimator.SetDeath();
        Destroy(gameObject, 1.5f);
    }
}