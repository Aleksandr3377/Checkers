using UnityEngine;

public abstract class OrbitControlBase : CameraControlBase
{
    [SerializeField] protected float RotationSpeed = 2f;
    private float _currentRotX;
    private float _finalRotX;
    private float _finalRotY;
    private float _currentRotY;

    protected override void Start()
    {
        base.Start();
        _finalRotX = transform.eulerAngles.x;
        _finalRotY = transform.eulerAngles.y;
        _currentRotX = _finalRotX;
        _currentRotY = _finalRotY;
    }

    private void Update()
    {
        _finalRotY += GetDeltaRotY();
        var nextRotY = Mathf.Lerp(_currentRotY, _finalRotY, Time.deltaTime * Smoothness);
        var deltaRotY = nextRotY - _currentRotY;
        Debug.Log(GetDeltaRotY());
        transform.RotateAround(Target.position, Vector3.up, deltaRotY);
        _currentRotY = nextRotY;
        
        _finalRotX += GetDeltaRotX();
        var nextRotX = Mathf.Lerp(_currentRotX, _finalRotX, Time.deltaTime * Smoothness);
        var deltaRotX = nextRotX - _currentRotX;
        transform.RotateAround(Target.position, -transform.right, deltaRotX);
        _currentRotX = nextRotX;
    }

    protected abstract float GetDeltaRotX();

    protected abstract float GetDeltaRotY();
}