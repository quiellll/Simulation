using UnityEngine;

public class AgentAnimator : MonoBehaviour
{
    private Animator _animator;
    const string animationPrefix = "AnimalArmature|";

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetIdle()
    {
        _animator.Play(animationPrefix + "Idle");
    }

    public void SetWalk()
    {
        _animator.Play(animationPrefix + "Walk");
    }

    public void SetRun()
    {
        _animator.Play(animationPrefix + "Gallop");
    }

    public void SetEat()
    {
        _animator.Play(animationPrefix + "Eating");
    }

    public void SetDeath()
    {
        _animator.Play(animationPrefix + "Death");
    }

    public void SetAttack()
    {
        _animator.Play(animationPrefix + "Attack");
    }
}