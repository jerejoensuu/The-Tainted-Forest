using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using TaintedForest;
using UnityEngine.InputSystem;


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
    public GameObject continueButton;
    public Slider[] volumeSliders;
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    public Texture2D cursorTexture;
    public GameObject blackScreen;
    public InputActions inputActions;

    void Awake() {
        Time.timeScale = 1;
        ChangePanel(0);
        FillResolutionDropdown();
        SetResolution();

        inputActions = new InputActions();
        inputActions.Disable();
        inputActions.UI.Cancel.performed += CancelSettings;
    }

    void Start() {
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);

        Levels.GetLevelNumber(mainMenuPanels[1].GetComponent<LevelSelectManager>().levels.Count);
        Score score = new Score(GameData.GetFilePath());

        if (!Levels.isChecked) {
            score.FillScoreArray();
            Levels.isChecked = true;
        }

        CheckLevelProgress();
    }

    int CheckLevelProgress() {
        Score score = new Score(GameData.GetFilePath());
        bool uncleared = false;
        int firstUncleared = 0;
        for (int i = 0; i < Levels.numberOfLevels; i++) {
            if (score.GetEntry(i).Score == 0 && !uncleared) {
                uncleared = true;
                firstUncleared = i;
            }
        }

        SetContinueButton(firstUncleared);

        return firstUncleared;
    }

    void SetContinueButton (int levelIndex) {
        if (levelIndex > 0) {
            continueButton.SetActive(true);
            continueButton.GetComponent<Button>().onClick.RemoveAllListeners();
            continueButton.GetComponent<Button>().onClick.AddListener(() => mainMenuPanels[1].GetComponent<LevelSelectManager>().OpenLevel(levelIndex + 1));
        }
        else {
            continueButton.SetActive(false);
        }
    }

    public void ChangePanel(int index) {
        for (int i = 0; i < mainMenuPanels.Length; i++) {
            if (index == i) {
                mainMenuPanels[i].SetActive(true);
                if (i == 0 && CheckLevelProgress() > 0) {
                    SetButtonSelection(continueButton);
                }
                else {
                    SetButtonSelection(panelActiveButtons[i]);
                }
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
        resolutionDropdown.value = ApplicationSettings.GetResolutionIndex();
        fullscreenToggle.isOn = ApplicationSettings.GetFullscreen();
    }

    public void OpenLevelSelect() {
        ChangePanel(1);
    }

    public void OpenCredits() {
        ChangePanel(3);
    }

    public void BackToMain() {
        ChangePanel(0);
    }

    void CancelSettings(InputAction.CallbackContext context) {
        ChangePanel(0);
    }

    public void SettingsOk() {
        SettingsApply();
        BackToMain();
    }

    public void SettingsCancel() {
        BackToMain();
    }

    public void SettingsApply() {
        ApplicationSettings.ChangeVolumeSettings(volumeSliders[0].value, volumeSliders[1].value, volumeSliders[2].value);
        ApplicationSettings.ChangeResolutionSettings(resolutionIndex, BoolToInt(fullscreen));
        GameObject.Find("GameController").GetComponent<GameController>().SetMusicVolume(ApplicationSettings.MusicVolume());
        SetResolution();
    }

    public void SettingsClearScores() {
        Score score = new Score(GameData.GetFilePath());
        score.Clear();
        CheckLevelProgress();
    }

    public void QuitGame() {
        Debug.Log("Quit game");
        Application.Quit();
    }

    public void StartLevel(int levelNumber) {
        string levelName = levelNumber.ToString();
        StartCoroutine(LevelLoader(levelName));
    }

    void FillResolutionDropdown() {
        ScreenResolutions sr = new ScreenResolutions();
        List<string> list = sr.GetResolutionString();
        resolutionDropdown.AddOptions(list);
    }

    public int resolutionIndex = 0;
    public void OnResolutionChanged() {
        resolutionIndex = resolutionDropdown.value;
    }

    public bool fullscreen = true;
    public void OnFullScreenToggled() {
        fullscreen = fullscreenToggle.isOn;
    }

    private int BoolToInt(bool value) {
        if (value) {
            return 1;
        }
        else {
            return 0;
        }
    }

    public void SetResolution() {
        ScreenResolutions sr = new ScreenResolutions();
        Screen.SetResolution(sr.GetResolution(ApplicationSettings.GetResolutionIndex())[0], sr.GetResolution(ApplicationSettings.GetResolutionIndex())[1], ApplicationSettings.GetFullscreen());
        Debug.Log("Resolution set to " + sr.GetResolution(ApplicationSettings.GetResolutionIndex())[0] + " x " + sr.GetResolution(ApplicationSettings.GetResolutionIndex())[1]);
    }

    IEnumerator LevelLoader(string levelName, bool transition = true) {
        inputActions.Disable();
        GameObject transitionScreen = Instantiate(blackScreen, Vector3.zero, Quaternion.identity) as GameObject;

        float maskSize = 1f;
        while (true) {
            maskSize -= Time.unscaledDeltaTime;
            if (maskSize <= 0) {
                Destroy(transform.transform.gameObject);
                break;
            } else {
                transitionScreen.GetComponentInChildren<SpriteMask>().transform.localScale = new Vector2(maskSize,maskSize);
                GameObject.Find("GameController").transform.Find("Music").GetComponent<AudioSource>().volume = ApplicationSettings.MusicVolume() * 0.1f * maskSize;
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

    void OnDisable() {
        inputActions.UI.Cancel.performed -= CancelSettings;
        inputActions.Disable();
    }
}
