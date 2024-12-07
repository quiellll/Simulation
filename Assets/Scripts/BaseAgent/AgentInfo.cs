

using UnityEngine;

public class AgentInfo : MonoBehaviour
{
    public int MaxHealth;
    public float WalkSpeed;
    public float RunSpeed;
    public float SeekFoodRange;
    public float WanderRange;
    public float CloseToLeaderRange;
    public float LeaderMoveThreshold;
    public float SeekSameSpeciesRange;

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, SeekFoodRange);
    }
}