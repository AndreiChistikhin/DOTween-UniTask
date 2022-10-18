using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyFactory _factory;
    [SerializeField] private CanvasScaler _canvasScaler; 

    private void Start()
    {
        SpawnEnemy(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private async UniTaskVoid SpawnEnemy(CancellationToken cancellationToken)
    {
        Enemy enemy=_factory.GetObject();
        enemy.Init(_canvasScaler);
        await UniTask.Delay(3000, cancellationToken: cancellationToken);
        SpawnEnemy(cancellationToken).Forget();
    }
}
