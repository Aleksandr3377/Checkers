using UnityEngine;

public class MouseControl : CameraControl
{
    [SerializeField] private float _rotationSpeed = 2f;
    
    protected override float GetDeltaZoom()
    {
        var zoomValue = Input.GetAxis("Mouse ScrollWheel");
        return zoomValue;
    }

    protected override void RotateCamera()
    {
        var inputPos = Input.mousePosition;
        _deltaMousePos = inputPos - _previousMousePos;
        _previousMousePos = inputPos;
        // var prevPos = transform.position;
        // var prevRotation = transform.rotation;
        transform.RotateAround(Target.position, Vector3.up,
            _deltaMousePos.x * _rotationSpeed * Time.deltaTime);
        transform.RotateAround(Target.position, transform.right,
            -_deltaMousePos.y * _rotationSpeed * Time.deltaTime);
        FinalePosition = transform.position;
        // FinaleRotation = transform.rotation;
        // transform.position = prevPos;
        // transform.rotation = prevRotation;
    }
}