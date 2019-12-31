using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnManagement
{
    public class AgentTurnSetter : MonoBehaviour
    {
        [SerializeField]
        private Generation.MapGenerator mapGenerator;
        //All the agents active in the Game
        private static List<Agent.Agent> agents = new List<Agent.Agent>();
        //the current agent for the current turn tick
        private int currentAgent = 0;
        //how many actions the current agent can take
        private int currentAgentsActionsPerTurn = 0;
        //how many actions the current agent has taken in this turn
        private int currentAgentsTakenActions = 0;
        //can the current agent do actions?
        bool currentAgentCanDoActions = false;

        private void Start()
        {
            mapGenerator.OnMapGenerating += agents.Clear;
        }

        private void LateUpdate()
        {
            ProcessAgentTurns();
        }

        public static void AddAgentToStateMachine(Agent.Agent agent)
        {
            agents.Add(agent);
        }

        private void ProcessAgentTurns()
        {
            if (agents.Count < 1)
                return;

            if (currentAgent > agents.Count - 1)
                currentAgent = 0;

            currentAgentsActionsPerTurn = agents[currentAgent].ActionsPerTurn;
            currentAgentsTakenActions = agents[currentAgent].ActionsTakenInTurn;
            currentAgentCanDoActions = currentAgentsTakenActions < currentAgentsActionsPerTurn;

            agents[currentAgent].CanDoActions = currentAgentCanDoActions;
            if (!currentAgentCanDoActions)
            {
                Debug.Log($"<b>Agent: {agents[currentAgent].name}</b> just finished their turn.");
                currentAgent++;
                TurnTicker.Tick();
            }
        }
    }
}
