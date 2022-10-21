using System;
using UnityEngine;
using UnityEngine.UI;

public class SpawnedObject : MonoBehaviour
{
    private CanvasScaler _canvasScaler;
    private Action<SpawnedObject> _returnToThePool;

    public CanvasScaler CanvasScaler=>_canvasScaler;
    public Action<SpawnedObject> ReturnToThePool=>_returnToThePool;

    public virtual void Init(CanvasScaler canvasScaler,Action<SpawnedObject> returnToPoolAction)
    {
        _canvasScaler = canvasScaler;
        transform.localPosition = transform.GetPositionWithinScreen(_canvasScaler);
        _returnToThePool = returnToPoolAction;
    }
}