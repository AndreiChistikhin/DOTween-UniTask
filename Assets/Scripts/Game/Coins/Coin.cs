using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Coin : SpawnedObject
{
    [SerializeField] private Image _image;
    [SerializeField] private BoxCollider2D _collider;
    
    private const int AppearanceTime = 1;
    
    public override void Init(CanvasScaler canvasScaler,Action<SpawnedObject> returnToPoolAction)
    {
        base.Init(canvasScaler,returnToPoolAction);
        _collider.enabled = false;
        _image.DOFade(1, AppearanceTime).OnComplete(()=>
        {
            _collider.enabled = true;
        }).WithCancellation(this.GetCancellationTokenOnDestroy());
    }

    public void DestroyCoin()
    {
        _image.DOFade(0, AppearanceTime).OnComplete(()=>
        {
            _image.DOKill();
            ReturnToThePool?.Invoke(this); 
        }).WithCancellation(this.GetCancellationTokenOnDestroy());
    }
}
