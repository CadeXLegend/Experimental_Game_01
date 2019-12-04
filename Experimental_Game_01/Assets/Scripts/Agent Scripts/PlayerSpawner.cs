﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agent
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> playerPrefabs;

        [SerializeField]
        private List<AgentConfig> playerDatas;
        [SerializeField]
        private int AmountOfPlayers = 1;

        [SerializeField]
        private Generation.MapGenerator generator;
        [SerializeField]
        private Transform SpawnedAgentsParent;

        private SpawnAgent agentSpawner;

        private List<GameObject> SpawnedPlayers = new List<GameObject>();
        // Start is called before the first frame update
        void Start()
        {
            agentSpawner = new SpawnAgent();
            generator.MapGenerated += SpawnPlayers;
        }

        public virtual void SpawnPlayers()
        {
            if (SpawnedPlayers.Count > 0)
            {
                SpawnedPlayers.ForEach(go => Destroy(go));
                SpawnedPlayers.Clear();
            }

            for (int i = 0; i < AmountOfPlayers; ++i)
                SpawnedPlayers.Add(agentSpawner.Spawn(
                    playerPrefabs[i],      //the player's prefab/gameobject to spawn
                    playerDatas[i],        //the player's data to bind to the player's components
                    generator.Grid.GetRandomUnoccupiedTile(Generation.Tile.PositionOnGrid.Center).transform.position, //the player's tile to spawn on
                    SpawnedAgentsParent));  //the parent the player will be a child of
        }
    }
}
