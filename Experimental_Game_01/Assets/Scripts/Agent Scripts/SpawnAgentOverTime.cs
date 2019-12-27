using Generation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAgentOverTime : MonoBehaviour
{
    private GridAssetSpawner assetSpawner;

    [Header("Spawn Information")]
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

    private void FixedUpdate()
    {
        if (TurnTicker.Ticks % 10 == 0)
            SpawnEnemy();
    }

    internal virtual void SpawnEnemy()
    {
        if (currentTile == null) return;
        if (assetSpawner == null) return;

        try
        {
            GameObject go = assetSpawner.SpawnAssetGeneric(
                asset: agentToSpawn,                    //the agent we want to spawn into the world
                assetName: agentToSpawn.name,           //the name of the agent, e.g: skeleton
                t: currentTile,                         //the current tile the thing this is attached to is on
                spawnOntoRandomTileNeighbour: true      //whether we want this object to spawn automagically onto a randomly selected neighbour tile
                );

            go.tag = "Enemy";
            AISuperSimpleMove aiMover = go.AddComponent<AISuperSimpleMove>();
            aiMover.Init(agentConfig, go.transform.parent.GetComponent<Tile>());
            go.GetComponent<Agent.Agent>().Init(agentConfig, aiMover);
        }
        catch { /*please don't do what I'm doing here*/ }
    }
}
