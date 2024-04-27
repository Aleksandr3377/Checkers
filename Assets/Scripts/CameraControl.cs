using System;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public abstract class CameraControl : MonoBehaviour
{
    protected Transform _target;
    [SerializeField] protected float _zoomSpeedForMouse = 10f;
    [SerializeField] protected float _zoomSpeedForTouchScreen = 3f;
    [SerializeField] private float _rotationSpeed = 2f;
    [SerializeField] private CheckerBoard _checkerBoard;
  [SerializeField]  protected float _minZoom = 1.0f;
  [SerializeField]  protected float _maxZoom = 10.0f;
    private Vector2 _previousTouchPos;
    private Vector2 _deltaTouchPos;
    private Vector3 _previousMousePos;
    private Vector3 _deltaMousePos;

    private void Awake()
    {
        _target = new GameObject("Target").transform;
    }
    
    private void Start()
    {
        _target.position = _checkerBoard.CalculateCenterOfDesk();
    }

    private void Update()
    {
        CheckIfPlayerMovedCamera();
        ChangeGameZoom();
    }

    private void CheckIfPlayerMovedCamera()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _previousMousePos = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(0)) return;

        ChangeOrbitCamera();

        if (Input.mouseScrollDelta.y != 0)
        {
            // ChangeGameZoomByMouse();
        }
    }

    protected abstract void ChangeGameZoom();
    
    protected abstract void ChangeOrbitCamera();

    protected void RotateCamera(Devices device)
    {
        switch (device)
        {
            case Devices.Mouse:
            {
                var inputPos = Input.mousePosition;
                _deltaMousePos = inputPos - _previousMousePos;
                _previousMousePos = inputPos;
                transform.RotateAround(_target.position, Vector3.up,
                    _deltaMousePos.x * _rotationSpeed * Time.deltaTime);
                transform.RotateAround(_target.position, transform.right,
                    -_deltaMousePos.y * _rotationSpeed * Time.deltaTime);
                break;
            }
            case Devices.TouchScreen:
            {
                var touch = GetTouchInput();
                var devicePos = touch.position;
                _deltaTouchPos = devicePos - _previousTouchPos;
                _previousTouchPos = devicePos;
                transform.RotateAround(_target.position, Vector3.up,
                    _deltaTouchPos.x * _rotationSpeed * Time.deltaTime);
                transform.RotateAround(_target.position, transform.right,
                    -_deltaTouchPos.y * _rotationSpeed * Time.deltaTime);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(device), device, null);
        }
    }

    // private void ApplyInertiaForTouchScreen()
    // {
    //     if (_deltaTouchPos == Vector2.zero) return;
    //     
    //     var sweepDirection = _deltaTouchPos.normalized;
    //     var swipeMagnitude = _deltaTouchPos.magnitude;
    //     var inertia = new Vector3(sweepDirection.x, 0f, sweepDirection.y) * (swipeMagnitude * _damping);
    //     transform.position += inertia * Time.deltaTime;
    //     _deltaTouchPos -= _deltaTouchPos * (_damping * Time.deltaTime);
    //     _deltaTouchPos = Vector2.ClampMagnitude(_deltaTouchPos, 10f);
    //     if (_deltaTouchPos.magnitude < 0.1f)
    //         _deltaTouchPos = Vector2.zero;
    // }

    // private void ApplyInertiaForMouse()
    //  {
    //      if (_deltaMousePos == Vector3.zero) return;
    //      
    //      var inertia = _deltaMousePos * _damping;
    //      transform.position += inertia * Time.deltaTime;
    //          
    //      _deltaMousePos -= _deltaMousePos * _damping * Time.deltaTime;
    //      if (_deltaMousePos.magnitude < 0.1f)
    //          _deltaMousePos = Vector3.zero;
    //  }

    protected Touch GetTouchInput()
    {
        return Input.touchCount > 0 ? Input.GetTouch(0) : new Touch();
    }
}