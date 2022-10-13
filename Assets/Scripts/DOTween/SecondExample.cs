using UnityEngine;

public class SecondExample : MonoBehaviour
{
    private Vector3 _targetPoint;
    private float _current, _target;

    private readonly int _speed=5;
    
    private void Update()
    {
        _current = Mathf.MoveTowards(_current, _target, _speed * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, _targetPoint, _current);
    }
}
