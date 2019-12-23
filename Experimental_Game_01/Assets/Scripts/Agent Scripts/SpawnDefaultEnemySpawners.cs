using Generation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDefaultEnemySpawners : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField]
    private MapGenerator mapGenerator;
    [SerializeField]
    private GridAssetSpawner assetSpawner;

    [SerializeField]
    private EnemyAssets assets;

    private void Start()
    {
        mapGenerator.OnMapGenerated += SpawnEnemyGameObjects;
    }

    private void SpawnEnemyGameObjects()
    {
        List<GameObject> spawners = new List<GameObject>();
        Tile.PositionOnGrid flags = 
            (
            Tile.PositionOnGrid.TopLeftCorner    | 
            Tile.PositionOnGrid.TopRightCorner   | 
            Tile.PositionOnGrid.BottomLeftCorner | 
            Tile.PositionOnGrid.BottomRightCorner
            );
        IEnumerable<Enum> flagValues = flags.GetIndividualFlags();
        foreach(Enum value in flagValues)
            spawners.Add(
                assetSpawner.SpawnAssetGeneric(
                mapGenerator.Grid,
                assets.EnemySpawner,
                assets.EnemySpawner.name,
                (Tile.PositionOnGrid)value
                ));

        foreach (GameObject go in spawners)
            go.GetComponent<SpawnAgentOverTime>().Init(assetSpawner, go.transform.parent.GetComponent<Tile>());       
    }
}
