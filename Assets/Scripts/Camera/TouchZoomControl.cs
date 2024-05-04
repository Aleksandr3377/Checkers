using UnityEngine;

public class TouchZoomControl:ZoomControlBase
{
    [SerializeField] private float _zoomSpeedForTouchScreen = 3f;
    
    protected override float GetDeltaZoom()
    {
        var touch = GetTouchInput();
        if (touch.phase != TouchPhase.Moved) return 0f;
    
        var deltaPos = touch.deltaPosition;
        var moveDirection = deltaPos.y * _zoomSpeedForTouchScreen * Time.deltaTime;
        transform.position += transform.forward * moveDirection;
        return moveDirection;
    }
    
    
    // protected override void RotateCamera()
    // {
    //     var touch = GetTouchInput();
    //     var devicePos = touch.position;
    //     _deltaTouchPos = devicePos - _previousTouchPos;
    //     _previousTouchPos = devicePos;
    //     transform.RotateAround(_target.position, Vector3.up,
    //         _deltaTouchPos.x * _rotationSpeed * Time.deltaTime);
    //     transform.RotateAround(_target.position, transform.right,
    //         -_deltaTouchPos.y * _rotationSpeed * Time.deltaTime);
    // }
}