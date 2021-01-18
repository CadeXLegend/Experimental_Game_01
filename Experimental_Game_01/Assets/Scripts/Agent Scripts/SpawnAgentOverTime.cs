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
    private Agents.AgentConfig agentConfig;
    private Tile currentTile;
    private int amountOfAgentsSpawned = 0;

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
        if (amountOfAgentsSpawned >= 2)
            return;
        
        if (TurnTicker.Ticks == 1)
            SpawnEnemy();
        if (TurnTicker.Ticks > 1)
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
                asset: agentToSpawn,                            //the agent we want to spawn into the world
                assetName: agentToSpawn.name,                   //the name of the agent, e.g: skeleton
                t: currentTile,                                 //the current tile the thing this is attached to is on
                spawnOntoRandomTileNeighbour: true              //whether we want this object to spawn automagically onto a randomly selected neighbour tile
                );

            if(!go.CompareTag("Enemy"))
                go.tag = "Enemy";
            Tile goT = go.GetComponent<Tile>();
            goT.Type = Tile.TileType.Enemy;
            Agents.Agent spawnedAgent = go.GetComponent<Agents.Agent>();
            AISuperSimpleMove aiMover = go.AddComponent<AISuperSimpleMove>();
            spawnedAgent.Init(agentConfig, aiMover);
            aiMover.Init(spawnedAgent, agentConfig, go.transform.parent.GetComponent<Tile>());
            TurnManagement.AgentTurnSetter.AddAgentToStateMachine(spawnedAgent);
            amountOfAgentsSpawned++;
            go.name += $" ({amountOfAgentsSpawned})";
        }
        catch { /*please don't do what I'm doing here*/ }
    }
}
