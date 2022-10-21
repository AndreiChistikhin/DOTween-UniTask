using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PointsUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _points;
    [SerializeField] private PlayerHP _player;

    private int _pointsAmount;

    private void OnEnable()
    {
        _player.CoinPicked += AddCoin;
    }

    private void OnDisable()
    {
        _player.CoinPicked -= AddCoin;
    }

    private void AddCoin()
    {
        _pointsAmount++;
        _points.text = _pointsAmount.ToString();
        _points.rectTransform.DOComplete();
        _points.DOComplete();
        _points.rectTransform.DOScale(1.5f, 1).SetLoops(2, LoopType.Yoyo).WithCancellation(this.GetCancellationTokenOnDestroy());
        _points.DOColor(Color.yellow, 1).SetLoops(2, LoopType.Yoyo).WithCancellation(this.GetCancellationTokenOnDestroy());
    }
}