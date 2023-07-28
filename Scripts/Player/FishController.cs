using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class FishController : MonoBehaviour
{
    [Header("----- Movement Variables -----")]
    [SerializeField] private float swimSpeed = 5f;
    [SerializeField] private float landSpeed = 1f;
    [SerializeField] private float maxSwimSpeed = 10f;
    [SerializeField] private float counterMovement = 0.8f;
    [SerializeField] private LayerMask Swimmable;
    [SerializeField] private LayerMask ground;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float groundTime = 3f;
    [SerializeField] private GameObject deathEffect;
    [Header("----- Misc -----")]
    [SerializeField] private GameObject afterImageAsset;
    [Header("----- Components -----")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spr;
    [SerializeField] private ParticleSystem sweatOnGround;
    [SerializeField] private afterImage af;

    private Image breathMeter;

    private float groundTimeRef = 2f;

    private Vector2 moveInput;
    private float orgSwimSpeed;

    private bool dying = false;

    private bool canMove = true;

    private bool isSprinting = false;
    private bool isPressingSprint = false;

    private float multiplier = 1f;

    public PlayerInputs input { get; private set; } = null;

    private void Awake()
    {
        orgSwimSpeed = swimSpeed;
        groundTimeRef = groundTime;

        input = new PlayerInputs();
        breathMeter = GameObject.FindGameObjectWithTag("breath").GetComponent<Image>();

    }

    private void OnEnable()
    {
        moveInput = Vector2.zero;
        rb.velocity = Vector2.zero;
        isPressingSprint = false;
        //Swim
        input.Player.Swim.performed += swimPerformed;
        input.Player.Swim.canceled += swimCanceled;
        //Sprint
        input.Player.Dash.performed += sprintStarted;
        input.Player.Dash.canceled += sprintCanceled;
    }

    private void OnDisable()
    {
        
        //Swim
        input.Player.Swim.performed -= swimPerformed;
        input.Player.Swim.canceled -= swimCanceled;
        //Sprint
        input.Player.Dash.performed -= sprintStarted;
        input.Player.Dash.canceled -= sprintCanceled;
    }
    public void setPlayerInputs(PlayerInputs pl)
    {
        input = pl;
    }

    private void Update()
    {
        setAnimations();
        outOfWater();
        swimSprint();
        //changeOffset();
    }

    private void FixedUpdate()
    {
        if (!canMove) return; 
        fishMovement();
        applyMaxSpeed();
        flipCharacter();
    }
    public void fixGroundTimeRef()
    {
        groundTimeRef = groundTime;
    }

    public void switchToFishMode()
    {
        rb.gravityScale = 0f;
        anim.SetLayerWeight(0,0);
        anim.SetLayerWeight(1, 1);
        af.setAfterImage(afterImageAsset);
    }

    

    public float getMeterValue()
    {
        return groundTimeRef / groundTime;
    }

    private bool isInWater()
    {
        return Physics2D.CircleCast(transform.position, 0.3f, Vector2.zero, 1f, Swimmable);
    }
    private bool grounded()
    {
        return Physics2D.CircleCast(transform.position, 0.5f, Vector2.zero, 1f, ground);
    }

    private void swimPerformed(InputAction.CallbackContext value)
    {
        moveInput = value.ReadValue<Vector2>();
    }
    private void swimCanceled(InputAction.CallbackContext value)
    {
        moveInput = Vector2.zero;
    }

    private void fishMovement()
    {

        if (isSprinting)
        {
            swimSpeed = sprintSpeed;
        }
        else
        {
            swimSpeed = orgSwimSpeed;
        }

        rb.AddForce(moveInput.normalized * swimSpeed * Time.fixedDeltaTime); // normal movement
        rb.AddForce(-rb.velocity * counterMovement); // counter movement

        if (!isInWater())
        {
            rb.gravityScale = 5f;
        }
        else
        {
            rb.gravityScale = 0f;
        }

        if(!isInWater() && grounded())
        {
            swimSpeed = landSpeed;
        }
        else
        {
            swimSpeed = orgSwimSpeed;
        }

        //Debug.Log(!isInWater() && grounded());

    }

    private void applyMaxSpeed()
    {
        if (isSprinting)
        {
            multiplier = 1.4f;
        }
        else
        {
            multiplier = 1f;
        }

        if (isInWater())
        {

            rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSwimSpeed * multiplier);
        }
        else if(!isInWater() && grounded())
        {
            rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSwimSpeed * 0.3f, maxSwimSpeed * 0.3f), rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSwimSpeed, maxSwimSpeed), rb.velocity.y);
        }
    }
        

    private void setAnimations()
    {
        anim.SetBool("isSwimming", moveInput.x != 0 || moveInput.y != 0);
        anim.SetBool("fishGrounded", grounded() && !isInWater());
        var pc = sweatOnGround.emission;
        pc.enabled = grounded() && !isInWater();
    }

    private void flipCharacter()
    {
        if (moveInput.x > 0)
        {
            spr.flipX = false;
        }
        else if (moveInput.x < 0)
        {
            spr.flipX = true;
        }
    }

    private void restrictMovement()
    {
        canMove = !canMove;
    }

    private void outOfWater()
    {
        if(!isInWater() && grounded())
        {
            groundTimeRef -= Time.deltaTime;
        }
        else
        {
            groundTimeRef = Mathf.MoveTowards(groundTimeRef, groundTime, 5f * Time.deltaTime);
        }

        if(groundTimeRef <= 0 && !dying)
        {
            // destroy player here
            die();
        }
    }

    private void sprintStarted(InputAction.CallbackContext value)
    {
        isPressingSprint = true;
    }

    private void sprintCanceled(InputAction.CallbackContext value)
    {
        isPressingSprint = false;
    }

    private void swimSprint()
    {
        isSprinting = isPressingSprint && isInWater();
        if(isSprinting)
        {
            af.enabled = true;
        }
        else
        {
            af.enabled = false;
        }
    }

    public void die()
    {
        if (dying) return;
        breathMeter.gameObject.SetActive(false);
        anim.enabled = false;
        spr.enabled = false;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        af.enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        sweatOnGround.gameObject.SetActive(false);
        restrictMovement();
        StartCoroutine(deathTime());
    }

    private IEnumerator deathTime()
    {
        dying = true;
        Instantiate(deathEffect, transform.position, deathEffect.transform.rotation);
        yield return new WaitForSeconds(1f);
        SceneTransitioner.Singleton.resetScene();

    }

}
