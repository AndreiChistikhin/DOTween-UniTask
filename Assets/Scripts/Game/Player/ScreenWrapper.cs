using System;
using UnityEngine;
using UnityEngine.UI;

public class ScreenWrapper : MonoBehaviour
{
    [SerializeField] private CanvasScaler _canvasScaler;
    
    private float _leftConstraint;
    private float _rightConstraint;
    private float _bottomConstraint;
    private float _topConstraint;

    public event Action ScreenWasCrossed;

    private void Start()
    {
        CalculateScreenBorders(new Vector2(_canvasScaler.referenceResolution.x,_canvasScaler.referenceResolution.y));
    }

    private void Update()
    {
        CheckWrapping();
    }

    private void CheckWrapping()
    {
        Vector3 position = transform.localPosition;
        if (position.x > _rightConstraint)
        {
            transform.localPosition = 
                new Vector3(_leftConstraint,position.y,transform.localPosition.z);
            ScreenWasCrossed?.Invoke();
        }

        if (position.x < _leftConstraint)
        {
            transform.localPosition = 
                new Vector3(_rightConstraint,position.y,transform.localPosition.z);
            ScreenWasCrossed?.Invoke();
        }

        if (position.y < _bottomConstraint)
        {
            transform.localPosition = 
                new Vector3(position.x,_topConstraint,transform.localPosition.z);
            ScreenWasCrossed?.Invoke();
        }

        if (position.y > _topConstraint)
        {
            transform.localPosition = 
                new Vector3(position.x,_bottomConstraint,transform.localPosition.z);
            ScreenWasCrossed?.Invoke();
        }
    }

    private void CalculateScreenBorders(Vector2 borders)
    {
        _rightConstraint = borders.x/2;
        _leftConstraint = -_rightConstraint;
        _topConstraint = borders.y/2;
        _bottomConstraint = -_topConstraint;
    }
}