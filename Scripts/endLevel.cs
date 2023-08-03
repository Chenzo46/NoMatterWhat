using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class endLevel : MonoBehaviour
{
    [SerializeField] private Transform orbPosition;
    [SerializeField] private Material EndMat;
    [SerializeField] private float rStrength;
    [SerializeField] private bool isActive = true;
    [SerializeField] private LayerMask box;
    [SerializeField] private Transform obsorbCenter;
    [SerializeField] private SceneType nextSceneType;
    private Animator anim;
    private BoxCollider2D bx;

    private float smoothRef;

    private enum SceneType 
    {
        plain,
        rain,
        main,
        oasis,
        sandstorm
    }


    private void Awake()
    {
        anim = GetComponent<Animator>();
        bx = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("active", isActive);
        checkForBox();

        if (Input.GetKeyDown(KeyCode.I))
        {
            finishLevel();
        }
    }

    public void finishLevel() 
    {
        MatterSwitcher.Singleton.disableMovement();
        try
        {
            songFadeHandler();
            SaveDataManager.Singleton.getCurrentLevelData().setCheckpoint(false);
            SaveDataManager.Singleton.saveData();
            SceneTransitioner.Singleton.leaveScene();
        }
        catch
        {
            Debug.Log("No Manager Functions Executed");
            SceneTransitioner.Singleton.leaveScene();
        }
    }

    private void checkForBox()
    {
        if (isActive) return;

        Collider2D boxRef = Physics2D.OverlapBox(transform.position, bx.bounds.size, 0f, box);

        if (boxRef != null)
        {
            boxBehavior temp = boxRef.GetComponent<boxBehavior>();
            temp.consumeBox();
            temp.setPickupLocation(obsorbCenter);

            isActive = true;
        }
    }

    public bool getActiveState()
    {
        return isActive;
    }

    private void songFadeHandler()
    {
        switch (nextSceneType)
        {
            case SceneType.rain:
                musicTrackManager.Singleton.musicCollection.crossFadeSongType(
                musicTrackManager.Singleton.musicCollection.currentlyPlayingMusic,
                "rain",
                0.35f,
                false);
                break;
            case SceneType.plain:
                musicTrackManager.Singleton.musicCollection.crossFadeSongType(
                musicTrackManager.Singleton.musicCollection.currentlyPlayingMusic,
                "main",
                0.35f,
                false);
                break;
            case SceneType.main:
                musicTrackManager.Singleton.musicCollection.fadeOutSong(0.35f);
                break;
        }
    }
}
