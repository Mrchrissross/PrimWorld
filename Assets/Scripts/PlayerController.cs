using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Speeds: ")]                           
        [Tooltip("This is how fast the player can run."), Range(0, 1)]
        public float playerRunningSpeed = 1f;

        [Tooltip("This is the speed that the player will walk."), Range(0, 1)]
        public float playerWalkingSpeed = 0.1f;

        [Tooltip("Used to disable and enable player movement.")]
        public bool movementEnabled = true;

    [Header("Health: ")]
        [Tooltip("This is the number of hits your character can take before dying.")]
        public int health = 3;

        [Tooltip("This is how far the player will be knocked back when hit."), Range(1, 3)]
        public float knockbackForce = 2.0f;

    [Tooltip("If enabled, the player will be invincible and cannot be harmed.")]
        public bool Invincible;

    [Header("Jumping:")]
        [Tooltip("Number of times that the player is allowed to jump."), Range(1, 2)]
        public int allowedJumps = 1;

        [Tooltip("The height that the player can jump."), Range(1, 10)]
        public float jumpHeight = 4.5f;

    [Header("Crouching:")]
        [Tooltip("This shows whether the player is crouching.")]
        public bool crouching;

        [Tooltip("This is the speed that the player can crawl."), Range(0.1f, 1)]
        public float crawlSpeed = 0.3f;

    private Rigidbody   rb;
    private Vector3     velocity = Vector3.zero;
    private Animator    anim;
    private List<string> collectedMagic = new List<string>();
    private float       xMov,
                        stuckMobTimer = 0.0f,
                        otherXPos;
    private int         jumps;
    private bool        isGrounded,
                        jump,
                        jumpAnim,
                        updateScore,
                        hit,
                        endHitAnim;

    void Start ()
    {
        rb = GetComponent<Rigidbody>();
        anim = transform.Find("Animator").GetComponent<Animator>();

        for (int i = 0; i < DataManager.CollectedMagic.Count; i++)
            if (!collectedMagic.Contains(DataManager.CollectedMagic[i]))
                collectedMagic.Add(DataManager.CollectedMagic[i]);
    }

	void Update ()
    {
        #region Movement
        if (Input.GetButton("Crouch"))
            crouching = true;
        else
            crouching = false;

        if (movementEnabled)
            xMov = Input.GetAxis("Horizontal");

        Vector3 horizontalMovement = transform.right * xMov;

        if (Input.GetKey(KeyCode.LeftShift))
            velocity = (horizontalMovement) * (playerWalkingSpeed * 10);
        else
        {
            if (crouching)
                velocity = (horizontalMovement) * (crawlSpeed * 10);
            else
                velocity = (horizontalMovement) * (playerRunningSpeed * 10);
        }
        #endregion

        #region Jumping
        Vector3 direction = transform.TransformDirection(Vector3.down);

        if ((Physics.Raycast(transform.position + new Vector3(-0.35f, 0, 0), direction, 1.45f)) || (Physics.Raycast(transform.position + new Vector3(0.35f, 0, 0), direction, 1.45f)))
        {
            if (jumpAnim)
            {
                anim.SetBool("Jumping", false);
                jumpAnim = false;
            }
        }
        else
        {
            jumpAnim = true;
            anim.SetBool("Jumping", true);
        }

        if ((Physics.Raycast(transform.position + new Vector3(-0.35f, 0, 0), direction, 0.55f)) || (Physics.Raycast(transform.position + new Vector3(0.35f, 0, 0), direction, 0.55f)))
        {
            jumps = 0;
            isGrounded = true;
            anim.SetBool("Jumping", false);
        }
        else 
            isGrounded = false;

        if ((isGrounded) || (jumps < allowedJumps))
            if (movementEnabled)
                if (Input.GetButtonDown("Jump"))
                {
                    if(!crouching)
                        jump = true;
                }
        #endregion

        #region Health
        if (!Invincible)
        {
            if (hit == true)
            {
                StartCoroutine(UseAnimation(0.40f, "Hit"));

                if (otherXPos >= transform.position.x)
                {
                    GetComponent<Rigidbody>().AddForce(Vector3.up * knockbackForce, ForceMode.Impulse);
                    //GetComponent<Rigidbody>().AddForce(Vector3.right * -knockbackForce, ForceMode.VelocityChange);
                    xMov = -1;
                }
                else
                {
                    GetComponent<Rigidbody>().AddForce(Vector3.up * knockbackForce, ForceMode.Impulse);
                    //GetComponent<Rigidbody>().AddForce(Vector3.right * knockbackForce, ForceMode.VelocityChange);
                    xMov = 1;
                }

                health -= 1;
                hit = false;
            }

            if (health == 0)
                DataManager.GameOver = true;
        }
        #endregion

        #region Scoring
        if (updateScore)
        {
            for (int i = 0; i < collectedMagic.Count; i++)
                DataManager.CollectedMagic.Add(collectedMagic[i]);

            updateScore = false;
        }
        #endregion
    }

    void FixedUpdate()
    {
        #region Movement
        if (crouching)
            anim.SetBool("Crouching", true);
        else
            anim.SetBool("Crouching", false);

        if (velocity != Vector3.zero)
        {
            if (velocity.x < 0)
            {
                if(movementEnabled)
                    transform.Find("Animator").localScale = new Vector3(-1, 1, 1);

                transform.Find("Animator").position = transform.position + new Vector3(0.07f, 0, 0);
            }
            else
            {
                if (movementEnabled)
                    transform.Find("Animator").localScale = new Vector3(1, 1, 1);

                transform.Find("Animator").position = transform.position;
            }

            if (movementEnabled)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    if (crouching)
                        anim.SetBool("Crawling", true);
                    else
                    {
                        anim.SetBool("Walking", true);
                        anim.SetBool("Running", false);
                        anim.SetBool("Crawling", false);
                    }
                }
                else
                {
                    if (crouching)
                        anim.SetBool("Crawling", true);
                    else
                    {
                        anim.SetBool("Running", true);
                        anim.SetBool("Crawling", false);
                    }
                }
            }

            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }
        else
        {
            anim.SetBool("Crawling", false);
            anim.SetBool("Walking", false);
            anim.SetBool("Running", false);
        }
        #endregion

        #region Jumping
        if (jump)
        {
            anim.SetBool("Jumping", true);
            if (jumps < allowedJumps)                                                                              
            {
                if(jumps >= 1)
                    GetComponent<Rigidbody>().AddForce(Vector3.up * (jumpHeight / 2), ForceMode.VelocityChange);
                else
                    GetComponent<Rigidbody>().AddForce(Vector3.up * jumpHeight, ForceMode.VelocityChange);

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
            updateScore = true;
        }

        if (other.tag == "LeakedMagic")
        {
            DataManager.Score += 1;
            collectedMagic.Add(other.name);
            other.gameObject.SetActive(false);
        }

        if (other.tag == "Fallen")
            health = 0;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Enemy")
        {
            stuckMobTimer -= Time.deltaTime;
            if (stuckMobTimer < 0)
            {
                hit = true;
                otherXPos = other.transform.position.x;
                stuckMobTimer = 1.0f;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
            stuckMobTimer = 0.0f;
    }

    private IEnumerator UseAnimation(float animationLength, string animationName)
    {
        anim.SetBool(animationName, true);
        movementEnabled = false;

       yield return new WaitForSeconds(animationLength);

        anim.SetBool(animationName, false);
        velocity = Vector3.zero;
        movementEnabled = true;
    }
}
