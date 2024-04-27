using UnityEngine;

public class ScreenControl:CameraControl
{
    protected override void ChangeGameZoom()
    {
        var touch = GetTouchInput();
        if (touch.phase != TouchPhase.Moved) return;

        var deltaPos = touch.deltaPosition;
        var moveDirection = deltaPos.y * _zoomSpeedForTouchScreen * Time.deltaTime;
        transform.position += transform.forward * moveDirection;
    }

    protected override void ChangeOrbitCamera()
    {
        var touch = GetTouchInput();
        switch (touch.phase)
        {
            case TouchPhase.Moved:
                RotateCamera(Devices.TouchScreen);
                break;
            case TouchPhase.Ended:
                //   ApplyInertiaForTouchScreen();
                break;
        }
    }
}