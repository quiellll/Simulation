using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class UIManager : MonoBehaviour
{
    [Header("Time Slider")] [SerializeField]
    private Slider _timeSlider;

    [Header("Action Buttons")] [SerializeField]
    private Button _killButton;

    [SerializeField] private Button _increaseHungerButton;
    [SerializeField] private Button _decreaseHungerButton;
    [SerializeField] private Button _increaseReproductionButton;
    [SerializeField] private Button _decreaseReproductionButton;

    [Header("Environment")] [SerializeField]
    private GameObject _ground; // Reference to your plane

    private AgentUI _selectedEntity;
    private List<Button> _contextualButtons;
    private Camera _mainCamera;

    private void Awake()
    {
        _contextualButtons = new List<Button>
        {
            _killButton,
            _increaseHungerButton,
            _decreaseHungerButton,
            _increaseReproductionButton,
            _decreaseReproductionButton
        };

        _timeSlider.onValueChanged.AddListener((value) => Time.timeScale = value);

        SetContextualButtonsVisibility(false);
        
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        _killButton.onClick.AddListener(KillSelected);
        _increaseHungerButton.onClick.AddListener(IncreaseHunger);
        _decreaseHungerButton.onClick.AddListener(DecreaseHunger);
        _increaseReproductionButton.onClick.AddListener(IncreaseReproductionUrge);
        _decreaseReproductionButton.onClick.AddListener(DecreaseReproductionUrge);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return; // ignora clics en UI

            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out var hit) || !hit.collider.GetComponent<AgentUI>())
            {
                OnEntitySelected(null); // deselecciona si no encuentras un `AgentUI`
            }
        }
    }

    public void OnEntitySelected(AgentUI entity)
    {
        if (_selectedEntity == entity) return; // si ya está seleccionada, no haces nada

        if (_selectedEntity != null)
        {
            _selectedEntity.SetSelected(false); // debería desactivar el indicador de selección
        }

        _selectedEntity = entity;

        if (_selectedEntity != null)
        {
            _selectedEntity.SetSelected(true);
            Debug.Log($"Selected entity: {_selectedEntity.name}");
        }

        UpdateContextualButtons();
    }

    private void UpdateContextualButtons()
    {
        if (_selectedEntity is null)
        {
            SetContextualButtonsVisibility(false);
            return;
        }

        SetContextualButtonsVisibility(true);
    }

    private void SetContextualButtonsVisibility(bool visible)
    {
        foreach (var button in _contextualButtons.Where(button => button is not null))
        {
            button.gameObject.SetActive(visible);
        }
    }

    private void KillSelected()
    {
        if (_selectedEntity == null) return;

        _selectedEntity.GetComponent<AgentHealth>().Kill();
        OnEntitySelected(null); // centraliza la lógica de deselección
    }


    private void IncreaseHunger() => _selectedEntity?.GetComponent<AgentHunger>().IncreaseHunger();

    private void DecreaseHunger() => _selectedEntity?.GetComponent<AgentHunger>().DecreaseHunger();

    private void IncreaseReproductionUrge()
    {
        // TODO: Implement reproduction urge increase logic
    }

    private void DecreaseReproductionUrge()
    {
        // TODO: Implement reproduction urge decrease logic
    }
}