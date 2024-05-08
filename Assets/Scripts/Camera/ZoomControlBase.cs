using UnityEngine;

public abstract class ZoomControlBase : CameraControlBase
{
    [SerializeField] protected float ZoomSpeed = 1000f;
    [SerializeField] protected float MinZoom = 1.0f;
    [SerializeField] protected float MaxZoom = 10.0f;
    private float _finalZoom;
    private float _currentZoom;

    protected override void Start()
    {
        base.Start();
        _finalZoom = Vector3.Distance(Target.position, transform.position);
        _currentZoom = _finalZoom;
    }

    private void Update()
    {
        var deltaZoom = GetDeltaZoom();
        _finalZoom -= deltaZoom;
        _finalZoom = Mathf.Clamp(_finalZoom, MinZoom, MaxZoom);
        _currentZoom = Mathf.Lerp(_currentZoom, _finalZoom, Smoothness * Time.deltaTime);
        transform.position = Target.position + (transform.position - Target.position).normalized * _currentZoom;
    }

    protected abstract float GetDeltaZoom();
}