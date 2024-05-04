using System;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public abstract class CameraControlBase : MonoBehaviour
{
    protected Transform Target;
    [SerializeField] private CheckerBoard _checkerBoard;
    [SerializeField] protected float Smoothness = 0.5f;
    protected Vector3 FinalPosition; //todo: check if zoom only pos
    protected Vector3 PreviousMousePos; //todo:
    
    protected virtual void Start()
    {
        Target = new GameObject("Target").transform;
        Target.position = _checkerBoard.CalculateCenterOfDesk();
        FinalPosition = transform.position;
    }

    private void LateUpdate()
    {
        PreviousMousePos = Input.mousePosition; //todo:
    }

    protected Touch GetTouchInput() //todo: refactor move method to proper place
    {
        return Input.touchCount > 0 ? Input.GetTouch(0) : new Touch();
    }
}