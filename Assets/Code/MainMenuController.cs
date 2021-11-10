using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

    public GameObject[] mainMenuStates;
    public void NewGame() {
        SceneManager.LoadScene("Cutscene");
    }

    public void OpenSettings() {
        Debug.Log("Add settings");
    }

    public void OpenLevelSelect() {

    }

    public void BackToMain() {

    }

    public void SaveAndExit() {

    }

    public void ExitWithoutSaving() {
        
    }

    public void QuitGame() {
        Debug.Log("Quit game");
        Application.Quit();
    }
}
