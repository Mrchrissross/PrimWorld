using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Speeds: ")]
    [Tooltip("This is the spped of the player.")]
    public float playerSpeed = 1f;

    private Rigidbody rb;

    private bool    left,
                    right;

    void Start ()
    {
        rb = GetComponent<Rigidbody>();
	}

	void Update ()
    {
        if (Input.GetAxisRaw("Horizontal") < 0)
            left = true;
        else
            left = false;

        if (Input.GetAxisRaw("Horizontal") > 0)
            right = true;
        else
            right = false;
    }

    void FixedUpdate()
    {
        if (left)
            rb.AddForce(-(playerSpeed * 10) * Time.deltaTime, 0, 0, ForceMode.VelocityChange);

        if (right)
            rb.AddForce((playerSpeed * 10) * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
    }
}
