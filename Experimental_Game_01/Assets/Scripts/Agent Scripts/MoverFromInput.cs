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
        private Rigidbody2D rb;

        /// <summary>
        /// The configuration file containing Agent information to be applied to this class's parameters.
        /// </summary>
        [SerializeField]
        private AgentConfig agentConfiguration;
        [SerializeField]
        private float movementSpeed;
        // Start is called before the first frame update
        public void Init(AgentConfig config)
        {
            agentConfiguration = config;
            rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            movementSpeed = agentConfiguration.MovementSpeed;
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            Vector2 movementDirection = new Vector2(x, y);

            Move(movementDirection);
        }

        public virtual void Move(Vector2 direction)
        {
            if (!agentConfiguration.UsesRigidbody2D)
                return;

            rb.MovePosition((Vector2)transform.position + (direction * movementSpeed * Time.fixedDeltaTime));
        }
    }
}
