
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpeciesManager : MonoBehaviour
{
    public static SpeciesManager Instance { get; private set; }

    private readonly List<AgentSocial> _deer = new();
    private readonly List<AgentSocial> _wolves = new();

    private List<AgentSocial> _deerMatingQueue = new();
    private List<AgentSocial> _wolvesMatingQueue = new();
    
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
        if (agentSocial.gameObject.CompareTag("Deer"))
        {
            _deer.Add(agentSocial);
        }
        else if (agentSocial.gameObject.CompareTag("Wolf"))
        {
            _wolves.Add(agentSocial);
        }
    }

    public void CheckOut(AgentSocial agentSocial)
    {
        if (agentSocial.gameObject.CompareTag("Deer"))
        {
            _deer.Remove(agentSocial);
        }
        else if (agentSocial.gameObject.CompareTag("Wolf"))
        {
            _wolves.Remove(agentSocial);
        }
    }

    //si devuelve true llena la lista de todos los ciervos dentro del rango, ordenados de mas cercano a mas lejano
    public bool TryGetSameSpecies(AgentSocial agentSocial, out List<AgentSocial> orderedSS, float maxRange)
    {
        orderedSS = null;
        Vector3 agentPos = agentSocial.transform.position;

        List<AgentSocial> list = null;
        
        if (agentSocial.gameObject.CompareTag("Deer"))
            list = _deer;
        else if (agentSocial.gameObject.CompareTag("Wolf"))
            list = _wolves;
        
        orderedSS = new();
        //quitamos los que esten muy lejos
        foreach (var other in list)
        {
            if(other != agentSocial && Vector3.Distance(agentPos, other.transform.position) <= maxRange)
                orderedSS.Add(other);
        }
        
        if (orderedSS is null || orderedSS.Count == 0) return false;
        
        orderedSS.Sort((a, b) => 
            Vector3.Distance(agentPos, a.transform.position).
                CompareTo(Vector3.Distance(agentPos, b.transform.position)));

        return true;
    }

    public void RegisterToMate(AgentSocial agentSocial)
    {
        if (agentSocial.gameObject.CompareTag("Deer"))
            RegisterToMate(_deerMatingQueue, agentSocial);
        
        else if (agentSocial.gameObject.CompareTag("Wolf"))
            RegisterToMate(_wolvesMatingQueue, agentSocial);
    }

    private void RegisterToMate(List<AgentSocial> matingQueue, AgentSocial agentSocial)
    {
        for (int i = 0; i < matingQueue.Count; i++)
        {
            var other = matingQueue[i];

            if (agentSocial.GetGroupAlpha() == other.GetGroupAlpha())
            {
                //match found
                matingQueue.RemoveAt(i);
                bool authority = Random.Range(0f, 1f) >= 0.5f;
                agentSocial.AssignPartner(other, authority);
                other.AssignPartner(agentSocial, !authority);
                return;
            }
        }
            
        matingQueue.Add(agentSocial);
    }

    public void CheckOutFromMate(AgentSocial agentSocial)
    {
        if (agentSocial.gameObject.CompareTag("Deer"))
            _deerMatingQueue.Remove(agentSocial);
        else if (agentSocial.gameObject.CompareTag("Wolf"))
            _wolvesMatingQueue.Remove(agentSocial);
    }
    

    public List<AgentHealth> GetPreyList()
    {
        return _deer.Select(d => d.GetComponent<AgentHealth>()).ToList();
    }
    
    
    
    

}
