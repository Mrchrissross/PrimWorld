using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Speeds: ")]                           
    [Tooltip("This is the speed of the player."), Range(0, 1)]
    public float playerSpeed = 1f;
    [Tooltip("The height that the player can jump."), Range(1, 10)]
    public float jumpHeight = 4.5f;

    private Rigidbody   rb;
    private Vector3     velocity = Vector3.zero;
    private float       xMov;
    private bool        isGrounded,                 
                        jump;                       

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

        if (Physics.Raycast(transform.position, direction, 1.2f))
            isGrounded = true;
        else
            isGrounded = false;

        if (isGrounded)
            if (Input.GetButtonDown("Jump"))
                jump = true;
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
            GetComponent<Rigidbody>().AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
            jump = false;
        }
        #endregion
    }
}
