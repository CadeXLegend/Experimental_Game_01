using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
namespace Agent
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Agent : MonoBehaviour
    {
        private AgentConfig agentData;
        private GameObject agent;
        [SerializeField, GetComponent]
        private Rigidbody2D rb;
        [SerializeField, GetComponent]
        private SpriteRenderer r;
        private MoverFromInput mover;

        private int health;
        private int Health
        {
            get
            {
                //restrict the possible values for health to be
                //within the min and max health range
                return Mathf.Clamp(0, maxHealth, health);
            }
            set
            {
                //restrict the possible values for health to be
                //within the min and max health range
                health = Mathf.Clamp(0, maxHealth, value);
            }
        }
        private int maxHealth;
        private int attack;
        private int attackRange;

        //NPC INIT -> use this to initialize a non-playing Agent.
        /// <summary>
        /// Initializes an Agent with the given Parameters.
        /// </summary>
        /// <param name="_agentData">The template data container for the Agent class.</param>
        public virtual void Init(AgentConfig _agentData)
        {
            agentData = _agentData;
            rb = GetComponent<Rigidbody2D>();
            r = GetComponent<SpriteRenderer>();

            //binding agent data
            name = agentData.AgentName;
            transform.localScale = agentData.AgentSize;
            r.sprite = agentData.Graphic;
            maxHealth = agentData.Health;
            health = maxHealth;
            attack = agentData.Attack;
            attackRange = agentData.AttackRange;
        }

        //PLAYER INIT -> use this to initialize a player Agent.
        /// <summary>
        /// Initializes an Agent with the given Parameters.
        /// </summary>
        /// <param name="_agentData">The template data container for the Agent class.</param>
        /// <param name="_mover">The input receiver for the Agent.</param>
        public virtual void Init(AgentConfig _agentData, MoverFromInput _mover)
        {
            agentData = _agentData;
            mover = _mover;
            //rb = GetComponent<Rigidbody2D>();
            //r = GetComponent<SpriteRenderer>();
            mover = GetComponent<MoverFromInput>();

            //binding agent data
            name = agentData.AgentName;
            transform.localScale = agentData.AgentSize;
            r.sprite = agentData.Graphic;
            maxHealth = agentData.Health;
            health = maxHealth;
            attack = agentData.Attack;
            attackRange = agentData.AttackRange;
        }
    }
}
