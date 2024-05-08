using UnityEngine;

public class TouchZoomControl : ZoomControlBase
{
    [SerializeField] private float _zoomSpeedForTouchScreen = 3f;
    private float _prevTouchDistance;

    protected override float GetDeltaZoom()
    {
        if (Input.touchCount != 2) return 0f;

        var touch1 = Input.GetTouch(0);
        var touch2 = Input.GetTouch(1);

        var currentDistance = Vector2.Distance(touch1.position, touch2.position);
        if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
        {
            _prevTouchDistance = currentDistance;
            return 0f;
        }

        var deltaZoom = currentDistance - _prevTouchDistance;
        _prevTouchDistance = currentDistance;

        return deltaZoom * _zoomSpeedForTouchScreen;
    }
}