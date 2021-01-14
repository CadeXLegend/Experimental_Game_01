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
        private void Update()
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

            transform.position = Vector2.LerpUnclamped(transform.position, direction, movementSpeed * Time.deltaTime);
        }

        private void ValueFromInput()
        {
            if (inputType.HasFlag(TypeOfInput.Keyboard))
                directionToMoveTo = KeyboardInputToVector2();
            if (inputType.HasFlag(TypeOfInput.ClickOnGrid))
                directionToMoveTo = ClickOnGridInputToVector2();

            if(Input.GetKeyDown(KeyCode.Keypad0))
                parent.ProcessAction();
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
                    if (t.IsOccupied || t.IsOccupiedByPlayer)
                        continue;

                    tileToMoveTo = t;
                    hasGotTileToMoveTo = true;
                    return tileToMoveTo.transform.position;
                }
            }
            hasGotTileToMoveTo = false;
            return currentTile.transform.position;
        }

        private Vector2 KeyboardInputToVector2()
        {
            if (hasGotTileToMoveTo)
                return tileToMoveTo.transform.position;

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                return GetTileToMoveToPos(TileNeighbour.NeighbourOrientation.Up);
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                return GetTileToMoveToPos(TileNeighbour.NeighbourOrientation.Left);
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                return GetTileToMoveToPos(TileNeighbour.NeighbourOrientation.Down);
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                return GetTileToMoveToPos(TileNeighbour.NeighbourOrientation.Right);
            }
            else if (Input.GetKey(KeyCode.W) && (Input.GetKey(KeyCode.A) 
                || Input.GetKey(KeyCode.UpArrow)) && Input.GetKey(KeyCode.LeftArrow))
            {
                return GetTileToMoveToPos(TileNeighbour.NeighbourOrientation.TopLeft);
            }
            else if (Input.GetKey(KeyCode.W) && (Input.GetKey(KeyCode.D) 
                || Input.GetKey(KeyCode.UpArrow)) && Input.GetKey(KeyCode.RightArrow))
            {
                return GetTileToMoveToPos(TileNeighbour.NeighbourOrientation.TopRight);
            }
            else if (Input.GetKey(KeyCode.S) && (Input.GetKey(KeyCode.A) 
                || Input.GetKey(KeyCode.DownArrow)) && Input.GetKey(KeyCode.LeftArrow))
            {
                return GetTileToMoveToPos(TileNeighbour.NeighbourOrientation.BottomLeft);
            }
            else if (Input.GetKey(KeyCode.S) && (Input.GetKey(KeyCode.D) 
                || Input.GetKeyDown(KeyCode.DownArrow)) && Input.GetKeyDown(KeyCode.RightArrow))
            {
                return GetTileToMoveToPos(TileNeighbour.NeighbourOrientation.BottomRight);
            }

            return currentTile.transform.position;
        }

        private Vector3 GetTileToMoveToPos(TileNeighbour.NeighbourOrientation orientation)
        {
            TileNeighbour t = currentTile.Neighbours.First(n => n.neighbourOrientation == orientation);
            return ComputeValidityOfTileMoves(t);
        }

        private Vector3 ComputeValidityOfTileMoves(TileNeighbour t)
        {
            if (!t.NeighbourTile || t.NeighbourTile.IsOccupied || t.NeighbourTile.IsOccupiedByPlayer)
                return currentTile.transform.position;

            tileToMoveTo = t.NeighbourTile;
            hasGotTileToMoveTo = true;

            return tileToMoveTo.transform.position;
        }
    }
}
