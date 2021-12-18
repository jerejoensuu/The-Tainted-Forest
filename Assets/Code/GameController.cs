using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public bool splashScreenPlayed = false;

    public AudioClip mainTheme;
    public AudioClip cutsceneTheme;
    public float ambientVolumeMod = 0.7f;
    public float musicVolumeMod = 0.15f;
    float musicVolume = 1;
    int theme = 0;
    int taintLevel = 0;
    
    void Awake() {
        DontDestroyOnLoad(transform.gameObject);
        SceneManager.LoadSceneAsync("MainMenu");
        SceneManager.activeSceneChanged += ChangedActiveScene;
    }

    void ChangedActiveScene(Scene current, Scene next) {
        Debug.Log(current);
        if (GameObject.Find("LevelManager") != null) {
            GameObject.Find("LevelManager").GetComponent<LevelManager>().ApplyBackground();
            if (theme != GameObject.Find("LevelManager").GetComponent<LevelManager>().theme || taintLevel != GameObject.Find("LevelManager").GetComponent<LevelManager>().taintLevel) {
                Debug.Log("switching music");
                StartCoroutine(SwitchMusic());
            }
            
        } else if (next.name == "MainMenu") {
            transform.Find("Music").GetComponent<AudioSource>().clip = mainTheme;
            transform.Find("Music").GetComponent<AudioSource>().volume = ApplicationSettings.MusicVolume() * 0.1f;
            transform.Find("Music").GetComponent<AudioSource>().Play();
        } else if (next.name == "Cutscene") {
            transform.Find("Music").GetComponent<AudioSource>().clip = cutsceneTheme;
            transform.Find("Music").GetComponent<AudioSource>().volume = ApplicationSettings.MusicVolume() * 0.1f;
            transform.Find("Music").GetComponent<AudioSource>().Play();
        }
    }

    IEnumerator SwitchMusic() {
        theme = GameObject.Find("LevelManager").GetComponent<LevelManager>().theme;
        taintLevel = GameObject.Find("LevelManager").GetComponent<LevelManager>().taintLevel;

        StartCoroutine(FadeMusicVolume(0, 0.3f));
        while(musicVolume != 0) {
            yield return null;
        }

        transform.Find("Music").GetComponent<AudioSource>().clip = GameObject.Find("Backgrounds").transform.GetChild(theme-1).GetChild(taintLevel-1).Find("MusicContainer").GetComponent<AudioSource>().clip;
        transform.Find("Music").GetComponent<AudioSource>().volume = ApplicationSettings.MusicVolume() * musicVolumeMod;
        transform.Find("Music").GetComponent<AudioSource>().Play();

        transform.Find("AmbientSound").GetComponent<AudioSource>().clip = GameObject.Find("Backgrounds").transform.GetChild(theme-1).GetChild(taintLevel-1).GetComponent<AudioSource>().clip;
        transform.Find("AmbientSound").GetComponent<AudioSource>().volume = ApplicationSettings.MusicVolume() * ambientVolumeMod;
        transform.Find("AmbientSound").GetComponent<AudioSource>().Play();

        StartCoroutine(FadeMusicVolume(1, 0.3f));
        while(musicVolume != 1) {
            yield return null;
        }
    }

    public void ToggleMusic(bool musicOn) {
        if (musicOn) {
            SetMusicVolume(1);
        } else {
            SetMusicVolume(0.35f);
        }
    }

    public void SetMusicVolume(float volume) {
        StopCoroutine(FadeMusicVolume(volume));
        StartCoroutine(FadeMusicVolume(volume));
    }

    IEnumerator FadeMusicVolume(float newVolume, float speed = 1f) {
        float originalAmbientVol = GetMusicVolume();
        float originalMusicVol = GetMusicVolume();
        float modifier = 7.5f * speed;
        if (originalMusicVol > newVolume) {
            while (musicVolume - newVolume > 0.1f) {
                musicVolume -= modifier * Time.unscaledDeltaTime;
                transform.Find("AmbientSound").GetComponent<AudioSource>().volume = musicVolume * ambientVolumeMod;
                transform.Find("Music").GetComponent<AudioSource>().volume = musicVolume * musicVolumeMod;
                yield return null;
            }

        } else {
            while (newVolume - musicVolume > 0.1f) {
                musicVolume += modifier * Time.unscaledDeltaTime;
                transform.Find("AmbientSound").GetComponent<AudioSource>().volume = musicVolume * ambientVolumeMod;
                transform.Find("Music").GetComponent<AudioSource>().volume = musicVolume * musicVolumeMod;
                yield return null;
            }
        }
        musicVolume = newVolume;

    }

    public float GetMusicVolume() {
        return musicVolume;
    }
}
