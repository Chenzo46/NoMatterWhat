using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class extraDash : MonoBehaviour
{
    [SerializeField] private float respawnTime = 3f;
    [SerializeField] private AudioClip addSound;
    [SerializeField] private Animator anim;

    private float respawnTimeRef;
    private bool used = false;
    

    private void Awake() => respawnTimeRef = respawnTime;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            MatterSwitcher m_switcher = collision.GetComponent<MatterSwitcher>();
            if(m_switcher.getCurrentState() == MatterSwitcher.PlayerState.Normal)
            {
                m_switcher.GetComponent<PlayerController>().addToDash();
                try
                {
                    AudioManager.Singleton.playSound(addSound, AudioManager.SoundType.SFX);
                }
                catch
                {
                    Debug.Log("extra dash sound can't be played");
                }
                hide();
            }
        }
    }

    private void hide()
    {
        used = true;
        anim.SetBool("used", used);
    }

    private void show()
    {
        used = false;
        respawnTimeRef = respawnTime;
        anim.SetBool("used", used);
    }

    private void Update()
    {
        if (used) 
        {
            respawnTimeRef -= Time.deltaTime;
        }

        if(respawnTimeRef <= 0 && used)
        {
            show();
        }
    }
}
