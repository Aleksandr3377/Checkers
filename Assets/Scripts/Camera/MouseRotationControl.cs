using UnityEngine;

public class MouseRotationControl : OrbitControlBase
{
    [SerializeField] private float _maxRotationX = 120f;

    protected override float GetDeltaRotX()
    {
        var deltaRotX = GetMouseDelta().y * RotationSpeed;
        var nextRotX = FinalRotX + deltaRotX;
        if (nextRotX > _maxRotationX)
        {
            deltaRotX = _maxRotationX - FinalRotX;
        }
        else if (nextRotX < -_maxRotationX)
        {
            deltaRotX = _maxRotationX - FinalRotX;
        }

        return deltaRotX;
    }

    protected override float GetDeltaRotY()
    {
        return GetMouseDelta().x * RotationSpeed;
    }

    private Vector2 GetMouseDelta()
    {
        if (!Input.GetMouseButton(0) || Input.GetMouseButtonDown(0) || Input.touchCount > 1)
        {
            return Vector2.zero;
        }

        var inputPos = Input.mousePosition;
        var deltaMousePos = inputPos - PreviousMousePos;
        return deltaMousePos;
    }
}