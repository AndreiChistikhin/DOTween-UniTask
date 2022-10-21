public class CoinSpawner : Spawner
{
    private const int CoinSpawnTime = 2000;
    public override void Start()
    {
        Init(CoinSpawnTime);
        base.Start();
    }
}
