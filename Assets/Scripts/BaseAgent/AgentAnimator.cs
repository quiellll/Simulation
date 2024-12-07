using UnityEngine;

public class AgentAnimator : MonoBehaviour
{
    private Animator _animator;
    const string animationPrefix = "AnimalArmature|";

    private string _last;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetIdle()
    {
        const string key = animationPrefix + "Idle";
        if(key == _last) return;
        _last = key;
        _animator.Play(key);
    }

    public void SetWalk()
    {
        const string key = animationPrefix + "Walk";
        if(key == _last) return;
        _last = key;
        _animator.Play(key);
    }

    public void SetRun()
    {
        const string key = animationPrefix + "Gallop";
        if(key == _last) return;
        _last = key;
        _animator.Play(key);
    }

    public void SetEat()
    {
        const string key = animationPrefix + "Eating";
        if(key == _last) return;
        _last = key;
        _animator.Play(key);
    }

    public void SetDeath()
    {
        const string key = animationPrefix + "Death";
        if(key == _last) return;
        _last = key;
        _animator.Play(key);
    }

    public void SetAttack()
    {
        _animator.Play(animationPrefix + "Attack");
    }
}