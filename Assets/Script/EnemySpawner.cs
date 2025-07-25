using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawn
{
    public string name;
    public GameObject enemyPrefab;
    public int maxCount = 5;
    public float respawnDelay = 3f;
    [HideInInspector]public List<GameObject> aliveEnemy = new List<GameObject>();
    private bool spawn = false;

    public IEnumerator TryRespawn(Transform[] spawnPoint)
    {
        if(spawn)
        {
            yield break;
        }

        spawn = true;
        yield return new WaitForSeconds(respawnDelay);
        aliveEnemy.RemoveAll(e => e= null);

        if(aliveEnemy.Count < maxCount )
        {
            int y = Random.Range(0,spawnPoint.Length);
            GameObject newEnemy = GameObject.Instantiate(enemyPrefab, spawnPoint[y].position, Quaternion.identity); ;
            aliveEnemy.Add(newEnemy);
        }
        
        spawn=false;
    }
}


public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoint;
    [SerializeField] private List<EnemySpawn> enemyTypes;

     void Start()
    {
       foreach (var enemyType in enemyTypes)
        {
            for (int i = 0; i < enemyType.maxCount ; i++)
            {
                int x = Random.Range(0,spawnPoint.Length);
                GameObject enemy = Instantiate(enemyType.enemyPrefab, spawnPoint[x].position,Quaternion.identity);
                enemyType.aliveEnemy.Add(enemy);
            }
        }
        
    }

    void Update()
    {
        foreach(var enemyType in enemyTypes)
        {
            enemyType.aliveEnemy.RemoveAll(e => e = null);

            if (enemyType.aliveEnemy.Count < enemyType.maxCount)
            {
                StartCoroutine(enemyType.TryRespawn(spawnPoint));
            }
        }

        
    }
}
