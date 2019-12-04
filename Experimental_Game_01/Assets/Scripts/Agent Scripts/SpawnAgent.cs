using UnityEngine;

namespace Agent
{
    public class SpawnAgent
    {
        private Agent agent;
        private MoverFromInput mover;

        public virtual GameObject Spawn(GameObject prefab, AgentConfig agentData, Vector3 position, Transform parent)
        {
            #region Instantiation & Component Initialization
            prefab = GameObject.Instantiate(prefab, position, Quaternion.identity, parent);
            agent = prefab.GetComponent<Agent>();
            mover = prefab.AddComponent<MoverFromInput>();
            #endregion

            #region Data Binding
            mover.Init(agentData);
            agent.Init(agentData, mover);
            #endregion

            return prefab;
        }
    }
}
