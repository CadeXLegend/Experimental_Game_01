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

        private bool hasReachedDestination = false;

        public void Init(AgentConfig config)
        {
            agentConfiguration = config;
            rb = GetComponent<Rigidbody2D>();
        }

        public void Init(AgentConfig config, TypeOfInput _inputType)
        {
            agentConfiguration = config;
            rb = GetComponent<Rigidbody2D>();
            inputType = _inputType;
        }

        public void Init(AgentConfig config, TypeOfInput _inputType, Tile _currentTile)
        {
            agentConfiguration = config;
            rb = GetComponent<Rigidbody2D>();
            inputType = _inputType;
            currentTile = _currentTile;
        }

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            movementSpeed = agentConfiguration.MovementSpeed;
        }

        int counter = 0;
        float timer = 0.5f;
        bool isCounting = false;
        private void Update()
        {
            if (timer <= 0)
            {
                counter = 0;
                timer = 0.5f;
                isCounting = false;
            }

            if (Input.GetKeyDown(KeyCode.E))
                counter++;

            if (counter == 2 && Input.GetKeyUp(KeyCode.E))
            {
                isCounting = false;
                counter = 0;
                timer = 0.5f;
            }

            if (counter > 0 && counter < 2)
                timer -= Time.deltaTime;

            if (counter == 2)
            {
                isCounting = true;
            }

            if (isCounting)
                Debug.Log(1);
        }

        // Update is called once per frame
        private void FixedUpdate()
        {

            Move(ValueFromInput());
        }

        public virtual void Move(Vector2 direction)
        {
            if (!agentConfiguration.UsesRigidbody2D)
                return;

            if (Vector2.Distance(transform.position, direction) < 0.1f)
                return;

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
                    TurnTicker.Tick();
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
