using UnityEngine;
using UnityEngine.UI;

public class AgentUI : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private Vector3 _healthBarOffset = new Vector3(0, 2f, 0);

    [Header("Selection Indicator")]
    [SerializeField] private GameObject _selectionIndicatorPrefab; // Assign the arrow or glow disk prefab
    [SerializeField] private Vector3 _selectionIndicatorOffset = new Vector3(0, 0.1f, 0); // Slight offset above ground

    [Header("UI References")]
    [SerializeField] private GameObject _healthBarPrefab;

    private GameObject _healthBarInstance;
    private GameObject _selectionIndicator;
    private Image _healthFillImage;
    private Canvas _worldSpaceCanvas;
    private bool _isSelected;
    private UIManager _uiManager;
    private Camera _mainCamera;

    private AgentHealth _agentHealth;

    private void Awake()
    {
        _agentHealth = GetComponent<AgentHealth>();
        _agentHealth.OnHealthChanged += OnHealthChanged;
        SetupSelectionIndicator();
    }

    public void Start()
    {
        _mainCamera = Camera.main;
        _uiManager = FindAnyObjectByType<UIManager>();
    }

    public void OnDisable()
    {
        _agentHealth.OnHealthChanged += OnHealthChanged;
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

    private void SetupHealthBar()
    {
        // Create world space canvas if not exists
        if (_worldSpaceCanvas == null)
        {
            GameObject canvasObj = new GameObject("WorldSpaceCanvas");
            _worldSpaceCanvas = canvasObj.AddComponent<Canvas>();
            _worldSpaceCanvas.renderMode = RenderMode.WorldSpace;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // Instantiate health bar
        if (_healthBarPrefab != null)
        {
            _healthBarInstance = Instantiate(_healthBarPrefab, _worldSpaceCanvas.transform);
            _healthBarInstance.transform.position = transform.position + _healthBarOffset;

            // Get fill image reference
            _healthFillImage = _healthBarInstance.transform.Find("Fill")?.GetComponent<Image>();
            OnHealthChanged();
        }
    }

    private void Update()
    {
        if (_healthBarInstance != null)
        {
            // Make health bar face camera
            _healthBarInstance.transform.LookAt(
                _healthBarInstance.transform.position + _mainCamera.transform.rotation * Vector3.forward,
                _mainCamera.transform.rotation * Vector3.up);

            // Update position to follow entity
            _healthBarInstance.transform.position = transform.position + _healthBarOffset;
        }

        // Rotate selection indicator if it's an arrow
        if (_selectionIndicator != null && _selectionIndicator.activeSelf)
        {
            _selectionIndicator.transform.Rotate(Vector3.up, 90f * Time.deltaTime); // Optional: rotate 90 degrees per second
        }
    }

    public void OnHealthChanged(float _ = 0f)
    {
        if (_healthFillImage != null)
        {
            _healthFillImage.fillAmount = _agentHealth.GetHealthPercentage();
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
}