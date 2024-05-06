using UnityEngine;

public class MouseZoomControl : ZoomControlBase
{
    protected override float GetDeltaZoom()
    {
        var zoomValue = Input.GetAxis("Mouse ScrollWheel");
        return zoomValue * ZoomSpeed;
    }
}