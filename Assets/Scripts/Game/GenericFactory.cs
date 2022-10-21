using UnityEngine;
using UnityEngine.Pool;

public class GenericFactory<T> : MonoBehaviour where T : SpawnedObject
{
    [SerializeField] private T _prefab;
    [SerializeField] private Transform _parentTransform;

    private ObjectPool<T> _objectPool;
    
    private void Awake()
    {
        _objectPool = new ObjectPool<T>(
            () => Instantiate(_prefab, _parentTransform),
            t => t.gameObject.SetActive(true),
            t => t.gameObject.SetActive(true)
        );
    }

    public T GetObject()
    {
        return _objectPool.Get();
    }
    
    public void ReleaseObject(T pooledObject)
    {
        _objectPool.Release(pooledObject);
    }
}