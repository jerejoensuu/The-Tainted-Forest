using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public bool splashScreenPlayed = false;
    
    void Awake() {
        DontDestroyOnLoad(transform.gameObject);
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
