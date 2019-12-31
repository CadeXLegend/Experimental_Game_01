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
        //[SerializeField, ReadOnly]
        private Tile previousTile;
        //[SerializeField, ReadOnly]
        private Tile currentTile;
        //[SerializeField, ReadOnly]
        private Tile tileToMoveTo;
        private bool hasGotTileToMoveTo = false;

        /// <summary>
        /// The configuration file containing Agent information to be applied to this class's parameters.
        /// </summary>
        [SerializeField]
        private AgentConfig agentConfiguration;
        [SerializeField]
        private float movementSpeed;

        public void Init(Agent _parent, AgentConfig config)
        {
            parent = _parent;
            agentConfiguration = config;
            rb = GetComponent<Rigidbody2D>();

            movementSpeed = agentConfiguration.MovementSpeed;
        }

        public void Init(Agent _parent, AgentConfig config, TypeOfInput _inputType)
        {
            parent = _parent;
            agentConfiguration = config;
            rb = GetComponent<Rigidbody2D>();
            inputType = _inputType;

            movementSpeed = agentConfiguration.MovementSpeed;
        }

        public void Init(Agent _parent, AgentConfig config, TypeOfInput _inputType, Tile _currentTile)
        {
            parent = _parent;
            agentConfiguration = config;
            rb = GetComponent<Rigidbody2D>();
            inputType = _inputType;
            currentTile = _currentTile;

            movementSpeed = agentConfiguration.MovementSpeed;
        }

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
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
            if (!hasGotTileToMoveTo)
            {
                currentTile.VisualizeNeighbours();
                return;
            }

            if (Vector2.Distance(transform.position, direction) < 0.01f)
            {
                previousTile = currentTile;
                currentTile = tileToMoveTo;
                transform.parent = tileToMoveTo.transform;
                hasGotTileToMoveTo = false;
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
            if (hasGotTileToMoveTo)
                return tileToMoveTo.transform.position;

            foreach(Tile t in currentTile.Neighbours)
            {
                if (t.Selected)
                {
                    currentTile.StopVisualizingNeighbours();
                    tileToMoveTo = t;
                    hasGotTileToMoveTo = true;
                    return tileToMoveTo.transform.position;
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
