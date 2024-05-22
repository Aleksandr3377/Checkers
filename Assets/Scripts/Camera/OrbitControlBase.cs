using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class OrbitControlBase : CameraControlBase
{
    [SerializeField] protected float RotationSpeed = 2f;
    protected Vector3 PreviousMousePos;
    private float _currentRotX;
    protected float FinalRotX;
    private float _finalRotY;
    private float _currentRotY;

    protected override void Start()
    {
        base.Start();
        FinalRotX = transform.eulerAngles.x;
        _finalRotY = transform.eulerAngles.y;
        _currentRotX = FinalRotX;
        _currentRotY = _finalRotY;
    }

    private void Update()
    {
        _finalRotY += GetDeltaRotY();
        var nextRotY = Mathf.Lerp(_currentRotY, _finalRotY, Time.deltaTime * Smoothness);
        var deltaRotY = nextRotY - _currentRotY;
        transform.RotateAround(Target.position, Vector3.up, deltaRotY);
        _currentRotY = nextRotY;
        
        FinalRotX += GetDeltaRotX();
        var nextRotX = Mathf.Lerp(_currentRotX, FinalRotX, Time.deltaTime * Smoothness);
        var deltaRotX = nextRotX - _currentRotX;
        transform.RotateAround(Target.position, -transform.right, deltaRotX);
        _currentRotX = nextRotX;
    }
    
    private void LateUpdate()
    {
        PreviousMousePos = Input.mousePosition;
    }

    protected abstract float GetDeltaRotX();

    protected abstract float GetDeltaRotY();
}

public  class A:IEnumerable<int>
{
    private int x = 5;

    private IEnumerator<int> MyEnumerator
    {
        get
        {
            yield return 5;
            yield return 7;
            yield return -1;
        }
    }

    private void Test()
    {
        int y = 0;
        while (x>y)
        {
            if (Random.Range(0, 100) == 50)
            {
                break;
            }
            
            y++;
            
        }

        foreach (var x in this)
        {
            
        }
    }


    public IEnumerator<int> GetEnumerator()
    {

        return MyEnumerator;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}