
using System;
using UnityEngine;

public class AgentGrowth : MonoBehaviour
{
    private float _growProgress = 0f;
    private bool _isGrowing;
    private AgentInfo _info;
    

    public void InitAsChild()
    {
        _info = GetComponent<AgentInfo>();
        _isGrowing = true;
        transform.localScale = Vector3.one * _info.ChildScale;
    }


    public void Update()
    {
        if (!_isGrowing) return;

        _growProgress += Time.deltaTime / _info.ChildGrowthTime;
        var scale = Mathf.Clamp01(Mathf.Lerp(_info.ChildScale, 1f, _growProgress));
        transform.localScale = Vector3.one * scale;
        if (_growProgress > 1f) _isGrowing = false;
    }
}
