

using UnityEngine;

public class AgentInfo : MonoBehaviour
{
    public int MaxHunger;
    public int MaxReproduction;
    public float WalkSpeed;
    public float RunSpeed;
    public float SeekFoodRange;
    public float WanderRange;
    public float CloseToLeaderRange;
    public float LeaderMoveThreshold;
    public float SeekSameSpeciesRange;
    public float MaxFindPartnerTime;
    public float ChildScale;
    public float ChildGrowthTime;
    public float MatingTime;

    public GameObject Prefab;
    
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, SeekFoodRange);
    }
}