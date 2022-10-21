using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GenericFactory<SpawnedObject> _factory;
    [SerializeField] private CanvasScaler _canvasScaler;

    private int _spawnTime;
    private const int DefaultSpawnTime=2000;

    public void Init(int spawnTime)
    {
        _spawnTime = spawnTime;
    }

    public virtual void Start()
    {
        SpawnObject(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private async UniTaskVoid SpawnObject(CancellationToken cancellationToken)
    {
        if (_spawnTime == 0)
            _spawnTime = DefaultSpawnTime;
        SpawnedObject spawnedObject = _factory.GetObject();
        spawnedObject.Init(_canvasScaler,_factory.ReleaseObject);
        await UniTask.Delay(_spawnTime, cancellationToken: cancellationToken);
        SpawnObject(cancellationToken).Forget();
    }
}