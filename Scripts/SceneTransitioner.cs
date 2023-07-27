using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitioner : MonoBehaviour
{
    private Animator anim;

    public static SceneTransitioner Singleton;

    //[SerializeField] private bool isMain = false;
    private void Awake()
    {
        Singleton = this;
        anim = GameObject.FindGameObjectWithTag("transition").GetComponent<Animator>();
    }

    public void leaveScene()
    {
        StartCoroutine(changeScene());
    }

    private IEnumerator changeScene()
    {
        anim.SetTrigger("exit");
        yield return new WaitForSecondsRealtime(1.3f);
        if(SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            leaveScene();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    private IEnumerator toMain()
    {
        anim.SetTrigger("exit");
        yield return new WaitForSecondsRealtime(1.3f);
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void startGame()
    {
        musicTrackManager.Singleton.musicCollection.crossFadeSong(
            musicTrackManager.Singleton.musicCollection.currentlyPlayingMusic,
            musicTrackManager.Singleton.getSong("matter"),
            0.4f,
            true);
        leaveScene();
    }

    public void resetScene()
    {
         StartCoroutine(reloadScene());
    }

    public void toMainMenu()
    {
        StartCoroutine(toMain());
    }

    private IEnumerator reloadScene()
    {
        anim.SetTrigger("exit");
        yield return new WaitForSecondsRealtime(1.3f);
        if (RainObj.currentRainType == RainObj.RainType.Ice) { RainObj.toggleType(); }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
