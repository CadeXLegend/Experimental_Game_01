using Generation;
using System;
using System.Linq;
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

        private Vector2 directionToMoveTo = Vector2.zero;

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

            if (!hasGotTileToMoveTo)
                ValueFromInput();

            Move(directionToMoveTo);           
        }

        public virtual void Move(Vector2 direction)
        {
            if (!hasGotTileToMoveTo)
            {
                currentTile.VisualizeNeighbours();
                return;
            }
            else
                currentTile.StopVisualizingNeighbours();

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

        private void ValueFromInput()
        {
            if (inputType.HasFlag(TypeOfInput.Keyboard))
                directionToMoveTo = KeyboardInputToVector2();
            if (inputType.HasFlag(TypeOfInput.ClickOnGrid))
                directionToMoveTo = ClickOnGridInputToVector2();
        }

        private Vector2 ClickOnGridInputToVector2()
        {
            if (hasGotTileToMoveTo)
                return tileToMoveTo.transform.position;

            foreach(TileNeighbour n in currentTile.Neighbours)
            {
                Tile t = n.NeighbourTile;
                if (t.Selected)
                {
                    tileToMoveTo = t;
                    hasGotTileToMoveTo = true;
                    return tileToMoveTo.transform.position;
                }
            }
            return currentTile.transform.position;
        }

        private Vector2 KeyboardInputToVector2()
        {
            if (hasGotTileToMoveTo)
                return tileToMoveTo.transform.position;

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                TileNeighbour t = currentTile.Neighbours.First(n => n.neighbourOrientation == TileNeighbour.NeighbourOrientation.Up);
                if (t.NeighbourTile != null && !t.NeighbourTile.IsOccupied)
                {
                    tileToMoveTo = t.NeighbourTile;
                    hasGotTileToMoveTo = true;
                    return tileToMoveTo.transform.position;
                }
            }
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                TileNeighbour t = currentTile.Neighbours.First(n => n.neighbourOrientation == TileNeighbour.NeighbourOrientation.Left);
                if (t.NeighbourTile != null && !t.NeighbourTile.IsOccupied)
                {
                    tileToMoveTo = t.NeighbourTile;
                    hasGotTileToMoveTo = true;
                    return tileToMoveTo.transform.position;
                }
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                TileNeighbour t = currentTile.Neighbours.First(n => n.neighbourOrientation == TileNeighbour.NeighbourOrientation.Down);
                if (t.NeighbourTile != null && !t.NeighbourTile.IsOccupied)
                {
                    tileToMoveTo = t.NeighbourTile;
                    hasGotTileToMoveTo = true;
                    return tileToMoveTo.transform.position;
                }
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                TileNeighbour t = currentTile.Neighbours.First(n => n.neighbourOrientation == TileNeighbour.NeighbourOrientation.Right);
                if (t.NeighbourTile != null && !t.NeighbourTile.IsOccupied)
                {
                    tileToMoveTo = t.NeighbourTile;
                    hasGotTileToMoveTo = true;
                    return tileToMoveTo.transform.position;
                }
            }

            return currentTile.transform.position;
        }
    }
}
