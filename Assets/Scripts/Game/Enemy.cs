using System.Threading;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    private CanvasScaler _canvasScaler;

    private const int MinimumSpawnTime = 1;
    private const int MaximumSpawnTime = 5;
    private const int TimeDelay = 3000;
    
    public void Init(CanvasScaler canvasScaler)
    {
        _canvasScaler = canvasScaler;
        transform.localPosition = SetRandomPosition();
        StartMovement(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private async UniTaskVoid StartMovement(CancellationToken token)
    {
        await UniTask.Delay(TimeDelay,cancellationToken:token);
        await transform.DOLocalMove(SetRandomPosition(), Random.Range(MinimumSpawnTime,MaximumSpawnTime)).AsyncWaitForCompletion();
        StartMovement(token).Forget();
    }
    
    private Vector2 SetRandomPosition()
    {
        float canvasXResolution = _canvasScaler.referenceResolution.x;
        float canvasYResolution = _canvasScaler.referenceResolution.y;
        float randomXPosition = Random.Range(-canvasXResolution / 2, canvasXResolution / 2);
        float randomYPosition = Random.Range(-canvasYResolution / 2, canvasYResolution / 2);
        return new Vector2(randomXPosition, randomYPosition);
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}
