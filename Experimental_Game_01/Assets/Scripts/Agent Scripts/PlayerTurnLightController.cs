using UnityEngine;
using Agents;
using TurnManagement;

public class PlayerTurnLightController : MonoBehaviour
{
    private GameObject light;
    private Agent agent;
    // Start is called before the first frame update
    private void Awake()
    {
        light = transform.GetChild(0).gameObject;
        agent = GetComponent<Agent>();
        AgentTurnSetter.OnAgentTurn += () =>
        {
            light.SetActive(AgentTurnSetter.IsCurrentAgent(agent));
        };
    }
}
