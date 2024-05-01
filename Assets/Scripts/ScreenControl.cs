using UnityEngine;

public class ScreenControl:CameraControl
{
    [SerializeField] private float _zoomSpeedForTouchScreen = 3f;
    
    // protected override void UpdateZoom()
    // {
    //     var touch = GetTouchInput();
    //     if (touch.phase != TouchPhase.Moved) return;
    //
    //     var deltaPos = touch.deltaPosition;
    //     var moveDirection = deltaPos.y * _zoomSpeedForTouchScreen * Time.deltaTime;
    //     transform.position += transform.forward * moveDirection;
    // }

    protected override float GetDeltaZoom()
    {
        throw new System.NotImplementedException();
    }

    protected override void RotateCamera()
    {
        var touch = GetTouchInput();
        switch (touch.phase)
        {
            case TouchPhase.Moved:
                RotateCamera();
                break;
            case TouchPhase.Ended:
                //   ApplyInertiaForTouchScreen();
                break;
        }
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