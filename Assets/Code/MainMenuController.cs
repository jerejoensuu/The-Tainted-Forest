using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TaintedForest;


public static class Levels {

    public static int numberOfLevels;
    public static bool isChecked = false;
    public static void GetLevelNumber(int number) {
        numberOfLevels = number;
    }
}
public class MainMenuController : MonoBehaviour {

    public GameObject[] mainMenuPanels;
    public GameObject[] panelActiveButtons; // The button that should be selected/active when a menu panel is opened
    public Slider[] volumeSliders;

    public Texture2D cursorTexture;
    public GameObject blackScreen;

    void Awake() {
        ChangePanel(0);
    }

    void Start() {
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        if (!Levels.isChecked) {
            Levels.GetLevelNumber(mainMenuPanels[1].GetComponent<LevelSelectManager>().levels.Count);
            Score score = new Score(GameData.GetFilePath());
            score.FillScoreArray();
            Levels.isChecked = true;
        }
    }

    void ChangePanel(int index) {
        for (int i = 0; i < mainMenuPanels.Length; i++) {
            if (index == i) {
                mainMenuPanels[i].SetActive(true);
                SetButtonSelection(panelActiveButtons[i]);
            }
            else {
                mainMenuPanels[i].SetActive(false);
            }
        }
    }

    public void SetButtonSelection(GameObject button) {
        GetComponent<EventSystem>().SetSelectedGameObject(null);
        GetComponent<EventSystem>().SetSelectedGameObject(button);
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
        StartCoroutine(LevelLoader(levelName));
    }

    IEnumerator LevelLoader(string levelName, bool transition = true) {
        
        GameObject transitionScreen = Instantiate(blackScreen, Vector3.zero, Quaternion.identity) as GameObject;

        float maskSize = 1f;
        while (true) {
            maskSize -= Time.unscaledDeltaTime;
            if (maskSize <= 0) {
                Destroy(transform.transform.gameObject);
                break;
            } else {
                transitionScreen.GetComponentInChildren<SpriteMask>().transform.localScale = new Vector2(maskSize,maskSize);
            }
            yield return null;
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelName);
        asyncLoad.allowSceneActivation = true;
        while(false) {
            // do something
            //asyncLoad.allowSceneActivation = true;
        }
    }
}
