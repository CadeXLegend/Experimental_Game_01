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

            if (counter == 2 &&  Input.GetKeyUp(KeyCode.E))
            {
                isCounting = false;
                counter = 0;
                timer = 0.5f;
            }

            if (counter > 0 && counter < 2)
                timer -= Time.deltaTime;        

            if(counter == 2)
            {
                isCounting = true;
            }

            if (isCounting)
                Debug.Log(1);
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
