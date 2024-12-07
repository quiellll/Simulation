using UnityEngine;
using UnityEngine.UI;

public class AgentUI : MonoBehaviour
{
    [SerializeField] private Slider _hungerBar;
    
    [Header("Selection Indicator")] [SerializeField]
    private GameObject _selectionIndicatorPrefab;
    [SerializeField] private Vector3 _selectionIndicatorOffset = new Vector3(0, 0.1f, 0); // Slight offset above ground

    private GameObject _selectionIndicator;
    private bool _isSelected;
    private UIManager _uiManager;
    private Camera _mainCamera;

    private AgentHunger _agentHunger;

    private float _maxHunger;

    private void Awake()
    {
        _agentHunger = GetComponent<AgentHunger>();

        SetupSelectionIndicator();
        _maxHunger = GetComponent<AgentInfo>().MaxHunger;
        _hungerBar.maxValue = _maxHunger;
        _hungerBar.value = 0f;
    }

    public void Start()
    {
        _mainCamera = Camera.main;
        _uiManager = FindAnyObjectByType<UIManager>();
        GetComponent<AgentHunger>().onHungerUpdated += (hunger) => _hungerBar.value = _maxHunger - hunger;
    }

    private void SetupSelectionIndicator()
    {
        if (_selectionIndicatorPrefab != null)
        {
            Debug.Log($"Creating selection indicator for {gameObject.name}"); // Debug setup
            _selectionIndicator = Instantiate(_selectionIndicatorPrefab, transform);
            _selectionIndicator.transform.localPosition = _selectionIndicatorOffset;
            _selectionIndicator.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Selection indicator prefab not assigned!"); // Debug missing prefab
        }
    }
    
    private void Update()
    {
        UpdateSelectionIndicator();
        
        
        // _hungerBar.transform.parent.LookAt(_mainCamera.transform.position);
        _hungerBar.transform.parent.rotation = Quaternion.LookRotation(-_mainCamera.transform.forward);
    }



    private void UpdateSelectionIndicator()
    {
        if (_selectionIndicator != null && _selectionIndicator.activeSelf)
        {
            _selectionIndicator.transform.Rotate(Vector3.up, 90f * Time.deltaTime);
        }
    }

    public void ToggleSelectionIndicator()
    {
        if (_selectionIndicator != null)
        {
            _selectionIndicator.SetActive(!_selectionIndicator.activeSelf);
        }
        else
        {
            Debug.Log($"Selection indicator is null on {gameObject.name}"); // Debug missing indicator
        }
    }
    
    private void OnMouseDown()
    {
        _uiManager.OnEntitySelected(this);
    }

    public void SetSelected(bool selected)
    {
        _isSelected = selected;
        Debug.Log($"SetSelected called on {gameObject.name} with value: {selected}"); // Debug selection state

        if (_selectionIndicator != null)
        {
            Debug.Log($"Selection indicator exists, setting active to: {selected}"); // Debug indicator
            _selectionIndicator.SetActive(selected);
        }
        else
        {
            Debug.LogWarning("Selection indicator is null!"); // Debug missing indicator
        }
    }

    private void OnDestroy()
    {

    }
}