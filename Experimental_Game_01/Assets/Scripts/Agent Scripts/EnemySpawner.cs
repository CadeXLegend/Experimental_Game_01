using Generation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Information")]
    [SerializeField]
    private float initialSpawnTime;
    [SerializeField]
    private float spawnTimeIncrement;
    private float actualSpawnTime;
    private float ActualSpawnTime
    {
        get
        {
            return actualSpawnTime /= spawnTimeIncrement;
        }
    }

    [Header("Dependencies")]
    [SerializeField]
    private MapGenerator mapGenerator;
    [SerializeField]
    private GridAssetSpawner assetSpawner;

    [SerializeField]
    private EnemyAssets assets;

    private void Start()
    {
        actualSpawnTime = initialSpawnTime;
        mapGenerator.MapGenerated += SpawnEnemyGameObjects;
        mapGenerator.MapGenerated += SpawnEnemyOnTick;
    }


    private void SpawnEnemyGameObjects()
    {
        assetSpawner.SpawnAssetGeneric(mapGenerator.Grid, assets.EnemySpawner, assets.EnemySpawner.name, Tile.PositionOnGrid.TopLeftCorner);
        assetSpawner.SpawnAssetGeneric(mapGenerator.Grid, assets.EnemySpawner, assets.EnemySpawner.name, Tile.PositionOnGrid.TopRightCorner);
        assetSpawner.SpawnAssetGeneric(mapGenerator.Grid, assets.EnemySpawner, assets.EnemySpawner.name, Tile.PositionOnGrid.BottomLeftCorner);
        assetSpawner.SpawnAssetGeneric(mapGenerator.Grid, assets.EnemySpawner, assets.EnemySpawner.name, Tile.PositionOnGrid.BottomRightCorner);
    }

    private void SpawnEnemyOnTick()
    {
        InvokeRepeating("SpawnEnemy", initialSpawnTime, ActualSpawnTime);
    }

    private void SpawnEnemy()
    {
        assetSpawner.SpawnAssetGeneric(mapGenerator.Grid, assets.Enemies.Entities[0], assets.Enemies.Entities[0].name, Tile.PositionOnGrid.Center);
    }
}
