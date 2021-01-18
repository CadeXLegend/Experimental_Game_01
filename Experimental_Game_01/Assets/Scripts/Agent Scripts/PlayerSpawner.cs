using Generation;
using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agents
{
    public class PlayerSpawner : MonoBehaviour
    {
        public static Action OnPlayersSpawned;
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

        private List<GameObject> SpawnedPlayers = new List<GameObject>();
        // Start is called before the first frame update
        void Start()
        {
            generator.OnMapGenerated += SpawnPlayers;
        }

        GameObject go;
        [ButtonMethod]
        public virtual void SpawnPlayers()
        {
            if (SpawnedPlayers.Count > 0)
            {
                SpawnedPlayers.ForEach(go => Destroy(go));
                SpawnedPlayers.Clear();
            }

            for (int i = 0; i < AmountOfPlayers; ++i)
            {
                Tile chosenTile = generator.Grid.GetRandomUnoccupiedTile(Tile.PositionOnGrid.Center);
                SpawnedPlayers.Add(go = Spawn(
                    prefab: playerPrefabs[i],                   //the player's prefab/gameobject to spawn
                    agentData: playerDatas[i],                  //the player's data to bind to the player's components
                    position: chosenTile.transform.position,    //the player's tile to spawn on
                    parent: chosenTile.transform,               //the parent the player will be a child of
                    tileSpawnedOn: chosenTile));                //the tile the player was spawned onto 
                go.GetComponent<Tile>().Parent = chosenTile;
            }
            OnPlayersSpawned?.Invoke();
        }

        public virtual GameObject Spawn(GameObject prefab, AgentConfig agentData, Vector3 position, Transform parent, Tile tileSpawnedOn)
        {
            #region Instantiation & Component Initialization
            prefab = Instantiate(prefab, position, Quaternion.identity, parent);
            if (prefab.tag != "Player")
                prefab.tag = "Player";
            Agent agent = prefab.GetComponent<Agent>();
            MoverFromInput mover = prefab.AddComponent<MoverFromInput>();
            UnityEditor.CAutoInjectionEditor.InjectFor_CurrentScene();
            #endregion

            #region Data Binding
            mover.Init(agent, agentData, agentData.InputTypesForAgent, tileSpawnedOn);
            agent.Init(agentData, mover);
            #endregion

            TurnManagement.AgentTurnSetter.AddAgentToStateMachine(agent);
            return prefab;
        }
    }
}
