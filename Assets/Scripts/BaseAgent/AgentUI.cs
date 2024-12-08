using UnityEngine;
using UnityEngine.UI;

public class AgentUI : MonoBehaviour
{
    [SerializeField] private Slider _hungerBar;
    [SerializeField] private Slider _reproductionBar;

    [Header("Selection Indicator")] [SerializeField]
    private GameObject _selectionIndicatorPrefab;

    [SerializeField] private Vector3 _selectionIndicatorOffset = new Vector3(0, 0.1f, 0); // Slight offset above ground

    private GameObject _selectionIndicator;
    private UIManager _uiManager;
    private Camera _mainCamera;

    private AgentHunger _agentHunger;
    private AgentReproduction _agentReproduction;

    private float _maxHunger, _maxReproduction;

    private void Awake()
    {
        _agentHunger = GetComponent<AgentHunger>();
        _agentReproduction = GetComponent<AgentReproduction>();

        SetupSelectionIndicator();
        _maxHunger = GetComponent<AgentInfo>().MaxHunger;
        _hungerBar.maxValue = _maxHunger;
        _hungerBar.value = 0f;
        _maxReproduction = GetComponent<AgentInfo>().MaxReproduction;
        _reproductionBar.maxValue = _maxReproduction;
        _reproductionBar.value = 0f;
    }

    public void Start()
    {
        _mainCamera = Camera.main;
        _uiManager = FindAnyObjectByType<UIManager>();
        _agentHunger.onHungerUpdated += (hunger) => _hungerBar.value = _maxHunger - hunger;
        _agentReproduction.OnReproductionUpdated += (rep) => _reproductionBar.value = _maxReproduction - rep;
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


        var rotation = Quaternion.LookRotation(-_mainCamera.transform.forward);
        _hungerBar.transform.parent.rotation = rotation;
        _reproductionBar.transform.parent.rotation = rotation;
    }


    private void UpdateSelectionIndicator()
    {
        if (_selectionIndicator is not null && _selectionIndicator.activeSelf)
        {
            _selectionIndicator.transform.Rotate(Vector3.up, 90f * Time.deltaTime);
        }
    }

    private void OnMouseDown()
    {
        _uiManager.OnEntitySelected(this);
    }

    public void SetSelected(bool selected) => _selectionIndicator?.SetActive(selected);
}