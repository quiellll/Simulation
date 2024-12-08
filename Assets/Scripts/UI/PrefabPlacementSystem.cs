using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class PrefabPlacementSystem : MonoBehaviour
{
    [System.Serializable]
    public class PrefabPair
    {
        public GameObject MainPrefab;
        public GameObject PreviewPrefab;
        public Button AssociatedButton;
        public bool UseRandomVariants;
        public GameObject[] VariantPrefabs;
    }

    [SerializeField] private PrefabPair[] _prefabAndPreview;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Camera _mainCamera;

    private GameObject _currentPreview;
    private PrefabPair _currentPair;
    private bool _isPlacementMode = false;

    
    private void Start()
    {
        if (_mainCamera == null)
            _mainCamera = Camera.main;

        foreach (var pair in _prefabAndPreview)
        {
            pair.AssociatedButton.onClick.AddListener(() => SelectPrefab(pair));
        }
    }
    
    private void SelectPrefab(PrefabPair pair)
    {
        if (_currentPreview != null)
            Destroy(_currentPreview);

        _currentPair = pair;
        _currentPreview = Instantiate(pair.PreviewPrefab);
        _isPlacementMode = true;
    }
    
    private void Update()
    {
        if (!_isPlacementMode || _currentPreview is null)
            return;

        var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, _groundLayer))
        {
            if (NavMesh.SamplePosition(hit.point, out var navHit, 5f, NavMesh.AllAreas))
            {
                _currentPreview.transform.position = navHit.position;

                if (Input.GetMouseButtonDown(0))
                {
                    SpawnPrefab(navHit.position);
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
        }
    }
    
    private void SpawnPrefab(Vector3 position)
    {
        GameObject prefabToSpawn;

        if (_currentPair.UseRandomVariants && _currentPair.VariantPrefabs.Length > 0)
        {
            var randomIdx = Random.Range(0, _currentPair.VariantPrefabs.Length);
            prefabToSpawn = _currentPair.VariantPrefabs[randomIdx];
        }
        else
        {
            prefabToSpawn = _currentPair.MainPrefab;
        }
        
        var randomRot = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
        
        Instantiate(prefabToSpawn, position, randomRot);
        CancelPlacement();
    }
    
    private void CancelPlacement()
    {
        if (_currentPreview is not null)
            Destroy(_currentPreview);

        _isPlacementMode = false;
        _currentPreview = null;
        _currentPair = null;
    }
    
    private void OnDisable()
    {
        CancelPlacement();
    }
}