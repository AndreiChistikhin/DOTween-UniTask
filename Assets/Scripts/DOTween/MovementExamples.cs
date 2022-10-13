using DG.Tweening;
using UnityEngine;

public class MovementExamples : MonoBehaviour
{
    private readonly int speed=5;
    
    private void Start()
    {
        Move(Vector3.zero);
    }

    private void Move(Vector3 pointToMove)
    {
        float distance = Vector3.Distance(transform.position ,pointToMove);
        float time = distance / speed;
        transform.DOMove(pointToMove, time);
    }
}
