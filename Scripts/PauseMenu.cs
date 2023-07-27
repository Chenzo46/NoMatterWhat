using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public bool isPaused { get; private set; } = false;
    private Animator anim;
    [SerializeField] private CanvasGroup levelName;
    [SerializeField] private CanvasGroup meterFrame;
    [SerializeField] private GameObject firstSelected;

    private MatterSwitcher mt;

    private PlayerInputs input;

    public static PauseMenu Singleton;

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
        Debug.Log("paused");
        Time.timeScale = 0f;
        EventSystem.current.SetSelectedGameObject(firstSelected);
        anim.SetTrigger("pause");
        isPaused = true;
        levelName.alpha = 1f;
        meterFrame.alpha = 1f;
    }

    public void unPause()
    {
        if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime! > 1)
        {
            Debug.Log("unpaused");
            Time.timeScale = 1f;
            anim.SetTrigger("pause");
            isPaused = false;
            levelName.alpha = 0f;
            meterFrame.alpha = 0f;
            EventSystem.current.SetSelectedGameObject(null);
        }
        
    }

    public void toMainMenu()
    {
        SceneTransitioner.Singleton.toMainMenu();
    }
}
