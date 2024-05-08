using UnityEngine;

public abstract class CameraControlBase : MonoBehaviour
{
    protected Transform Target;
    [SerializeField] private CheckerBoard _checkerBoard;
    [SerializeField] protected float Smoothness = 0.5f;
    
    protected virtual void Start()
    {
        Target = new GameObject("Target").transform;
        Target.position = _checkerBoard.CalculateCenterOfDesk();
    }
}