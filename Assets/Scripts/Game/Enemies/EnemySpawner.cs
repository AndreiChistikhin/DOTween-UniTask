public class EnemySpawner : Spawner
{
    private const int EnemySpawnTime = 3000;
    public override void Start()
    {
        Init(EnemySpawnTime);
        base.Start();
    }
}
