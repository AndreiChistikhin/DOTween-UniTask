using System;
using System.Threading;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Enemy : SpawnedObject
{
    private const int MinimumChangePositionTime = 1;
    private const int MaximumChangePositionTime = 5;
    private const int TimeDelay = 3000;
    
    public override void Init(CanvasScaler canvasScaler,Action<SpawnedObject> returnToPoolAction)
    {
        base.Init(canvasScaler,returnToPoolAction);
        StartMovement(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private async UniTaskVoid StartMovement(CancellationToken token)
    {
        await UniTask.Delay(TimeDelay,cancellationToken:token);
        await transform.DOLocalMove(transform.GetPositionWithinScreen(CanvasScaler), 
            Random.Range(MinimumChangePositionTime,MaximumChangePositionTime)).WithCancellation(this.GetCancellationTokenOnDestroy());
        StartMovement(token).Forget();
    }
    
    private void OnDestroy()
    {
        transform.DOKill();
    }
}
