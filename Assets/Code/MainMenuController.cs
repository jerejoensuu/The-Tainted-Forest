using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuController : MonoBehaviour {

    public GameObject[] mainMenuPanels;
    public GameObject[] panelActiveButtons; // The button that should be selected/active when a menu panel is opened
    public Slider[] volumeSliders;

    void Awake() {
        ChangePanel(0);
    }

    void ChangePanel(int index) {
        for (int i = 0; i < mainMenuPanels.Length; i++) {
            if (index == i) {
                mainMenuPanels[i].SetActive(true);
                GetComponent<EventSystem>().SetSelectedGameObject(null);
                GetComponent<EventSystem>().SetSelectedGameObject(panelActiveButtons[i]);
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
        ChangePanel(2);
        volumeSliders[0].value = ApplicationSettings.GetMasterVolume();
        volumeSliders[1].value = ApplicationSettings.GetSoundVolume();
        volumeSliders[2].value = ApplicationSettings.GetMusicVolume();
    }

    public void OpenLevelSelect() {
        ChangePanel(1);
    }

    public void BackToMain() {
        ChangePanel(0);
    }

    public void SaveAndExit() {
        ApplicationSettings.ChangeVolumeSettings(volumeSliders[0].value, volumeSliders[1].value, volumeSliders[2].value);
        BackToMain();
    }

    public void ExitWithoutSaving() {
        BackToMain();
    }

    public void QuitGame() {
        Debug.Log("Quit game");
        Application.Quit();
    }

    public void StartLevel(int levelNumber) {
        string levelName = levelNumber.ToString();
        SceneManager.LoadScene(levelName);
    }
}
