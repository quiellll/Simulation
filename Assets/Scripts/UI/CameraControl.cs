using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _movementSpeed = 10f;
    [SerializeField] private float _fastMovementMultiplier = 2f;
    [SerializeField] private float _smoothMovementTime = 0.2f;

    [Header("Rotation Settings")]
    [SerializeField] private float _rotationSpeed = 100f;
    [SerializeField] private float _smoothRotationTime = 0.1f;

    [Header("Orthographic Zoom Settings")]
    [SerializeField] private float _focusedSize = 20f;
    [SerializeField] private float _mediumSize = 60f;
    [SerializeField] private float _fullSize = 100f;
    [SerializeField] private float _smoothZoomTime = 0.2f;

    [Header("Bounds Settings")]
    [SerializeField] private Vector2 _mapSize = new Vector2(100f, 100f);
    [SerializeField] private Transform _mapCenter;

    private Camera _camera;
    private Vector3 _currentVelocity;
    private Vector3 _smoothedPosition;
    private float _currentRotationVelocity;
    private float _smoothedYRotation;
    private float _currentZoomVelocity;
    private float _smoothedOrthoSize;
    private int _currentZoomLevel = 1; // 0 = focused, 1 = medium, 2 = full
    private Vector3 _initialRotation;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        if (_camera == null)
        {
            Debug.LogError("Camera component not found!");
            enabled = false;
            return;
        }

        if (!_camera.orthographic)
        {
            Debug.LogError("Camera must be orthographic!");
            enabled = false;
            return;
        }

        _initialRotation = transform.eulerAngles;
        _smoothedPosition = transform.position;
        _smoothedYRotation = _initialRotation.y;
        _smoothedOrthoSize = _mediumSize;
        _camera.orthographicSize = _mediumSize;
        
        if (_mapCenter == null)
        {
            Debug.LogWarning("Map center not set! Creating one at world origin.");
            GameObject centerObj = new GameObject("Map Center");
            _mapCenter = centerObj.transform;
            _mapCenter.position = Vector3.zero;
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
        ApplyTransformation();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();
        
        Vector3 right = transform.right;
        right.y = 0f;
        right.Normalize();

        Vector3 targetDirection = forward * vertical + right * horizontal;

        if (targetDirection != Vector3.zero)
        {
            float currentSpeed = _movementSpeed;
            if (Input.GetKey(KeyCode.LeftShift))
                currentSpeed *= _fastMovementMultiplier;

            Vector3 movement = targetDirection.normalized * (currentSpeed * Time.unscaledDeltaTime);
            _smoothedPosition += movement;
            
            float halfWidth = _mapSize.x * 0.5f;
            float halfLength = _mapSize.y * 0.5f;
            float centerX = _mapCenter.position.x;
            float centerZ = _mapCenter.position.z;

            _smoothedPosition.x = Mathf.Clamp(_smoothedPosition.x, centerX - halfWidth, centerX + halfWidth);
            _smoothedPosition.z = Mathf.Clamp(_smoothedPosition.z, centerZ - halfLength, centerZ + halfLength);
        }
    }

    private void HandleRotation()
    {
        var rotation = 0f;
        if (Input.GetKey(KeyCode.Q)) rotation -= 1f;
        if (Input.GetKey(KeyCode.E)) rotation += 1f;

        var targetRotation = _smoothedYRotation;
        if (rotation != 0f)
        {
            targetRotation += rotation * _rotationSpeed * Time.unscaledDeltaTime;
        }

        _smoothedYRotation = Mathf.SmoothDampAngle(
            _smoothedYRotation, 
            targetRotation, 
            ref _currentRotationVelocity, 
            _smoothRotationTime
        );
    }

    private void HandleZoom()
    {
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        
        if (scroll != 0f)
        {
            _currentZoomLevel = scroll > 0f ? Mathf.Max(0, _currentZoomLevel - 1) : Mathf.Min(2, _currentZoomLevel + 1);
        }

        var targetSize = _currentZoomLevel switch
        {
            0 => _focusedSize,
            1 => _mediumSize,
            2 => _fullSize,
            _ => _mediumSize
        };

        _smoothedOrthoSize = Mathf.SmoothDamp(
            _smoothedOrthoSize, 
            targetSize, 
            ref _currentZoomVelocity, 
            _smoothZoomTime
        );
    }

    private void ApplyTransformation()
    {
        var targetPosition = Vector3.SmoothDamp(
            transform.position,
            new Vector3(_smoothedPosition.x, transform.position.y, _smoothedPosition.z),
            ref _currentVelocity,
            _smoothMovementTime
        );
        
        transform.position = targetPosition;
        transform.rotation = Quaternion.Euler(
            _initialRotation.x,
            _smoothedYRotation,
            _initialRotation.z
        );
        
        _camera.orthographicSize = _smoothedOrthoSize;
    }
}