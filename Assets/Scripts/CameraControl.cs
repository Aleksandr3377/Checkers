using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public abstract class CameraControl : MonoBehaviour
{
    protected Transform Target;
    [SerializeField] private float _zoomSpeed = 1000f;
    [SerializeField] private CheckerBoard _checkerBoard;
    [SerializeField] protected float MinZoom = 1.0f;
    [SerializeField] protected float MaxZoom = 10.0f;
    [SerializeField] private float _smoothness = 0.5f;
    private Vector2 _previousTouchPos;
    private Vector2 _deltaTouchPos;
    protected Vector3 _previousMousePos;
    protected Vector3 _deltaMousePos;
    protected Vector3 FinalePosition;
    protected Quaternion FinaleRotation;

    private void Awake()
    {
        Target = new GameObject("Target").transform;
    }

    private void Start()
    {
        Target.position = _checkerBoard.CalculateCenterOfDesk();
        FinalePosition = transform.position;
        FinaleRotation = transform.rotation;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _previousMousePos = Input.mousePosition;
            return;
        }

        if (Input.GetMouseButton(0))
        {
            RotateCamera();
        }
        
         var deltaZoom =  GetDeltaZoom();
         AddZoom(deltaZoom);
         transform.position = Vector3.Lerp(transform.position, FinalePosition, _smoothness * Time.deltaTime);
         // transform.rotation = FinaleRotation;
    }

    protected abstract float GetDeltaZoom();

    protected abstract void RotateCamera();

    private void AddZoom(float delta)
    {
        if (delta == 0)
        {
          return;  
        }
        var deltaPos = transform.forward * (delta * _zoomSpeed * Time.deltaTime);
        var nextPos = FinalePosition+ deltaPos;
        var nextDistanceToTarget = Vector3.Distance(Target.position, nextPos);
        var clampedDistance = Mathf.Clamp(nextDistanceToTarget, MinZoom, MaxZoom);
        FinalePosition = Target.position - transform.forward * clampedDistance;
    }
    
    protected Touch GetTouchInput()
    {
        return Input.touchCount > 0 ? Input.GetTouch(0) : new Touch();
    }
}