using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class musicTrackManager : MonoBehaviour
{
    public MusicCollection musicCollection;
    public static musicTrackManager Singleton;
    private void Awake()
    {
        if(Singleton != null) 
        {
            Destroy(gameObject); 
        }
        else
        {
            Singleton = this;
        }
        
        DontDestroyOnLoad(gameObject);
        
        musicCollection.setParent(transform);
    }

    private void OnEnable()
    {
        musicCollection.mono = this;
        //musicCollection.playSong("title");
        
    }

    public Music getSong(string songName)
    {
        return musicCollection.getSong(songName);
    }

    #region Music/track classes
    [System.Serializable]
    public class MusicCollection
    {
        [SerializeField] private List<Music> musicList;
        private Transform musicParent;
        public MonoBehaviour mono { get; set; }
        public Music currentlyPlayingMusic { get; set; }

        public void setParent(Transform parent)
        {
            musicParent = parent;
        }

        public Music getSong(string name)
        {
            foreach (Music m in musicList)
            {
                if (m.songName.Equals(name))
                {
                    return m;
                }
            }

            throw new SongNotFoundException($"Song name '{name}' not found.");
        }

        public void playSong(string name)
        {
            foreach(Music m in musicList)
            {
                if (m.songName.Equals(name))
                {
                    m.generateAudioSources(musicParent);
                    currentlyPlayingMusic = m;
                }
            }
        }

        public void changeCurrentSongVolume(float newVolume)
        {
            currentlyPlayingMusic.currentlyPlayingType.audioSource.volume = newVolume;
        }

        #region public fade coroutine calls
        public void crossFadeSong(Music from, Music to, float speed, bool wait)
        {
            mono.StartCoroutine(crossFade(from, to, speed, wait));
        }
        public void crossFadeSongType(Music from, string typeName, float speed, bool wait)
        {
            mono.StartCoroutine(crossFadeType(from, typeName, speed, wait));
        }
        public void fadeOutSong(float speed)
        {
            mono.StartCoroutine(fadeOut(speed));
        }
        public void fadeInSong(Music musicToFade, float speed)
        {
            mono.StartCoroutine(fadeIn(musicToFade, speed));
        }
        #endregion
        #region fade coroutines
        private IEnumerator crossFade(Music from, Music to, float speed, bool wait)
        {
            float savedVolume = SaveDataManager.Singleton.gameStateVariables.getFloat("music_volume");
            float to_volume = 0;
            float from_volume = from.getCurrentVolume();
            
            if (!wait)
            {
                to.generateAudioSources(musicParent);
                to.changeVolume(0f);
                while (to_volume < savedVolume)
                {
                    yield return new WaitForEndOfFrame();
                    to_volume = Mathf.MoveTowards(to_volume, savedVolume, speed * Time.deltaTime);
                    from_volume = savedVolume - to_volume;

                    from.changeVolume(from_volume);
                    to.changeVolume(to_volume);

                }
            }
            else if(wait)
            {
                while (from_volume > 0)
                {
                    yield return new WaitForEndOfFrame();
                    from_volume = Mathf.MoveTowards(from_volume, 0f, speed * Time.unscaledDeltaTime);
                    from.changeVolume(from_volume);
                }
                to.generateAudioSources(musicParent);
                to.changeVolume(0f);
                while (to_volume < savedVolume)
                {
                    yield return new WaitForEndOfFrame();
                    to_volume = Mathf.MoveTowards(to_volume, savedVolume, speed * Time.unscaledDeltaTime);
                    to.changeVolume(to_volume);
                }
            }
            Debug.Log("Fade ending");
            currentlyPlayingMusic = to;
            from.deleteAudioSources();

        }

        private IEnumerator fadeOut(float speed)
        {
            float from_volume = currentlyPlayingMusic.getCurrentVolume();
            float to_volume = 0f;
            while (from_volume > to_volume)
            {
                yield return new WaitForEndOfFrame();
                from_volume = Mathf.MoveTowards(from_volume, to_volume, speed * Time.unscaledDeltaTime);
                currentlyPlayingMusic.changeVolume(from_volume);
            }
            
            currentlyPlayingMusic.deleteAudioSources();
            currentlyPlayingMusic = null;
        }
        private IEnumerator fadeIn(Music newMusic, float speed)
        {
            float from_volume = 0f;
            float to_volume = SaveDataManager.Singleton.gameStateVariables.getFloat("music_volume");
            newMusic.generateAudioSources(musicParent, from_volume);
            while (from_volume < to_volume)
            {
                yield return new WaitForEndOfFrame();
                currentlyPlayingMusic = newMusic;
                from_volume = Mathf.MoveTowards(from_volume, to_volume, speed * Time.unscaledDeltaTime);
                
                newMusic.changeVolume(from_volume);
            }
        }

        private IEnumerator crossFadeType(Music current, string typeName, float speed, bool wait)
        {
            float savedVolume = SaveDataManager.Singleton.gameStateVariables.getFloat("music_volume");
            float to_volume = 0;
            float from_volume = current.getCurrentVolume();
            MusicType nextType = current.getType(typeName);

            if(current.currentlyPlayingType == nextType) { yield break; }

            nextType.changeVolume(0f);

            if (!wait)
            {
                while (to_volume < savedVolume)
                {
                    yield return new WaitForEndOfFrame();
                    to_volume = Mathf.MoveTowards(to_volume, savedVolume, speed * Time.unscaledDeltaTime);
                    from_volume = savedVolume - to_volume;

                    current.changeVolume(from_volume);
                    nextType.changeVolume(to_volume);
                }
            }
            else if(wait)
            {
                while (from_volume > 0)
                {
                    yield return new WaitForEndOfFrame();
                    from_volume = Mathf.MoveTowards(from_volume, 0, speed * Time.unscaledDeltaTime);
                    current.changeVolume(from_volume);
                }
                while (to_volume < savedVolume)
                {
                    yield return new WaitForEndOfFrame();
                    to_volume = Mathf.MoveTowards(to_volume, savedVolume, speed * Time.unscaledDeltaTime);
                    nextType.changeVolume(to_volume);
                }
            }
            
            current.currentlyPlayingType = nextType;

        }
        #endregion
    }
    [System.Serializable]
    public class Music
    {
        public string songName = "";
        public List<MusicType> types = new List<MusicType>();
        private GameObject trackParent;
        public MusicType currentlyPlayingType { get; set; }
        public void generateAudioSources(Transform manager)
        {
            float savedVolume = SaveDataManager.Singleton.gameStateVariables.getFloat("music_volume");
            GameObject songParent = new GameObject(songName);
            songParent.transform.SetParent(manager);
            trackParent = songParent;
            foreach (MusicType m in types)
            {
                AudioSource ad = new GameObject(m.typeName).AddComponent<AudioSource>();
                ad.transform.SetParent(songParent.transform);
                m.audioSource = ad;
                ad.clip = m.clip;
                ad.loop = true;
                ad.Play();

                if (m.isMainType)
                {
                    ad.volume = savedVolume;
                    currentlyPlayingType = m;
                }
                else
                {
                    ad.volume = 0f;
                }
            }
        }

        public void generateAudioSources(Transform manager, float startVolume)
        {
            GameObject songParent = new GameObject(songName);
            songParent.transform.SetParent(manager);
            trackParent = songParent;
            foreach (MusicType m in types)
            {
                AudioSource ad = new GameObject(m.typeName).AddComponent<AudioSource>();
                ad.transform.SetParent(songParent.transform);
                m.audioSource = ad;
                ad.clip = m.clip;
                ad.loop = true;
                ad.Play();

                if (m.isMainType)
                {
                    ad.volume = startVolume;
                    currentlyPlayingType = m;
                    Debug.Log("you");
                }
                else
                {
                    ad.volume = 0f;
                }
            }
        }

        public void deleteAudioSources()
        {
            Destroy(trackParent);
        }

        public float getCurrentVolume() => currentlyPlayingType.audioSource.volume;

        public void changeVolume(float newVolume)
        {
            currentlyPlayingType.changeVolume(newVolume);
        }

        public MusicType getType(string typeName)
        {
            foreach(MusicType m in types)
            {
                if (m.typeName.Equals(typeName))
                {
                    return m;
                }
            }

            throw new SongNotFoundException($"Song type '{typeName}' not found.");
        }


    }
    [System.Serializable]
    public class MusicType
    {
        public string typeName = "";
        public AudioClip clip;
        public AudioSource audioSource { get; set; }
        public bool isMainType = false;

        public void changeVolume(float newVolume)
        {
            audioSource.volume = newVolume;
        }
    }
    private class SongNotFoundException : System.Exception
    {
        public SongNotFoundException(string message) : base(message) { }
    }
    #endregion
}
