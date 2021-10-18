using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {
    // Start is called before the first frame update
    public void NewGame() {
        SceneManager.LoadScene("Cutscene");
    }

    public void Settings() {

    }

    public void QuitGame() {
        Debug.Log("Quit game");
        Application.Quit();
    }
}
