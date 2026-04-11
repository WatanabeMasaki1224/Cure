using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] Transform[] _spawnPoints;
    [SerializeField] EnemyData[] _enemyDatas;
    [SerializeField] int _maxTotal = 10;
    [SerializeField] float _spawnInterval = 2f;
    float _timer;

    Dictionary<EnemyData, int> _currentCounts = new Dictionary<EnemyData, int>();
    int _currentTotal = 0;

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

        EnemyData data = GetRandomEnemy();

        if (data == null) return;

        // スポーン位置ランダム
        Transform point = _spawnPoints[Random.Range(0, _spawnPoints.Length)];

        GameObject obj = Instantiate(data.prefab, point.position, Quaternion.identity);

        _currentCounts[data]++;
        _currentTotal++;

        // 死亡時に減らす
        obj.GetComponent<Enemy>().Init(this, data);
    }

    EnemyData GetRandomEnemy()
    {
        List<EnemyData> candidates = new List<EnemyData>();

        foreach (var data in _enemyDatas)
        {
            if (_currentCounts[data] < data.maxCount)
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

}
