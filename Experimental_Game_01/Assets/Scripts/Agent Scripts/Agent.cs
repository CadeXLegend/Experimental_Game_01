using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agent
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Agent : MonoBehaviour
    {
        private AgentConfig agentData;
        private GameObject agent;
        private Rigidbody2D rb;
        private SpriteRenderer r;
        private MoverFromInput mover;

        public virtual void Init(AgentConfig _agentData, MoverFromInput _mover)
        {
            name = _agentData.AgentName;
            agentData = _agentData;
            mover = _mover;
            rb = GetComponent<Rigidbody2D>();
            r = GetComponent<SpriteRenderer>();
            mover = GetComponent<MoverFromInput>();
            r.sprite = agentData.Graphic;
            transform.localScale = agentData.AgentSize;
        }
    }
}
