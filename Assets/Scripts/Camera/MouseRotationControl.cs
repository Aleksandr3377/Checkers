using UnityEngine;

public class MouseRotationControl : OrbitControlBase
{
    // protected override void RotateCamera()
    // {
    //     var inputPos = Input.mousePosition;
    //    // DeltaMousePos = inputPos - PreviousMousePos;
    //     PreviousMousePos = inputPos;
    //     // var prevPos = transform.position;
    //     // var prevRotation = transform.rotation;
    //     transform.RotateAround(Target.position, Vector3.up,
    //      //   DeltaMousePos.x * RotationSpeed * Time.deltaTime);
    //     transform.RotateAround(Target.position, transform.right,
    //      //   -DeltaMousePos.y * RotationSpeed * Time.deltaTime);
    //    // TargetPosition = transform.position;
    //     // FinaleRotation = transform.rotation;
    //     // transform.position = prevPos;
    //     // transform.rotation = prevRotation;
    // }

    protected override float GetDeltaRotX()
    {
        return GetMouseDelta().y * RotationSpeed;
    }
    
    protected override float GetDeltaRotY()
    {
        return GetMouseDelta().x * RotationSpeed;
    }
    
    private Vector2 GetMouseDelta()
    {
        if (!Input.GetMouseButton(0) || Input.GetMouseButtonDown(0))
        {
            return Vector2.zero;
        }

        var inputPos = Input.mousePosition;
        var deltaMousePos = inputPos - PreviousMousePos;
        return deltaMousePos;
    }
}