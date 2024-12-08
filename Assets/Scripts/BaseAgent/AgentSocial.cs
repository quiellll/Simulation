using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentSocial : MonoBehaviour
{
    [SerializeField] private Material _alphaMat;
    [SerializeField] private int _alphaMatIndex = 1;
    private AgentHealth _health;
    private AgentInfo _info;

    private AgentSocial _leader = null;

    // private readonly HashSet<AgentSocial> _subordinates = new();
    private Material _defaultMat;

    private int _subCount = 0;

    private AgentSocial _partner;
    private bool _isCoupleAuthority;

    public AgentSocial GetLeader() => _leader;
    public bool IsAlpha() => _leader is null;
    public bool HasLeader() => _leader is not null;
    // public bool HasSubordinates() => _subordinates.Count > 0;

    public bool IsInGroup() => HasLeader() || _subCount > 0;
    public float IsInGroupFloat() => IsInGroup() ? 1f : 0f;

    public bool HasPartner() => _partner is not null;
    public float HasPartnerFloat() => _partner is null ? 0f : 1f;

    public AgentSocial GetPartner() => _partner;

    public bool IsCoupleAuthority() => _isCoupleAuthority;
    
    private void Awake()
    {
        _info = GetComponent<AgentInfo>();
        _health = GetComponent<AgentHealth>();
        _health.OnHealthChanged += OnHealthChanged;
        _defaultMat = GetComponentInChildren<SkinnedMeshRenderer>().materials[_alphaMatIndex];
        if(IsAlpha()) SetMaterial(_alphaMat);
    }

    private void Start()
    {
        SpeciesManager.Instance.Register(this);
    }

    private void SetMaterial(Material mat)
    {
        var smr = GetComponentInChildren<SkinnedMeshRenderer>();
        var mats = new List<Material>(smr.materials)
        {
            [_alphaMatIndex] = mat
        };
        smr.SetMaterials(mats);
    }

    public bool IsInferiorOf(AgentSocial other) //comprueba si other esta en la cadena de lideres de this
    {
        var leader = _leader;
        while (leader is not null)
        {
            if (leader == other)
                return true;
            leader = leader.GetLeader();
        }

        return false;
    }

    public AgentSocial GetGroupAlpha()
    {
        if (IsAlpha()) return this;

        var alpha = _leader;
        while (!alpha.IsAlpha()) alpha = alpha.GetLeader();

        return alpha;
    }
    
    public void SetLeader(AgentSocial leader)
    {
        if (leader is null) throw new Exception("bro leader no puede ser null hazlo con otra funcion eso");

        _leader = leader;
        _leader._subCount++;
        // leader._subordinates.Add(this);

        _leader.GetComponent<AgentHealth>().OnHealthChanged += OnLeaderHealthChange;
        SetMaterial(_defaultMat);
    }

    public void RemoveLeader()
    {
        if (_leader is null)
            return;

        // _leader._subordinates.Remove(this);
        _leader = null;
        SetMaterial(_alphaMat);

        try
        {

            _leader.GetComponent<AgentHealth>().OnHealthChanged -= OnLeaderHealthChange;
            _leader._subCount--;
        }
        catch
        {
            Debug.LogWarning("Estas quitando el lider ya destruido, eso esta regular pero bueno es la que hay");
        }
    }
    

    public bool FindAndSetLeader()
    {
        if (HasLeader()) throw new Exception("bro quita antes el lider actual antes de buscar otro nuevo");
        
        if (!SpeciesManager.Instance.TryGetSameSpecies
                (this, out var possibleLeaders, _info.SeekSameSpeciesRange))
            return false;

        foreach (var possibleLeader in possibleLeaders)
        {
            if (possibleLeader.IsInferiorOf(this)) continue; //si soy su lider no puede ser mi lider
            SetLeader(possibleLeader);
            return true;
        }

        return false;
    }

    public bool FindAnySubordinate(out AgentSocial sub)
    {
        sub = null;
        if (!SpeciesManager.Instance.TryGetSameSpecies
                (this, out var possibleSubs, _info.SeekSameSpeciesRange))
            return false;

        foreach (var pSub in possibleSubs)
        {
            if (pSub.IsInferiorOf(this))
            {
                sub = pSub;
                return true;
            }
        }

        return false;

    }

    public void AssignPartner(AgentSocial partner, bool isCoupleAuthority)
    {
        if (HasPartner()) throw new Exception("ya tienes pareja bro no puedes mas");
        _partner = partner;
        _isCoupleAuthority = isCoupleAuthority;
        _partner.GetComponent<AgentHealth>().OnHealthChanged += OnPartnerHealthChange;

    }

    public void RemovePartner()
    {
        if (_partner is null) throw new Exception("bro no puedes remove pareja si no tiene pareja!");

        // _leader._subordinates.Remove(this);
        _partner.GetComponent<AgentHealth>().OnHealthChanged -= OnLeaderHealthChange;
        _partner = null;
    }

    private void OnHealthChanged(float _)
    {
        if (!_health.IsDead()) return;

        if (HasLeader()) RemoveLeader();

        SpeciesManager.Instance.CheckOut(this);

        // if (HasSubordinates())
        //     foreach (var sub in _subordinates)
        //         sub.RemoveLeader();
    }

    private void OnLeaderHealthChange(float _)
    {
        if (!HasLeader()) return;

        var leaderHealth = _leader.GetComponent<AgentHealth>();
        if (leaderHealth.IsDead())
        {
            Debug.Log("quitando lider porque se murio...");
            RemoveLeader();
        }
    }

    private void OnPartnerHealthChange(float _)
    {
        if (!HasPartner()) return;

        var partnerHealth = _partner.GetComponent<AgentHealth>();

        if (partnerHealth.IsDead())
            RemovePartner();
    }
    
    private void OnDrawGizmos()
    {
        if (!HasLeader()) return;
        var color = Gizmos.color;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, _leader.transform.position);
        Vector3 spherePos = Vector3.Lerp(transform.position, _leader.transform.position, .9f);
        Gizmos.DrawWireSphere(spherePos, 1f);
        Gizmos.color = color;
    }
}