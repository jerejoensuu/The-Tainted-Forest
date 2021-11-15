using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

    public GameObject[] mainMenuPanels;

    void ChangePanel(int index) {
        for (int i = 0; i < mainMenuPanels.Length; i++) {
            if (index == i) {
                mainMenuPanels[i].SetActive(true);
            }
            else {
                mainMenuPanels[i].SetActive(false);
            }
        }
    }
    public void NewGame() {
        SceneManager.LoadScene("Cutscene");
    }

    public void OpenSettings() {
        Debug.Log("Add settings");
    }

    public void OpenLevelSelect() {
        ChangePanel(1);
    }

    public void BackToMain() {
        ChangePanel(0);
    }

    public void SaveAndExit() {

    }

    public void ExitWithoutSaving() {
        
    }

    public void QuitGame() {
        Debug.Log("Quit game");
        Application.Quit();
    }

    public void StartLevel(int levelNumber) {
        string levelName = "Level " + levelNumber.ToString();
        SceneManager.LoadScene(levelName);
    }
}
