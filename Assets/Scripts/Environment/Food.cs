using System;
using System.Collections;
using UnityEngine;

public class Food : MonoBehaviour
{
    public bool IsMushroom;

    private Action _callback;


    public void Start()
    {
        FoodManager.Instance.RegisterFood(this);
    }

    public bool IsAvailable()
    {
        return gameObject.activeSelf;
    }
    public void Eat(Action callback)
    {
        _callback = callback;
        StartCoroutine(FinishEat());
    }

    private IEnumerator FinishEat()
    {
        yield return new WaitForSeconds(4f);
        if (IsAvailable())
        {
            _callback?.Invoke();

            gameObject.SetActive(false);
        }
    }

}