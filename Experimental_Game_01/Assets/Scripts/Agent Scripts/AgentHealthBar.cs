using UnityEngine;
using UnityEngine.UI;

public class AgentHealthBar : MonoBehaviour
{
    [SerializeField]
    private Slider healthSlider;
    [SerializeField] private Agent.Agent agent;

    private void Start()
    {
        healthSlider.value = agent.MaxHealth;
        healthSlider.maxValue = agent.MaxHealth;
        agent.OnHealthChanged += () => { UpdateHealthSlider(); };
    }

    private void UpdateHealthSlider()
    {
        healthSlider.value = agent.Health;
    }
}
