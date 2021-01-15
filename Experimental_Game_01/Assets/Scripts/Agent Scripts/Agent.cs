using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System;

namespace Agent
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Agent : MonoBehaviour
    {
        public Action OnHealthChanged;
        public enum AgentSpecialType
        {
            None,
            Druid,
            Lumberjack,
        }
        private AgentSpecialType specialType;

        private AgentConfig agentData;
        private GameObject agent;
        [SerializeField, GetComponent]
        private Rigidbody2D rb;
        [SerializeField, GetComponent]
        private SpriteRenderer r;
        private IMover mover;

        private GenerateResources resourcesGenerator;

        private int health;
        public int Health
        {
            get
            {
                //restrict the possible values for health to be
                //within the min and max health range
                return Mathf.Clamp(health, 0, maxHealth);
            }
            set
            {
                //restrict the possible values for health to be
                //within the min and max health range
                health = Mathf.Clamp(value, 0, maxHealth);
                OnHealthChanged?.Invoke();
            }
        }
        private int maxHealth;
        public int MaxHealth { get => maxHealth; }
        private int attack;
        public float Attack { get => attack; }
        private int attackRange;
        public float AttackRange { get => attackRange; }
        private float detectionRange;
        public float DetectionRange { get => detectionRange; }
        //defaults to 1 action per turn
        private int actionsPerTurn = 1;
        public int ActionsPerTurn { get => actionsPerTurn; }
        private int actionsTakenInTurn = 0;
        public int ActionsTakenInTurn { get => actionsTakenInTurn; }
        private int turnsTakenInGame = 0;

        public bool CanDoActions { get; set; }
        private bool canGenerateAgain = false;

        private void Start()
        {
            TurnTicker.OnTick += ResetTurnActionPoints;

            switch(specialType)
            {
                case AgentSpecialType.Druid:
                    resourcesGenerator = GameObject.Find("Druid - Generate Resources").GetComponent<GenerateResources>();
                    break;
            }
        }

        private void LateUpdate()
        {
            DruidSpecialAbility();
        }

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
            detectionRange = agentData.DetectionRange;
            actionsPerTurn = agentData.ActionsPerTurn;
            specialType = agentData.specialType;
            detectionRange = agentData.DetectionRange;
        }

        /// <summary>
        /// Initializes an Agent with the given Parameters.
        /// </summary>
        /// <param name="_agentData">The template data container for the Agent class.</param>
        /// <param name="_mover">The input receiver for the Agent.</param>
        public virtual void Init(AgentConfig _agentData, IMover _mover)
        {
            agentData = _agentData;
            mover = _mover;
            //rb = GetComponent<Rigidbody2D>();
            //r = GetComponent<SpriteRenderer>();
            //mover = GetComponent<MoverFromInput>();

            //binding agent data
            name = agentData.AgentName;
            transform.localScale = agentData.AgentSize;
            r.sprite = agentData.Graphic;
            maxHealth = agentData.Health;
            health = maxHealth;
            attack = agentData.Attack;
            attackRange = agentData.AttackRange;
            actionsPerTurn = agentData.ActionsPerTurn;
            specialType = agentData.specialType;
            detectionRange = agentData.DetectionRange;
        }

        public virtual void ProcessAction()
        {
            actionsTakenInTurn++;
        }

        public virtual void ResetTurnActionPoints()
        {
            actionsTakenInTurn = 0;
            turnsTakenInGame++;

            if (!(specialType == AgentSpecialType.Druid))
                return;

            canGenerateAgain = true;
        }

        private void DruidSpecialAbility()
        {
            if (!(specialType == AgentSpecialType.Druid))
                return;

            //resourcesGenerator.Generate();

            if (!CanDoActions)
                return;
            if (!canGenerateAgain)
                return;
            if (!(turnsTakenInGame % 1 == 0))
                return;

            resourcesGenerator.Generate();
            canGenerateAgain = false;
        }
    }
}
