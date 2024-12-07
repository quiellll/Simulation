using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentSocial : MonoBehaviour
{
    [SerializeField] private Material _alphaMat;

    private AgentHealth _health;
    private AgentInfo _info;

    private AgentSocial _leader = null;

    // private readonly HashSet<AgentSocial> _subordinates = new();
    private Material _defaultMat;

    public AgentSocial GetLeader() => _leader;
    public bool IsAlpha() => _leader is null;
    public bool HasLeader() => _leader is not null;
    // public bool HasSubordinates() => _subordinates.Count > 0;


    private void Awake()
    {
        _info = GetComponent<AgentInfo>();
        _health = GetComponent<AgentHealth>();
        _health.OnHealthChanged += OnHealthChanged;
        _defaultMat = GetComponentInChildren<SkinnedMeshRenderer>().materials[1];
        SetMaterial(_alphaMat);
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
            [1] = mat
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

    // public bool IsSuperiorOf(AgentSocial other) //comprueba si other esta en la lista de subordinados
    // {
    //     Queue<AgentSocial> queue = new Queue<AgentSocial>(_subordinates);
    //
    //     while (queue.TryPeek(out var subordinate))
    //     {
    //         queue.Dequeue();
    //
    //         if (subordinate == other)
    //             return true;
    //
    //         foreach (var s in subordinate._subordinates)
    //             queue.Enqueue(s);
    //     }
    //
    //     return false;
    // }

    public void SetLeader(AgentSocial leader)
    {
        if (leader is null) throw new Exception("bro leader no puede ser null hazlo con otra funcion eso");

        _leader = leader;

        // leader._subordinates.Add(this);

        _leader.GetComponent<AgentHealth>().OnHealthChanged += OnLeaderHealthChange;
        SetMaterial(_defaultMat);
    }

    public void RemoveLeader()
    {
        if (_leader is null) throw new Exception("bro no peudes remove leader si no tiene leader!");

        // _leader._subordinates.Remove(this);
        _leader.GetComponent<AgentHealth>().OnHealthChanged -= OnLeaderHealthChange;
        _leader = null;
        SetMaterial(_alphaMat);
    }

    public bool FindLeader()
    {
        if (HasLeader()) throw new Exception("bro quita antes el lider actual antes de buscar otro nuevo");

        Debug.Log("buscando lider...");

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
        if (!leaderHealth.IsDead()) return;
        RemoveLeader();
    }

    private static AgentSocial a = null;


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