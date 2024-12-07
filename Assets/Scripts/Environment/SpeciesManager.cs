
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpeciesManager : MonoBehaviour
{
    public static SpeciesManager Instance { get; private set; }

    private readonly List<DeerBehaviourRunner> _deer = new();

    private List<AgentSocial> _deerMatingQueue = new();
    
    // public bool IsDirty { get; private set; }
    
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Register(AgentSocial agentSocial)
    {
        if (agentSocial.TryGetComponent<DeerBehaviourRunner>(out var deer))
        {
            _deer.Add(deer);
        }
    }

    public void CheckOut(AgentSocial agentSocial)
    {
        if (agentSocial.TryGetComponent<DeerBehaviourRunner>(out var deer))
        {
            _deer.Remove(deer);
        }
    }

    //si devuelve true llena la lista de todos los ciervos dentro del rango, ordenados de mas cercano a mas lejano
    public bool TryGetSameSpecies(AgentSocial agentSocial, out List<AgentSocial> orderedSS, float maxRange)
    {
        orderedSS = null;
        if (agentSocial.TryGetComponent<DeerBehaviourRunner>(out var deer))
        {
            Vector3 deerPos = deer.transform.position;
            orderedSS = new();
            //quitamos los que esten muy lejos
            foreach (var d in _deer)
            {
                if(d != deer && Vector3.Distance(deerPos, d.transform.position) <= maxRange)
                    orderedSS.Add(d.GetComponent<AgentSocial>());
            }
            
            //ordenamos
            orderedSS.Sort((a, b) => 
                Vector3.Distance(deerPos, a.transform.position).
                    CompareTo(Vector3.Distance(deerPos, b.transform.position)));
        }

        return orderedSS is not null && orderedSS.Count > 0;
    }

    public void RegisterToMate(AgentSocial agentSocial)
    {
        if (agentSocial.TryGetComponent<DeerBehaviourRunner>(out var _))
        {
            for (int i = 0; i < _deerMatingQueue.Count; i++)
            {
                var other = _deerMatingQueue[i];

                if (agentSocial.GetGroupAlpha() == other.GetGroupAlpha())
                {
                    //match found
                    _deerMatingQueue.RemoveAt(i);
                    bool authority = Random.Range(0f, 1f) >= 0.5f;
                    agentSocial.AssignPartner(other, authority);
                    other.AssignPartner(agentSocial, !authority);
                    return;
                }
            }
            
            _deerMatingQueue.Add(agentSocial);
        }
    }

    public void CheckOutFromMate(AgentSocial agentSocial)
    {
        _deerMatingQueue.Remove(agentSocial);
    }
    

    public List<AgentHealth> GetPreyList()
    {
        return _deer.Select(d => d.GetComponent<AgentHealth>()).ToList();
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    private void OnGUI()
    {
        // Primer botón
        if (GUI.Button(new Rect(700, 10, 300, 50), "Crear Sociedad"))
        {
            foreach (var deer in _deer)
            {
                var social = deer.GetComponent<AgentSocial>();
                if (!social.HasLeader())
                    social.FindAndSetLeader();

            }
        }

    }
}
