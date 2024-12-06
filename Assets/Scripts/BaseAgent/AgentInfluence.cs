using UnityEngine;

public class AgentInfluences : MonoBehaviour
{
    private bool _hasHibernateInfluence;
    private bool _hasRabiesInfluence;

    public bool CheckHibernateInfluence()
    {
        return _hasHibernateInfluence;
    }

    public bool CheckHibernateEnd()
    {
        return !_hasHibernateInfluence;
    }

    public bool CheckRabiesInfluence()
    {
        return _hasRabiesInfluence;
    }

    public void SetHibernateInfluence(bool value)
    {
        _hasHibernateInfluence = value;
    }

    public void SetRabiesInfluence(bool value)
    {
        _hasRabiesInfluence = value;
    }
}