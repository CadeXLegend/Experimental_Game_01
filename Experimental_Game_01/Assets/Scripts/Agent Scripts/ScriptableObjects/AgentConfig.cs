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
        /// The Agent's Graphic Sprite to be Rendered.
        /// </summary>
        [Header("Agent Parameters")]
        [Tooltip("The Agent's Graphic Sprite to be Rendered.")]
        public Sprite Graphic;
        /// <summary>
        /// Whether or not the Agent will use a Rigidbody 2D for Physics based controls.
        /// </summary>
        [Tooltip("Whether or not the Agent will use a Rigidbody 2D for Physics based controls.")]
        public bool UsesRigidbody2D;
        /// <summary>
        /// The rate at which the Agent will move.
        /// </summary>
        [Tooltip("The rate at which the Agent will move.")]
        public float MovementSpeed;
    }
}
