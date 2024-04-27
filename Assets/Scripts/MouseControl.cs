using UnityEditor.Rendering;
using UnityEngine;

public class MouseControl:CameraControl
{
    protected override void ChangeGameZoom()
    {
        var scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");
        Debug.Log($"{Time.frameCount} scrollInput = {scrollWheelInput}");
        var deltaPos = transform.forward * (scrollWheelInput * _zoomSpeedForMouse * Time.deltaTime);
        Debug.Log($"{Time.frameCount} deltaPos =  {deltaPos}");
        var nextPos = transform.position + deltaPos;
        Debug.Log($"{Time.frameCount} nextPos =  {nextPos}");
        var nextDistanceToTarget = Vector3.Distance(_target.position, nextPos);
        Debug.Log($"{Time.frameCount} nextDistance =  {nextDistanceToTarget}"); 
        var clampedDistance = Mathf.Clamp(nextDistanceToTarget, _minZoom, _maxZoom);
        nextPos = _target.position - transform.forward * clampedDistance;
        transform.position = nextPos;
    }

    protected override void ChangeOrbitCamera()
    {
        if (Input.GetMouseButton(0))
        {
            RotateCamera(Devices.Mouse);
        }
        else
        {
            // ApplyInertiaForMouse();
        }
    }
}