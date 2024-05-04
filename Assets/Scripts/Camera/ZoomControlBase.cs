using UnityEngine;

public abstract class ZoomControlBase:CameraControlBase
{
    [SerializeField] private float _zoomSpeed = 1000f;
    [SerializeField] protected float MinZoom = 1.0f;
    [SerializeField] protected float MaxZoom = 10.0f; 
    
    private void Update()
    {
        var deltaZoom =  GetDeltaZoom(); 
        AddZoom(deltaZoom); 
        transform.position = Vector3.Lerp(transform.position, FinalPosition, Smoothness * Time.deltaTime); //todo: test together simultaneously
    }
    
    private void AddZoom(float delta)
    {
        if (delta == 0)
        {
            return;  
        }
        
        var deltaPos = transform.forward * (delta * _zoomSpeed * Time.deltaTime);
        var nextPos = FinalPosition + deltaPos;
        var nextDistanceToTarget = Vector3.Distance(Target.position, nextPos);
        var clampedDistance = Mathf.Clamp(nextDistanceToTarget, MinZoom, MaxZoom);
        FinalPosition = Target.position - transform.forward * clampedDistance;
    }
    
    protected abstract float GetDeltaZoom();
  
}