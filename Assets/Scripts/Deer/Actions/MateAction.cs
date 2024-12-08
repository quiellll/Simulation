
using System;
using BehaviourAPI.Core;
using UnityEngine;
using Random = UnityEngine.Random;

public class MateAction : MonoBehaviour
{
    private AgentSocial _social;
    private AgentInfo _info;
    private AgentMovementWrapper _movement;
    private AgentAnimator _animator;

    private float _findPartnerCountdown;
    private float _mateCountdown;

    private void Awake()
    {
        _social = GetComponent<AgentSocial>();
        _info = GetComponent<AgentInfo>();
        _movement = GetComponent<AgentMovementWrapper>();
        _animator = GetComponent<AgentAnimator>();
    }

    public void FindPartnerStart()
    {
        _animator.SetIdle();
        if(_social.HasPartner()) _social.RemovePartner();
        SpeciesManager.Instance.RegisterToMate(_social);
        _findPartnerCountdown = _info.MaxFindPartnerTime;
    }

    public Status FindPartnerUpdate()
    {
        if (_social.HasPartner())
        {
            return Status.Success;
        }

        _findPartnerCountdown -= Time.deltaTime;
        if (_findPartnerCountdown <= 0f)
        {
            SpeciesManager.Instance.CheckOutFromMate(_social);
            return Status.Failure;
        }

        return Status.Running;
    }

    public void FindPartnerExit()
    {
        SpeciesManager.Instance.CheckOutFromMate(_social);
    }


    public void GoToPartnerStart()
    {
        var partnerPos = _social.GetPartner().transform.position;
        if (!_social.IsCoupleAuthority())
        {
            _movement.LookAt(partnerPos);
        }
        else
        {
            _movement.Walk();
            _movement.NavMeshAgentMovement.SetTarget(partnerPos);
        }
    }

    public Status GoToPartnerUpdate()
    {
        if (!_social.HasPartner())
        {
            return Status.Failure;
        }

        if (Vector3.Distance(transform.position, _social.GetPartner().transform.position) < 5f)
        {
            return Status.Success;
        }

        return Status.Running;
    }

    public void GoToPartnerExit()
    {
        _movement.NavMeshAgentMovement.CancelMove();
        _animator.SetIdle();
        if(_social.HasPartner()) _social.RemovePartner();
    }

    public void MateStart()
    {
        _mateCountdown = _info.MatingTime;
    }

    public Status MateUpdate()
    {
        _mateCountdown -= Time.deltaTime;
        if (_mateCountdown > 0f) return Status.Running;
        
        GetComponent<AgentReproduction>().ResetReproduction();
        if (!_social.IsCoupleAuthority()) return Status.Success;

        var newBorn = Instantiate(_info.Prefab, transform.position, Quaternion.identity);
        newBorn.GetComponent<AgentSocial>().SetLeader(_social);
        newBorn.GetComponent<AgentGrowth>().InitAsChild();
        return Status.Success;
    }
    
}
