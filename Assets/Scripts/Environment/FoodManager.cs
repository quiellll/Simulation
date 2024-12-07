using UnityEngine;
using System.Collections.Generic;

public class FoodManager : MonoBehaviour
{

    public static FoodManager Instance { get; private set; }

    private List<Food> _foods = new();
    private List<Prey> _preys = new();
    private HashSet<Food> _assignedFoods = new();

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

    public void RegisterPrey(Prey prey)
    {
        _preys.Add(prey);
    }

    public bool TryGetFood(Vector3 position, out Food food, float maxRange)
    {
        bool _allowAssignedFood = _assignedFoods.Count >= _foods.Count;
        
        food = null;
        float nearestDistance = float.MaxValue;
        foreach (var f in _foods)
        {
            if (!f.gameObject.activeSelf)
            {
                _assignedFoods.Add(f);
                continue;
            }

            if (!_allowAssignedFood && _assignedFoods.Contains(f))
            {
                continue;
            }
            
            float distance = Vector3.Distance(f.transform.position, position);
            if (distance <= maxRange && distance <= nearestDistance)
            {

                food = f;
                _assignedFoods.Add(f);
                nearestDistance = distance;
            }
        }
        return food != null;
    }

    public bool TryGetPrey(Vector3 position, out Prey prey, float maxRange)
    {
        prey = null;
        float nearestDistance = float.MaxValue;
        foreach (var p in _preys)
        {
            if(p == null || !p.gameObject.activeSelf) continue;
            float distance = Vector3.Distance(p.transform.position, position);
            if (distance <= maxRange && distance <= nearestDistance)
            {
                prey = p;
                nearestDistance = distance;
            }
        }
        return prey != null;
    }
}