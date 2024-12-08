using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class EnvironmentObjectPlacer : EditorWindow
{
    private List<GameObject> _prefabs = new List<GameObject>();
    private Transform _parent;
    private float _radius = 50f;
    private float _innerRadius = 10f; // Minimum distance from center
    private int _count = 100;
    private float _minDistance = 2f;
    private float _clusterProbability = 0.3f;
    private int _objectsPerCluster = 3;
    private Vector2 _scaleRange = new Vector2(0.8f, 1.2f);
    private Vector3 _rotationRange = new Vector3(0f, 360f, 0f);
    private Vector2 _scrollPosition;

    [MenuItem("Tools/Environment Object Placer")]
    public static void ShowWindow()
    {
        GetWindow<EnvironmentObjectPlacer>("Object Placer");
    }

    private void OnGUI()
    {
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
        
        EditorGUILayout.LabelField("Environment Object Placer", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        DrawBasicSettings();
        DrawPrefabList();
        DrawButtons();

        EditorGUILayout.EndScrollView();
    }

    private void DrawBasicSettings()
    {
        _parent = EditorGUILayout.ObjectField("Parent Transform", _parent, typeof(Transform), true) as Transform;
        _radius = EditorGUILayout.FloatField("Spawn Radius", _radius);
        _innerRadius = EditorGUILayout.FloatField("Inner Radius (Empty Space)", _innerRadius);
        _count = EditorGUILayout.IntField("Object Count", _count);
        _minDistance = EditorGUILayout.FloatField("Minimum Distance Between Objects", _minDistance);
        _clusterProbability = EditorGUILayout.Slider("Cluster Probability", _clusterProbability, 0f, 1f);
        _objectsPerCluster = EditorGUILayout.IntField("Objects Per Cluster", _objectsPerCluster);
        _scaleRange = EditorGUILayout.Vector2Field("Scale Range (Min, Max)", _scaleRange);
        _rotationRange = EditorGUILayout.Vector3Field("Rotation Range (X,Y,Z)", _rotationRange);
    }

    private void DrawPrefabList()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Prefabs", EditorStyles.boldLabel);

        for (int i = 0; i < _prefabs.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            _prefabs[i] = EditorGUILayout.ObjectField($"Prefab {i + 1}", _prefabs[i], typeof(GameObject), false) as GameObject;
            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                _prefabs.RemoveAt(i);
                i--;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Prefab Slot"))
        {
            _prefabs.Add(null);
        }
    }

    private void DrawButtons()
    {
        EditorGUILayout.Space();
        GUI.enabled = _parent != null && _prefabs.Count > 0 && _prefabs.TrueForAll(p => p != null);
        
        if (GUILayout.Button("Place Objects"))
        {
            PlaceObjects();
        }
        if (GUILayout.Button("Clear Objects"))
        {
            ClearObjects();
        }
        
        GUI.enabled = true;
    }

    private void PlaceObjects()
    {
        if (_innerRadius >= _radius)
        {
            EditorUtility.DisplayDialog("Error", "Inner radius must be smaller than spawn radius", "OK");
            return;
        }

        Undo.RegisterFullObjectHierarchyUndo(_parent.gameObject, "Place Environment Objects");
        List<Vector3> placedPositions = new List<Vector3>();

        for (int i = 0; i < _count; i++)
        {
            Vector3 position = GetRandomPosition(placedPositions);
            if (position != Vector3.zero) // Valid position found
            {
                if (Random.value < _clusterProbability)
                {
                    CreateCluster(position, placedPositions);
                }
                else
                {
                    CreateSingleObject(position);
                    placedPositions.Add(position);
                }
            }
        }
    }

    private Vector3 GetRandomPosition(List<Vector3> placedPositions, int maxAttempts = 30)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            // Use square root for better radial distribution
            float randomRadius = Mathf.Sqrt(Random.Range(_innerRadius * _innerRadius, _radius * _radius));
            float randomAngle = Random.Range(0f, 360f);
            
            Vector3 position = new Vector3(
                randomRadius * Mathf.Cos(randomAngle * Mathf.Deg2Rad),
                0f,
                randomRadius * Mathf.Sin(randomAngle * Mathf.Deg2Rad)
            );

            // Check minimum distance from other objects
            bool tooClose = false;
            foreach (Vector3 placedPos in placedPositions)
            {
                if (Vector3.Distance(position, placedPos) < _minDistance)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
                return position;
        }

        return Vector3.zero; // No valid position found
    }

    private void CreateCluster(Vector3 centerPosition, List<Vector3> placedPositions)
    {
        placedPositions.Add(centerPosition);
        CreateSingleObject(centerPosition);

        for (int i = 1; i < _objectsPerCluster; i++)
        {
            float angle = Random.Range(0f, 360f);
            float distance = Random.Range(_minDistance * 0.5f, _minDistance * 0.8f);
            Vector3 offset = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;
            Vector3 position = centerPosition + offset;

            // Only place if within radius bounds
            if (position.magnitude <= _radius && position.magnitude >= _innerRadius)
            {
                CreateSingleObject(position);
                placedPositions.Add(position);
            }
        }
    }

    private void CreateSingleObject(Vector3 position)
    {
        GameObject prefab = _prefabs[Random.Range(0, _prefabs.Count)];
        GameObject obj = PrefabUtility.InstantiatePrefab(prefab, _parent) as GameObject;
        
        obj.transform.position = position;
        obj.transform.rotation = Quaternion.Euler(
            Random.Range(-_rotationRange.x, _rotationRange.x),
            Random.Range(-_rotationRange.y, _rotationRange.y),
            Random.Range(-_rotationRange.z, _rotationRange.z)
        );
        
        float scale = Random.Range(_scaleRange.x, _scaleRange.y);
        obj.transform.localScale = Vector3.one * scale;
    }

    private void ClearObjects()
    {
        if (_parent == null) return;
        
        Undo.RegisterFullObjectHierarchyUndo(_parent.gameObject, "Clear Environment Objects");
        
        while (_parent.childCount > 0)
        {
            DestroyImmediate(_parent.GetChild(0).gameObject);
        }
    }
}