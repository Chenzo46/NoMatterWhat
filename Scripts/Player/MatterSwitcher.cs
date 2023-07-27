using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MatterSwitcher : MonoBehaviour
{
    [SerializeField] private areaData groundData;
    [SerializeField] private areaData waterData;
    [SerializeField] private LayerMask switchLayer;
    [SerializeField] private LayerMask endLayer;
    [SerializeField] private GameObject tooltip;
    [SerializeField] private BoxCollider2D bxc;
    [SerializeField] private AudioClip rippleSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private GameObject interactGraphic;
    [SerializeField] private Transform pickupArea;

    private Transform checkpointPosition;
    private Vector2 orgSize;
    private Vector2 fish_size = new Vector2(0.7192169f, 0.7138041f);

    public enum PlayerState { Normal, Fish, Bird }

    private PlayerState currentPlayerState = PlayerState.Normal;

    private FishController FC;
    private PlayerController PC;

    private matterPortal mp;

    private bool nextToEnd = false;

    private float TimeSinceLastUsedMeter = 5f;

    private Image breathMeter;
    private CanvasGroup meterGroup;

    private PlayerInputs input;

    public bool isDying { get; private set; } = false;

    private Vector2 oppositeBoxPos;
    private Vector2 orgBoxPos;
    private SpriteRenderer spr;

    // Start is called before the first frame update
    void Awake()
    {
        FC = GetComponent<FishController>();
        PC = GetComponent<PlayerController>();
        groundData = GameObject.FindGameObjectWithTag("ground_data").GetComponent<areaData>();
        waterData = GameObject.FindGameObjectWithTag("water_data").GetComponent<areaData>();
        orgSize = bxc.size;
        try
        {
            if (SaveDataManager.Singleton.getCurrentLevelData().getReachedCheckpoint())
            {
                checkpointPosition = GameObject.FindGameObjectWithTag("checkpoint").transform;
                transform.position = checkpointPosition.position;
            }
        }
        catch (System.NullReferenceException)
        {
            Debug.Log("Save Data manager singleton is null, no variables will be saved in scene.");
        }

        breathMeter = GameObject.FindGameObjectWithTag("breath").GetComponent<Image>();
        meterGroup = breathMeter.transform.parent.GetComponent<CanvasGroup>();
        meterGroup.alpha = 0f;

        input = new PlayerInputs();

        PC.setPlayerInputs(input);
        FC.setPlayerInputs(input);
    }
    private void OnEnable()
    {
        input.Enable();

        input.Player.Interact.performed += interactPerformed;
        input.Player.Reset.performed += resetPlayer;
    }

    private void OnDisable()
    {
        input.Disable();

        input.Player.Interact.performed -= interactPerformed;
        input.Player.Reset.performed -= resetPlayer;
    }

    private void Start()
    {
        CameraFollow.Singleton.setTarget(transform);
        oppositeBoxPos = new Vector2(-pickupArea.localPosition.x, pickupArea.localPosition.y);
        orgBoxPos = pickupArea.localPosition;
        spr = GetComponent<SpriteRenderer>();

        
    }

    private void changeBreathMeter()
    {
        breathMeter.transform.localScale = new Vector3(FC.getMeterValue(), 1f, 1f);
        if (FC.getMeterValue() != 1)
        {
            TimeSinceLastUsedMeter = 0f;
        }
        else
        {
            TimeSinceLastUsedMeter += Time.deltaTime;
        }

        if (TimeSinceLastUsedMeter >= 1.3f)
        {
            meterGroup.alpha = Mathf.MoveTowards(meterGroup.alpha, 0, 1f * Time.deltaTime);
            //Debug.Log("meter fade out");
        }
        else
        {
            meterGroup.alpha = Mathf.MoveTowards(meterGroup.alpha, 1, 3.5f * Time.deltaTime);
            //Debug.Log("meter fade in");
        }
    }

    private bool inSwitchArea()
    {
        Collider2D temp = Physics2D.OverlapCircle(transform.position, 1f, switchLayer);
        if(temp != null) mp = temp.GetComponent<matterPortal>();
        return temp != null;
    }

    // Update is called once per frame
    void Update()
    {
        changeBreathMeter();
        applyCurrentState();
        isnextToEnd();
        setLookPos();
        handlePause();
    }

    private void setLookPos()
    {
        if (spr.flipX)
        {
            pickupArea.localPosition = oppositeBoxPos;
        }
        else
        {
            pickupArea.localPosition = orgBoxPos;
        }
    }

    private void handlePause()
    {
        if (PauseMenu.Singleton.isPaused)
        {
            PC.input.Disable();
            FC.input.Disable();
        }
        else
        {
            PC.input.Enable();
            FC.input.Enable();
        }
    }

    private void resetPlayer(InputAction.CallbackContext value)
    {
        killPlayer();
    }

    private void interactPerformed(InputAction.CallbackContext val)
    {
        matterToggler();
        isnextToEnd();
    }

    private void matterToggler()
    {
        if (currentPlayerState == PlayerState.Normal && inSwitchArea() && mp.getActiveState())
        {
            currentPlayerState = PlayerState.Fish;
            transform.position = mp.getFishPos();
            PC.softDropBox();
            FC.switchToFishMode();
            bxc.size = fish_size;
            groundData.toggleMatter();
            waterData.toggleMatter();
            tryPlayAudio(rippleSound);
            RainObj.toggleType();
        }
        else if (currentPlayerState == PlayerState.Fish && inSwitchArea() && mp.getActiveState())
        {
            currentPlayerState = PlayerState.Normal;
            transform.position = mp.transform.position;
            PC.switchToPlayerMode();
            bxc.size = orgSize;
            groundData.toggleMatter();
            waterData.toggleMatter();
            tryPlayAudio(rippleSound);
            RainObj.toggleType();
        }
    }

    private void applyCurrentState()
    {
        interactGraphic.SetActive((nextToEnd && currentPlayerState == PlayerState.Normal) || (inSwitchArea() && mp != null && mp.getActiveState()));

        if (currentPlayerState == PlayerState.Normal)
        {
            PC.enabled = true;
            FC.fixGroundTimeRef();
            FC.enabled = false;
        }
        else if (currentPlayerState == PlayerState.Fish)
        {
            PC.enabled = false;
            FC.enabled = true;

        }
        
    }


    private void isnextToEnd()
    {
        Collider2D  cl = Physics2D.OverlapCircle(transform.position, 1f, endLayer);

        if (cl != null && cl.GetComponent<endLevel>().getActiveState())
        {
            nextToEnd = true;
        }
        else
        {
            nextToEnd = false;
        }

        if (currentPlayerState == PlayerState.Normal && cl != null && cl.GetComponent<endLevel>().getActiveState())
        {
            cl.gameObject.GetComponent<endLevel>().finishLevel();
            PC.restricMovement();
        }
        
    }

    public void killPlayer()
    {
        isDying = true;
        if (currentPlayerState == PlayerState.Normal)
        {
            PC.die();
            tryPlayAudio(deathSound);
        }
        else if(currentPlayerState == PlayerState.Fish)
        {
            FC.die();
            tryPlayAudio(deathSound);
        }
    }

    public void killPlayer(PlayerState state)
    {
        isDying = true;
        if (state == PlayerState.Normal)
        {
            PC.die();
            tryPlayAudio(deathSound);
        }
        else if (state == PlayerState.Fish)
        {
            FC.die();
            tryPlayAudio(deathSound);
        }
    }

    private void tryPlayAudio(AudioClip clip)
    {
        if (AudioManager.Singleton == null || clip == null) { return; }
        AudioManager.Singleton.playSound(clip, AudioManager.SoundType.SFX);
    }

    public PlayerState getCurrentState()
    {
        return currentPlayerState;
    }


}