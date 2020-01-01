using MyBox;
using UnityEngine;

namespace Agent
{

    /// <summary>
    /// ScriptableObject containing Agent information.
    /// </summary>
    [CreateAssetMenu(fileName = "New Agent Configuration", menuName = "Agents/New Agent Config", order = 0)]
    public class AgentConfig : ScriptableObject
    {
        /// <summary>
        /// The Agent's Name to Identify said Agent and to be Displayed.
        /// </summary>
        [Header("Agent Parameters")]
        [Tooltip("The Agent's Name to Identify said Agent and to be Displayed.")]
        public string AgentName;
        /// <summary>
        /// The Agent's Special Type: Unique Abilities
        /// </summary>
        [Tooltip("The Agent's Special Type: Unique Abilities")]
        public Agent.AgentSpecialType specialType;
        /// <summary>
        /// The Agent's Graphic Sprite to be Rendered.
        /// </summary>
        [Tooltip("The Agent's Graphic Sprite to be Rendered.")]
        public Sprite Graphic;
        /// <summary>
        /// Whether or not the Agent will use a Rigidbody 2D for Physics based controls.
        /// </summary>
        [Tooltip("Whether or not the Agent will use a Rigidbody 2D for Physics based controls.")]
        public bool UsesRigidbody2D;
        /// <summary>
        /// The localScale of the Agent.
        /// </summary>
        [Tooltip("The localScale of the Agent.")]
        public Vector2 AgentSize;

        [Header("Agent Stats")]
        /// <summary>
        /// Whether or not to set stats like Health, Attack, etc for this Agent.
        /// </summary>
        [Tooltip("Whether or not to set stats like Health, Attack, etc for this Agent.")]
        [SerializeField]
        private bool useAgentStats;
        /// <summary>
        /// The rate at which the Agent will move.
        /// </summary>
        [Tooltip("The rate at which the Agent will move.")]
        public float MovementSpeed;
        /// <summary>
        /// The Maximum Health an Agent will start with.
        /// </summary>
        [Tooltip("The Maximum Health an Agent will start with.")]
        public int Health;
        /// <summary>
        /// The base Attack an Agent will start with.
        /// </summary>
        [Tooltip("The base Attack an Agent will start with.")]
        public int Attack;
        /// <summary>
        /// The base Attack Range an Agent will start with
        /// </summary>
        [Tooltip("The base Attack Range an Agent will start with.")]
        public int AttackRange;
        /// <summary>
        /// How many Actions this Agent can make in 1 Turn
        /// </summary>
        [Tooltip("How many Actions this Agent can make in 1 Turn")]
        public int ActionsPerTurn;

        [Header("Agent Input Types")]
        [EnumMask] public MoverFromInput.TypeOfInput InputTypesForAgent;
    }
}
