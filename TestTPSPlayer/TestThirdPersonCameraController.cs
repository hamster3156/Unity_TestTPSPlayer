using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestThirdPersonCameraController : MonoBehaviour
{
    [SerializeField]
    private float _zoomSpd = 2f;

    [SerializeField]
    private float _zoomLerpSpd = 10f;

    [SerializeField]
    private float _minDis = 3f;

    [SerializeField]
    private float _maxDis = 15f;

    private InputControls _inputControls;
    private CinemachineCamera _camera;
    private CinemachineOrbitalFollow _orbitalFollow;
    private Vector2 _scrollDelta;

    private float _targetZoom;
    private float _currentZoom;

    private void Start()
    {
        _inputControls = new();
        _inputControls.Enable();

        _inputControls.CameraControls.MouseZoom.performed += HandleMouseScroll;

        Cursor.lockState = CursorLockMode.Locked;
        _camera = GetComponent<CinemachineCamera>();
        _orbitalFollow = _camera.GetComponent<CinemachineOrbitalFollow>();

        _targetZoom = _orbitalFollow.Radius;
    }

    private void HandleMouseScroll(InputAction.CallbackContext context)
    {
        _scrollDelta = context.ReadValue<Vector2>();
        //Debug.Log($"Scroll Delta: {_scrollDelta}");
    }

    private void Update()
    {
        if (_scrollDelta.y != 0)
        {
            if (_orbitalFollow != null)
            {
                _targetZoom = Mathf.Clamp(_orbitalFollow.Radius - _scrollDelta.y * _zoomSpd, _minDis, _maxDis);
                _scrollDelta = Vector2.zero;
            }
        }

        var bumperDelta = _inputControls.CameraControls.GamepadZoom.ReadValue<float>();

        if (bumperDelta != 0)
        {
            _targetZoom = Mathf.Clamp(_orbitalFollow.Radius - bumperDelta * _zoomSpd, _minDis, _maxDis);
        }

        _currentZoom = Mathf.Lerp(_currentZoom, _targetZoom, Time.deltaTime * _zoomLerpSpd);
        _orbitalFollow.Radius = _currentZoom;
    }
}
