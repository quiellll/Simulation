using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("Spawn Buttons")]
    [SerializeField] private Button _spawnRabbitButton;
    [SerializeField] private Button _spawnDeerButton;
    [SerializeField] private Button _spawnFoxButton;
    [SerializeField] private Button _spawnWolfButton;
    [SerializeField] private Button _spawnPlantButton;

    [Header("Time Slider")]
    [SerializeField] private Slider _timeSlider;

    [Header("Action Buttons")]
    [SerializeField] private Button _killButton;
    [SerializeField] private Button _hibernateButton;
    [SerializeField] private Button _rabiesButton;
    [SerializeField] private Button _migrateButton;
    [SerializeField] private Button _omnivoreButton;
    [SerializeField] private Button _increaseHungerButton;
    [SerializeField] private Button _decreaseHungerButton;
    [SerializeField] private Button _increaseReproductionButton;
    [SerializeField] private Button _decreaseReproductionButton;

    [Header("Environment")]
    [SerializeField] private GameObject _ground; // Reference to your plane
    [SerializeField] private float _spawnHeight = 0.5f; // Height above ground to spawn entities

    // References to prefabs
    [Header("Prefabs")]
    [SerializeField] private GameObject _rabbitPrefab;
    [SerializeField] private GameObject _deerPrefab;
    [SerializeField] private GameObject _foxPrefab;
    [SerializeField] private GameObject _wolfPrefab;
    [SerializeField] private GameObject _plantPrefab;

    private AgentUI _selectedEntity;
    private List<Button> _contextualButtons;

    private void Awake()
    {
        // Initialize lists
        _contextualButtons = new List<Button>
        {
            _killButton,
            _hibernateButton,
            _rabiesButton,
            _migrateButton,
            _omnivoreButton,
            _increaseHungerButton,
            _decreaseHungerButton,
            _increaseReproductionButton,
            _decreaseReproductionButton
        };

        // Add listeners to spawn buttons
        _spawnRabbitButton.onClick.AddListener(() => SpawnEntity(_rabbitPrefab));
        _spawnDeerButton.onClick.AddListener(() => SpawnEntity(_deerPrefab));
        _spawnFoxButton.onClick.AddListener(() => SpawnEntity(_foxPrefab));
        _spawnWolfButton.onClick.AddListener(() => SpawnEntity(_wolfPrefab));
        _spawnPlantButton.onClick.AddListener(() => SpawnEntity(_plantPrefab));

        _timeSlider.onValueChanged.AddListener((float value) => Time.timeScale = value);

        // Hide contextual buttons initially
        SetContextualButtonsVisibility(false);
    }

    private void Start()
    {
        // Initialize basic buttons
        _killButton.onClick.AddListener(KillSelected);
        _hibernateButton.onClick.AddListener(HibernateSelected);
        _migrateButton.onClick.AddListener(StartMigration);
        _increaseHungerButton.onClick.AddListener(IncreaseHunger);
        _decreaseHungerButton.onClick.AddListener(DecreaseHunger);
        _increaseReproductionButton.onClick.AddListener(IncreaseReproductionUrge);
        _decreaseReproductionButton.onClick.AddListener(DecreaseReproductionUrge);
    }

    private void SpawnEntity(GameObject prefab)
    {
        if (prefab == null || _ground == null)
            return;

        // Get plane dimensions
        MeshRenderer groundRenderer = _ground.GetComponent<MeshRenderer>();
        if (groundRenderer == null)
            return;

        Bounds bounds = groundRenderer.bounds;

        // Calculate random position within plane bounds
        Vector3 randomPosition = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            _spawnHeight,
            Random.Range(bounds.min.z, bounds.max.z)
        );

        GameObject entity = Instantiate(prefab, randomPosition, Quaternion.identity);

    }

    public void OnEntitySelected(AgentUI entity)
    {
        if (_selectedEntity != null)
        {
            // Deselect previous entity
            if (_selectedEntity != null)
            {
                _selectedEntity.SetSelected(false);
            }
        }

        // Select new entity
        _selectedEntity = entity;
        if (entity != null)
        {
            entity.SetSelected(true);
            Debug.Log($"Selected entity: {_selectedEntity.name}");
        }

        UpdateContextualButtons();
    }

    private void UpdateContextualButtons()
    {
        if (_selectedEntity == null)
        {
            SetContextualButtonsVisibility(false);
            return;
        }

        // Show basic buttons for all entities
        SetContextualButtonsVisibility(true);

        // Show/hide special buttons based on entity type
        bool isHerbivore = IsHerbivore(_selectedEntity.gameObject);
        bool isFox = IsFox(_selectedEntity.gameObject);

        _rabiesButton.gameObject.SetActive(isHerbivore);
        _omnivoreButton.gameObject.SetActive(isFox);
    }

    private void SetContextualButtonsVisibility(bool visible)
    {
        foreach (Button button in _contextualButtons)
        {
            if (button != null)
            {
                button.gameObject.SetActive(visible);
            }
        }
    }

    // Placeholder methods for button actions
    private void KillSelected()
    {
        if (_selectedEntity != null)
        {
            Destroy(_selectedEntity);
            _selectedEntity = null;
            SetContextualButtonsVisibility(false);
        }
    }

    private void HibernateSelected()
    {
        // TODO: Implement hibernation logic
    }

    private void StartMigration()
    {
        // TODO: Implement migration logic
    }

    private void IncreaseHunger()
    {
        // TODO: Implement hunger increase logic
    }

    private void DecreaseHunger()
    {
        // TODO: Implement hunger decrease logic
    }

    private void IncreaseReproductionUrge()
    {
        // TODO: Implement reproduction urge increase logic
    }

    private void DecreaseReproductionUrge()
    {
        // TODO: Implement reproduction urge decrease logic
    }

    // Helper methods to check entity types
    private bool IsHerbivore(GameObject entity)
    {
        // TODO: Implement proper type checking
        return entity.CompareTag("Rabbit") || entity.CompareTag("Deer");
    }

    private bool IsFox(GameObject entity)
    {
        // TODO: Implement proper type checking
        return entity.CompareTag("Fox");
    }

    private void OnDrawGizmos()
    {
        if (_ground == null)
            return;

        // Get plane bounds
        MeshRenderer groundRenderer = _ground.GetComponent<MeshRenderer>();
        if (groundRenderer == null)
            return;

        Bounds bounds = groundRenderer.bounds;

        // Draw spawn area outline matching plane bounds
        Gizmos.color = new Color(1f, 1f, 0f, 0.5f); // Semi-transparent yellow

        // Draw the box slightly above ground to prevent z-fighting
        Vector3 center = bounds.center + Vector3.up * 0.01f;
        Vector3 size = new Vector3(bounds.size.x, 0.02f, bounds.size.z);

        Gizmos.DrawWireCube(center, size);

        // Optionally draw corner markers for better visibility
        float cornerSize = 0.5f;
        Gizmos.color = Color.yellow;

        Vector3[] corners = new Vector3[]
        {
            new Vector3(bounds.min.x, _spawnHeight, bounds.min.z),
            new Vector3(bounds.min.x, _spawnHeight, bounds.max.z),
            new Vector3(bounds.max.x, _spawnHeight, bounds.min.z),
            new Vector3(bounds.max.x, _spawnHeight, bounds.max.z)
        };

        foreach (Vector3 corner in corners)
        {
            Gizmos.DrawWireSphere(corner, cornerSize);
        }
    }
}