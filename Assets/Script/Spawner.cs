using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public enum SpawnType
    {
        Both,
        PlayerOnly,
        RepairOnly
    }

    [System.Serializable]
    public class SpawnPointData
    {
        public Transform point;
        public SpawnType type;
    }

    [SerializeField] SpawnPointData[] _spawnPoints;
    [SerializeField] EnemyData[] _enemyDatas;
    [SerializeField] int _maxTotal = 10;
    [SerializeField] float _spawnInterval = 2f;
    float _timer;

    Dictionary<EnemyData, int> _currentCounts = new Dictionary<EnemyData, int>();
    int _currentTotal = 0;
    Transform player;

    void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    void Start()
    {
        foreach (var data in _enemyDatas)
        {
            _currentCounts[data] = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        if(_timer >= _spawnInterval)
        {
            _timer = 0;
            TrySpawn();
        }
    }

    void TrySpawn()
    {
        if (_currentTotal >= _maxTotal) return;

        // プレイヤーが近くにいないポイントを探す
        List<SpawnPointData> validPoints = new List<SpawnPointData>();

        foreach (var p in _spawnPoints)
        {
            if (!IsNearPlayer(p.point.position))
            {
                validPoints.Add(p);
            }
        }

        if (validPoints.Count == 0) return;

        // スポーンポイント決める
        var point = validPoints[Random.Range(0, validPoints.Count)];
        //  そる敵を集めるの場所で出せ
        EnemyData data = GetRandomEnemy(point);

        if (data == null) return;

        //スポーン
        GameObject obj = Instantiate(data.prefab, point.point.position, Quaternion.identity);
        _currentCounts[data]++;
        _currentTotal++;
        obj.GetComponent<Enemy>().Init(this, data);
    }

    bool CanSpawn(EnemyData data, SpawnPointData point)
    {
        if (point.type == SpawnType.Both) return true;

        if (data.type == EnemyType.TargetPlayer && point.type == SpawnType.PlayerOnly)
            return true;

        if (data.type == EnemyType.TargetRepair && point.type == SpawnType.RepairOnly)
            return true;

        return false;
    }

    EnemyData GetRandomEnemy(SpawnPointData point)
    {
        List<EnemyData> candidates = new List<EnemyData>();

        foreach (var data in _enemyDatas)
        {
            if (_currentCounts[data] >= data.maxCount) continue;

            if (CanSpawn(data, point))
            {
                candidates.Add(data);
            }
        }

        if (candidates.Count == 0) return null;

        // 重み付きランダム
        float totalWeight = 0f;
        foreach (var c in candidates)
        {
            totalWeight += c.spawnRate;
        }

        float rand = Random.value * totalWeight;

        float sum = 0f;
        foreach (var c in candidates)
        {
            sum += c.spawnRate;
            if (rand <= sum)
                return c;
        }

        return candidates[0];
    }

    public void OnEnemyDead(EnemyData data)
    {
        _currentCounts[data]--;
        _currentTotal--;
    }

    bool IsNearPlayer(Vector2 pos)
    {
        return Vector2.Distance(player.position, pos) < 3.0f;
    }

}
