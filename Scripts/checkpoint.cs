using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkpoint : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private AudioClip checkpointSound;
    private SaveDataManager.Level lv;

    private void Awake()
    {
        try
        {
            lv = SaveDataManager.Singleton.getCurrentLevelData();
        }
        catch
        {
            Debug.Log("Checkpoint has no reference to SaveDataManager");
        }
    }

    private void Update()
    {
        try
        {
            anim.SetBool("pole_up", lv.getReachedCheckpoint());
        }
        catch
        {

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.tag.Equals("Player") && lv != null && !lv.getReachedCheckpoint())
        {
            if(collision.GetComponent<MatterSwitcher>().getCurrentState() == MatterSwitcher.PlayerState.Normal && !lv.getReachedCheckpoint())
            {
                lv.setCheckpoint(true);
                SaveDataManager.Singleton.saveData();
                try
                {
                    AudioManager.Singleton.playSound(checkpointSound, AudioManager.SoundType.SFX);
                }
                catch
                {

                }
            } 
        }
    }
}
