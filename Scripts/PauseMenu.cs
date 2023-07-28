using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public bool isPaused { get; private set; } = false;
    private Animator anim;
    [SerializeField] private CanvasGroup levelName;
    [SerializeField] private CanvasGroup meterFrame;
    [SerializeField] private GameObject firstSelected;
    [SerializeField] private Animator optionsMenuAnim;
    [SerializeField] private Sprite FishPause;
    [SerializeField] private Sprite BirdPause;
    [SerializeField] private Sprite PlayerPause;
    [SerializeField] private Image pauseBack;

    private MatterSwitcher mt;

    private PlayerInputs input;

    public static PauseMenu Singleton;

    private bool inOptions = false;

    private void Awake()
    {
        Singleton = this;
        anim = GetComponent<Animator>();
        mt = GameObject.FindGameObjectWithTag("Player").GetComponent<MatterSwitcher>();
        input = new PlayerInputs();
    }

    private void OnEnable()
    {
        input.Enable();

        input.Player.Pause.performed += pausePerformed;
    }

    private void OnDisable()
    {
        input.Disable();

        input.Player.Pause.performed -= pausePerformed;
    }

    private void Update()
    {
        if (isPaused)
        {
            meterFrame.alpha = Mathf.MoveTowards(meterFrame.alpha, 1f, 2f * Time.unscaledDeltaTime);
        }
    }

    private void pausePerformed(InputAction.CallbackContext value)
    {
        if (!isPaused && anim.GetCurrentAnimatorStateInfo(0).normalizedTime! > 1 && !mt.isDying)
        {
            pause();

        }
        else if (isPaused && anim.GetCurrentAnimatorStateInfo(0).normalizedTime! > 1)
        {
            unPause();

        }

        
    }
    public void pause()
    {
        Time.timeScale = 0f;
        EventSystem.current.SetSelectedGameObject(firstSelected);
        anim.SetTrigger("pause");
        isPaused = true;
        
        if(mt.getCurrentState() == MatterSwitcher.PlayerState.Normal)
        {
            pauseBack.sprite = PlayerPause;
        }
        else if (mt.getCurrentState() == MatterSwitcher.PlayerState.Fish)
        {
            pauseBack.sprite = FishPause;
        }
    }

    public void unPause()
    {
        if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime! > 1 && !inOptions)
        {
            Time.timeScale = 1f;
            anim.SetTrigger("pause");
            isPaused = false;
            EventSystem.current.SetSelectedGameObject(null);
        }
        
    }

    public void toggleOptionsOpen()
    {
        optionsMenuAnim.SetTrigger("open");
        inOptions = !inOptions;
    }

    public void toMainMenu()
    {
        SceneTransitioner.Singleton.toMainMenu();
    }
}
