using System;
using System.Collections;
using UnityEngine;

public class Prey : MonoBehaviour
{
    private Action _callback;
    public void Start()
    {
        FoodManager.Instance.RegisterPrey(this);
    }

    public bool IsAvailable()
    {
        return gameObject.activeSelf;
    }

    public void Eat(Action callback)
    {
        // FALTA HACER QUE EL CIERVO PARE EL MOVIMIENTO Y HAGA LA ANIMACION DE MORIR
        _callback = callback;
        StartCoroutine(FinishEat());
    }

    private IEnumerator FinishEat()
    {
        yield return new WaitForSeconds(4f);
        if (!IsAvailable()) yield break;
        
        _callback?.Invoke();
        gameObject.SetActive(false);
    }
}