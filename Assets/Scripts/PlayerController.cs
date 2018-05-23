using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Speeds: ")]                           
        [Tooltip("This is the speed of the player."), Range(0, 1)]
        public float playerSpeed = 1f;

        [Tooltip("The height that the player can jump."), Range(1, 10)]
        public float jumpHeight = 4.5f;

    [Header("Health: ")]
        [Tooltip("This the number of hits your character can take before dying.")]
        public int health = 3;

    private Rigidbody   rb;
    private Vector3     velocity = Vector3.zero;
    private float       xMov;
    private int         jumps;
    private bool        isGrounded,
                        jump,
                        hit;

    void Start ()
    {
        rb = GetComponent<Rigidbody>();
	}

	void Update ()
    {
        #region Movement
        xMov = Input.GetAxis("Horizontal");
        Vector3 horizontalMovement = transform.right * xMov;
        velocity = (horizontalMovement) * (playerSpeed * 10);
        #endregion

        #region Jumping
        Vector3 direction = transform.TransformDirection(Vector3.down);

        if (Physics.Raycast(transform.position, direction, 0.55f))                
        {
            jumps = 0;
            isGrounded = true;
        }
        else
            isGrounded = false;

        if ((isGrounded) || (jumps < 2))                                         
            if (Input.GetButtonDown("Jump"))
                jump = true;
        #endregion

        #region Health
        if(hit == true)
        {
            health -= 1;

            if (health == 0)
                DataManager.GameOver = true;

            hit = false;
        }

        #endregion
    }

    void FixedUpdate()
    {
        #region Movement
        if (velocity != Vector3.zero)
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        #endregion

        #region Jumping
        if (jump)
        {
            if (jumps < 2)                                                                              
            {
                GetComponent<Rigidbody>().AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
                jumps += 1;                                                                                 
                jump = false;
            }
        }
        #endregion
    }
                                                               
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "SpawnPoint")
        {
            DataManager.PlayerSpawnPosition = other.transform;
            DataManager.SpawnLocation = other.name;
        }

        if(other.tag == "Enemy")
            hit = true;

        if (other.tag == "LeakedMagic")
        {
            Destroy(other.gameObject);
            DataManager.Score += 1;
        }
    }
}
