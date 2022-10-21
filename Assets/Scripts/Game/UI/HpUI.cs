using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HpUI : MonoBehaviour
{
    [SerializeField] private PlayerHP _playerHP;
    [SerializeField] private List<Image> _hpImage;

    private const int FlashDuration=1;
    private const int FlashTimes = 7;
    
    private void OnEnable()
    {
        _playerHP.PlayerHit += ShowEnemyHit;
    }

    private void OnDisable()
    {
        _playerHP.PlayerHit -= ShowEnemyHit;

    }

    private void ShowEnemyHit()
    {
        if(_hpImage.Count==0)
            return;
        _hpImage[0].DOFade(0, FlashDuration).SetEase(Ease.Flash,FlashTimes).WithCancellation(this.GetCancellationTokenOnDestroy());
        _hpImage.Remove(_hpImage[0]);
    }

    private void OnDestroy()
    {
        foreach (var image in _hpImage)
        {
            image.DOKill();
        }
    }
}
