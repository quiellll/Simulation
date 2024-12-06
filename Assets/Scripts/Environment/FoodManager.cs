using UnityEngine;
using System.Collections.Generic;

public class FoodManager : MonoBehaviour
{

    public static FoodManager Instance { get; private set; }

    private List<Food> _foods = new();

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
    
    public void RegisterFood(Food food)
    {
        _foods.Add(food);
    }

    public bool TryGetFood(Vector3 position, out Food food, float maxRange)
    {
        food = null;
        float nearestDistance = float.MaxValue;
        foreach (var f in _foods)
        {
            if(f == null || !f.gameObject.activeSelf) continue;
            float distance = Vector3.Distance(f.transform.position, position);
            if (distance <= maxRange && distance <= nearestDistance)
            {

                food = f;
                nearestDistance = distance;
            }
        }
        return food != null;
    }
}