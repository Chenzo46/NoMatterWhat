using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("----- Walk / Jump -----")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpStrength = 12f;
    [SerializeField] private Transform groundCheckPosition;
    [SerializeField] private Vector2 groundCheckBounds = new Vector2();
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float jumpQueueTime = 0.1f;
    [SerializeField] private float cyoteTime = 0.1f;
    [SerializeField] private float jumpCancelSlowdown = 5f;
    [SerializeField] private float extraFallGravity = 1.2f;
    [Header("----- Dash -----")]
    [SerializeField] private float dashMultiplier = 10f;
    [SerializeField] private float dashTime = 2f;
    [SerializeField] private int dashCount = 1;
    [SerializeField] private float dashQueueTime = 0.1f;
    [Header("----- Box Pick Up -----")]
    [SerializeField] private Transform pickupPos;
    [SerializeField] private Transform pickupArea;
    [SerializeField] private float pickupRange;
    [SerializeField] private float throwStrength;
    [SerializeField] private LayerMask boxMask;
    private boxBehavior box;
    [Header("----- Sound FX -----")]
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private AudioClip throwSound;
    [Header("----- Other -----")]
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private GameObject afterImageAsset;
    [SerializeField] private GameObject jumpGas;
    [SerializeField] private GameObject groundGas;
    [Header("----- Components -----")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spr;
    [SerializeField] private Animator anim;
    [SerializeField] private ParticleSystem sweatOnGround;
    [SerializeField] private afterImage af;


    //Other private vars
    private float refSpeed;
    private float lookDir = 1;
    private float moveInput;
    private float jumpQueueRef;
    private float cyoteRef;
    private float dashTimeRef = 0;
    private float dashQueueTimeRef = 0;
    private int direction = 0;
    private int dashCountRef;
    private bool jumpQueued = false;
    private bool canMove = true;
    private bool isDashing = false;
    private bool dying = false;
    private bool holdingBox = false;
    private bool dashQueued = false;

    public PlayerInputs input { get; private set; } = null;

    #region Update_Functions

    private void Awake()
    {
        jumpQueueRef = jumpQueueTime;
        cyoteRef = cyoteTime;
        dashCountRef = dashCount;
        dashQueueTimeRef = dashQueueTime;
        refSpeed = speed;

        Application.targetFrameRate = 60;

        input = new PlayerInputs();
    }

    private void OnEnable()
    {
        moveInput = 0f;
        rb.velocity = Vector2.zero;
        //Walk
        input.Player.walk.performed += updateMovement;
        input.Player.walk.canceled += updateMovementCanceled;
        //Jump
        input.Player.Jump.performed += collectJumpInput;
        input.Player.Jump.canceled += jumpCanceled;
        //Dash
        input.Player.Dash.performed += dashPerformed;
        //Box Pickup
        input.Player.Interact.performed += interactPerformed;
    }
    private void OnDisable()
    {
        //Walk
        input.Player.walk.performed -= updateMovement;
        input.Player.walk.canceled -= updateMovementCanceled;
        //Jump
        input.Player.Jump.performed -= collectJumpInput;
        input.Player.Jump.canceled -= jumpCanceled;
        //Dash
        input.Player.Dash.performed -= dashPerformed;
        //Box Pickup
        input.Player.Interact.performed -= interactPerformed;
    }

    public void setPlayerInputs(PlayerInputs pl)
    {
        input = pl;
    }


    private void Start()
    {
        
        setAnimations();
    }
    void Update()
    {
        //Movement code
        jumpQueuerTimer();
        cyoteTimer();
        dashTimer();
        dashQueuer();
        if (!canMove) return;
        var ps = sweatOnGround.emission;
        ps.enabled = false;
    }

    private void FixedUpdate()
    {
        //Movement code
        if (!canMove) return;
        dash();
        movement();
        //applyExtraGravityAfterJump();
    }
    #endregion

    #region Movement_Code

    private void dashPerformed(InputAction.CallbackContext value)
    {
        collectDashInput();
        dashQueuerInput();
    }

    private void collectDashInput()
    {
        

        if (!isDashing && !grounded() && dashCount > 0 && !dashQueued)
        {
            isDashing = true;
            dashTimeRef = dashTime;
            direction = spr.flipX ? -1 : 1;
            dashCount -= 1;
            tryPlayAudio(dashSound);
        }
    }

    private void dash()
    {
        if (isDashing && dashTimeRef > 0)
        {
            speed = refSpeed * dashMultiplier;
            rb.velocity = new Vector2(direction * speed * Time.fixedDeltaTime, 0f);
            Debug.Log("dashing");
        }
        else
        {
            speed = refSpeed;
        }
    }
    private void dashQueuerInput()
    {
        if (!grounded() && isDashing && !dashQueued)
        {
            dashQueued = true;
        }
    }

    private void dashQueuer()
    {
        af.enabled = isDashing;

        if (dashQueued)
        {
            dashQueueTimeRef -= Time.deltaTime;
        }

        if (dashQueueTimeRef <= 0)
        {
            dashQueueTimeRef = dashQueueTime;
            dashQueued = false;
        }

        if (dashQueued && !isDashing && dashCount > 0)
        {
            isDashing = true;
            dashQueued = false;
            dashQueueTimeRef = dashQueueTime;
            dashTimeRef = dashTime;
            direction = spr.flipX ? -1 : 1;
            dashCount -= 1;
            tryPlayAudio(dashSound);
        }
    }

    private void dashTimer()
    {
        if (isDashing && dashTimeRef > 0)
        {
            dashTimeRef -= Time.deltaTime;
            rb.gravityScale = 0f;
        }

        if (dashTimeRef <= 0)
        {
            isDashing = false;
            rb.gravityScale = 5f;
        }

        if (grounded())
        {
            dashCount = dashCountRef;
        }
    }

    private void updateMovement(InputAction.CallbackContext value)
    {
        moveInput = value.ReadValue<float>();
    }
    private void updateMovementCanceled(InputAction.CallbackContext value)
    {
        moveInput = 0f;
    }

    private void movement()
    {
        flipCharacter();
        setAnimations();
        if (!isDashing)
        {
            float xSpeed = moveInput * speed * Time.fixedDeltaTime;
            rb.velocity = new Vector2(xSpeed, rb.velocity.y);
        }
    }

    private void flipCharacter()
    {
        if (moveInput > 0)
        {
            spr.flipX = false;
            lookDir = 1;
        }
        else if (moveInput < 0)
        {
            spr.flipX = true;
            lookDir = -1;
        }
    }
    #endregion

    #region Jump_Code

    private void collectJumpInput(InputAction.CallbackContext value)
    {
        baseJump();
        jumpQueuerAction();
    }
    private void jumpCanceled(InputAction.CallbackContext value)
    {
        jumpCancel();
    }

    private void baseJump()
    {
        if (cyoteRef > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * jumpStrength, ForceMode2D.Impulse);
            tryPlayAudio(jumpSound);
            cyoteRef = 0f;
            //Instantiate(jumpGas, (Vector2)transform.position - (Vector2.up * 0.3f), Quaternion.identity);
        }
    }

    private void jumpQueuerAction()
    {
        if (!grounded())
        {
            jumpQueued = true;
        }
    }
    private void jumpQueuerTimer()
    {
        if (grounded() && jumpQueued)
        {
            jumpQueued = false;
            jumpQueueRef = jumpQueueTime;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * jumpStrength, ForceMode2D.Impulse);
            tryPlayAudio(jumpSound);
        }
        if (jumpQueueRef > 0 && jumpQueued)
        {
            jumpQueueRef -= Time.deltaTime;
        }
        else if (jumpQueueRef < 0 && jumpQueued)
        {
            jumpQueueRef = jumpQueueTime;
            jumpQueued = false;
        }
    }


    private void cyoteTimer()
    {
        if (cyoteRef > 0)
        {
            cyoteRef -= Time.deltaTime;
        }
        if (grounded())
        {
            cyoteRef = cyoteTime;
        }
    }

    private void jumpCancel()
    {
        if (rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / jumpCancelSlowdown);
            cyoteRef = 0f;
        }
    }

    private void applyExtraGravityAfterJump()
    {
        if (rb.velocity.y < 0 &&  !isDashing)
        {
            rb.AddForce(Vector2.down * extraFallGravity);
        }
    }

    private bool grounded()
    {
        return Physics2D.OverlapBox(groundCheckPosition.position, groundCheckBounds, 0f, groundMask);
    }

    #endregion

    #region Player_State_Code

    public void switchToPlayerMode()
    {
        rb.gravityScale = 5f;
        anim.SetLayerWeight(0, 1);
        anim.SetLayerWeight(1, 0);
        af.setAfterImage(afterImageAsset);
        if (box != null) box.dropBox();
    }

    public void restricMovement()
    {
        canMove = !canMove;
        rb.velocity = Vector2.zero;
        moveInput = 0;
        setAnimations();
    }

    public void die()
    {
        if (dying) return;
        anim.enabled = false;
        spr.enabled = false;
        rb.isKinematic = true;
        GetComponent<BoxCollider2D>().enabled = false;
        restricMovement();
        af.enabled = false;
        rb.velocity = Vector2.zero;
        sweatOnGround.gameObject.SetActive(false);
        StartCoroutine(deathTime());
    }

    private IEnumerator deathTime()
    {
        dying = true;
        Instantiate(deathEffect, transform.position, deathEffect.transform.rotation);
        yield return new WaitForSeconds(1f);
        SceneTransitioner.Singleton.resetScene();

    }
    #endregion

    #region Box_Code

    private void interactPerformed(InputAction.CallbackContext val) 
    {
        if(holdingBox)
        {
            dropBox();
        }
        else
        {
            pickUpBox();
        }
        
    }

    private void pickUpBox()
    {
        Collider2D boxInRange = Physics2D.OverlapCircle(pickupArea.position, pickupRange, boxMask);

        
        if (boxInRange != null && !holdingBox)
        {
            Debug.Log("Box Picked up");

            tryPlayAudio(pickupSound);

            box = boxInRange.GetComponent<boxBehavior>();

            box.setPickupLocation(pickupPos);

            holdingBox = true;
        }
    }

    private void dropBox()
    {
        if(holdingBox)
        {
            tryPlayAudio(throwSound);
            box.applyForce(new Vector2(1 * lookDir, 1) * throwStrength );
            Debug.Log("Box dropped");
            box = null;
            holdingBox = false;
        }
    }

    public void softDropBox()
    {
        if (box != null) { box.dropBox();}
    }
    #endregion

    #region Public Edit Methods

    public void addToDash()
    {
        dashCount += 1;
    }

    #endregion

    #region Miscellaneous
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundCheckPosition.position, groundCheckBounds);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(pickupArea.position, pickupRange);
    }

    private void tryPlayAudio(AudioClip clip)
    {
        try
        {
            AudioManager.Singleton.playSound(clip, AudioManager.SoundType.SFX);
        }
        catch { }

    }

    private void setAnimations()
    {
        anim.SetBool("walking", moveInput != 0);
        anim.SetBool("falling", rb.velocity.y < 0);
        anim.SetBool("grounded", grounded());
    }
    #endregion

}
