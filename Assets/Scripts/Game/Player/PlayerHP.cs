using System;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    private int _hpAmount = 3;

    public event Action PlayerHit;
    public event Action PlayerDied;
    public event Action CoinPicked;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent(out Enemy enemy))
        {
            _hpAmount--;
            PlayerHit?.Invoke();
            if (_hpAmount <= 0)
                PlayerDied?.Invoke();
        }

        if (col.TryGetComponent(out Coin coin))
        {
            CoinPicked?.Invoke();
            coin.DestroyCoin();
        }
    }
}