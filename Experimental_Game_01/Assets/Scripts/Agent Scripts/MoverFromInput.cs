using Generation;
using System;
using UnityEngine;

namespace Agent
{
    /// <summary>
    /// Moves the Agent from Input Received.
    /// </summary>
    //we only want to do movement calculations
    //with the physics engine, not translate,
    //or any vector math outside of the fixedupdate physics cycle
    [RequireComponent(typeof(Rigidbody2D))]
    public class MoverFromInput : MonoBehaviour, IMover
    {
        [Flags]
        public enum TypeOfInput
        {
            Keyboard = 1 << 0, // 1
            Mouse = 1 << 1, // 2
            ClickOnGrid = 1 << 2, // 4
            Touch = 1 << 3, // 8
        }

        //defaults to reading keyboard input
        private TypeOfInput inputType = TypeOfInput.Keyboard;

        private Agent parent;

        private Rigidbody2D rb;
        [SerializeField, ReadOnly]
        private Tile currentTile;

        /// <summary>
        /// The configuration file containing Agent information to be applied to this class's parameters.
        /// </summary>
        [SerializeField]
        private AgentConfig agentConfiguration;
        [SerializeField]
        private float movementSpeed;

        private Vector2 previousTilePosition = Vector2.zero;

        public void Init(Agent _parent, AgentConfig config)
        {
            parent = _parent;
            agentConfiguration = config;
            rb = GetComponent<Rigidbody2D>();
        }

        public void Init(Agent _parent, AgentConfig config, TypeOfInput _inputType)
        {
            parent = _parent;
            agentConfiguration = config;
            rb = GetComponent<Rigidbody2D>();
            inputType = _inputType;
        }

        public void Init(Agent _parent, AgentConfig config, TypeOfInput _inputType, Tile _currentTile)
        {
            parent = _parent;
            agentConfiguration = config;
            rb = GetComponent<Rigidbody2D>();
            inputType = _inputType;
            currentTile = _currentTile;
            previousTilePosition = _currentTile.transform.position;
        }

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            movementSpeed = agentConfiguration.MovementSpeed;
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            if (!parent.CanDoActions)
            {
                currentTile.StopVisualizingNeighbours();
                return;
            }

            Move(ValueFromInput());           
        }

        public virtual void Move(Vector2 direction)
        {
            if (!agentConfiguration.UsesRigidbody2D)
                return;

            if (direction == previousTilePosition)
                return;

            if (Vector2.Distance(transform.position, direction) < 0.01f)
            {
                previousTilePosition = direction;
                parent.ProcessAction();
                return;
            }

            transform.position = Vector2.LerpUnclamped(transform.position, direction, movementSpeed * Time.fixedDeltaTime);
            //rb.MovePosition(direction * (movementSpeed * Time.fixedDeltaTime));
        }

        private Vector2 ValueFromInput()
        {
            switch (inputType)
            {
                case TypeOfInput.Keyboard:
                    return KeyboardInputToVector2();
                case TypeOfInput.ClickOnGrid:
                    return ClickOnGridInputToVector2();
                default:
                    //do nothing...
                    break;
            }
            return Vector2.zero;
        }

        private Vector2 ClickOnGridInputToVector2()
        {
            currentTile.VisualizeNeighbours();
            foreach(Tile t in currentTile.Neighbours)
            {
                if (t.Selected)
                {
                    currentTile.StopVisualizingNeighbours();
                    currentTile = t;
                    return currentTile.transform.position;
                }
            }
            return currentTile.transform.position;
        }

        private Vector2 KeyboardInputToVector2()
        {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            return new Vector2(x, y);
        }
    }
}
