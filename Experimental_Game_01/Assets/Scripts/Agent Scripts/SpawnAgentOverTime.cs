using Generation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAgentOverTime : MonoBehaviour
{
    private GridAssetSpawner assetSpawner;

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

    [SerializeField]
    private GameObject agentToSpawn;
    [SerializeField]
    private Agent.AgentConfig agentConfig;
    private Tile currentTile;

    /// <summary>
    /// Initialize this Object with the defined parameters.
    /// </summary>
    /// <param name="_assetSpawner">Asset Spawner required to call spawning functions</param>
    /// <param name="_currentTile">The tile this object is currently situated on</param>
    public void Init(GridAssetSpawner _assetSpawner, Tile _currentTile)
    {
        currentTile = _currentTile;
        assetSpawner = _assetSpawner;
    }

    private void Awake()
    {
        actualSpawnTime = initialSpawnTime;
        SpawnEnemyOnTick();
    }

    internal virtual void SpawnEnemyOnTick()
    {
        InvokeRepeating("SpawnEnemy", initialSpawnTime, ActualSpawnTime);
    }

    internal virtual void SpawnEnemy()
    {
        if (currentTile == null) return;
        if (assetSpawner == null) return;

        try
        {
            GameObject go = assetSpawner.SpawnAssetGeneric(
                agentToSpawn,  //the agent we want to spawn into the world
                agentToSpawn.name, //the name of the agent, e.g: skeleton
                currentTile, //the current tile the thing this is attached to is on
                spawnOntoRandomTileNeighbour: true //whether we want this object to spawn automagically onto a randomly selected neighbour tile
                );

            AISuperSimpleMove aiMover = go.AddComponent<AISuperSimpleMove>();
            aiMover.Init(agentConfig, go.transform.parent.GetComponent<Tile>());
            go.GetComponent<Agent.Agent>().Init(agentConfig, aiMover);
        }
        catch { /*please don't do what I'm doing here*/ }
    }
}
