using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    private JArray[] enemySpacecraftDatas;
    private List<Enemy> enemies = new();
    private List<Station> stations = new();
    
    private const int maxEnemies = 5;

    private void Awake()
    {
        stations = new List<Station>(FindObjectsOfType<Station>());
    }

    private void Start()
    {
        LoadEnemyDatas();

        for (int i = 0; i < maxEnemies; i++)
        {
            var spawnPosition = FindRandomSpawnPosition();
            if (IsSpawnPositionClear(spawnPosition))
            {
                SpawnRandomEnemy(spawnPosition);
            }
        }

        StartCoroutine(SpawnEnemiesCoroutine());
    }

    private void SpawnRandomEnemy(Vector2 position)
    {
        var randomIndex = Random.Range(0, enemySpacecraftDatas.Length);
        var enemyData = enemySpacecraftDatas[randomIndex];

        var type = (EnemyType)Random.Range(0, 2);
        var rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));

        var enemyGameObject = new GameObject("Enemy");
        var enemy = enemyGameObject.AddComponent<Enemy>();
        enemy.Initialize(type, enemyData, position, rotation);
        enemies.Add(enemy);
    }

    private Vector2 FindRandomSpawnPosition()
    {
        var spawnRadius = Random.Range(30.0f, 40.0f);
        var offset =  (Vector2)Random.onUnitSphere * spawnRadius;
        return (Vector2)Camera.main.transform.position + offset;
    }

    private bool IsSpawnPositionClear(Vector2 position)
    {
        var layerMask = LayerMask.GetMask("Station", "Module");
        var colliders = Physics2D.OverlapCircleAll(position, 5.0f, layerMask);

        return colliders.Length == 0;
    }

    private IEnumerator SpawnEnemiesCoroutine()
    {
        while (true)
        {
            if (enemies.Count < maxEnemies)
            {
                var spawnPosition = FindRandomSpawnPosition();
                if (IsSpawnPositionClear(spawnPosition))
                {
                    SpawnRandomEnemy(spawnPosition);
                }
            }
            yield return new WaitForSeconds(5.0f);
        }
    }

    private void LoadEnemyDatas()
    {
        var gameResource = GameManager.Instance.GetSystem<GameResource>();
        var enemyDatas = gameResource.GameResourceAsset.enemyDatas;
        enemySpacecraftDatas = new JArray[enemyDatas.Length];
        for (var i = 0; i < enemyDatas.Length; i++)
        {
            var json = enemyDatas[i].text;
            enemySpacecraftDatas[i] = JArray.Parse(json);
        }
    }
}
