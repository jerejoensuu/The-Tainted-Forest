using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UIController : MonoBehaviour {

    public bool paused = false;
    public GameObject pauseMenu;
    public GameObject[] pauseMenuPanels;
    public GameObject[] panelActiveButtons;
    public Slider[] volumeSliders;
    [Tooltip("Set selection to this button when level is won")] public GameObject winScreenActiveButton;
    [Tooltip("Set selection to this button when level is lost")] public GameObject loseScreenActiveButton;
    public Texture2D cursorTexture;
    public AudioSource audioSrc;
    public AudioClip failSound;


    public GameObject winScreen;
    public GameObject loseScreen;
    public GameObject endOverlay;
    InputActions inputActions;

    void Awake() {
        inputActions = new InputActions();
        inputActions.Enable();

        inputActions.UI.Pause.performed += TogglePause;
        inputActions.UI.Cancel.performed += CancelSettings;
    }

    void Start() {
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        Cursor.visible = false;
    }

    void TogglePause(InputAction.CallbackContext context) {
        if (transform.parent.Find("Canvas").Find("LevelText").GetComponent<LevelStartTransition>().levelStarted) {
            if (!paused) {
                PauseGame();
            }
            else {
                UnpauseGame();
            }
        }
    }

    void PauseGame() {
        paused = true;
        pauseMenu.SetActive(true);
        ChangePanel(0);
        Time.timeScale = 0;
        Cursor.visible = true;

        if (GameObject.Find("GameController") != null) {
            GameObject.Find("GameController").GetComponent<GameController>().ToggleMusic(false);
        }
    }

    public void UnpauseGame() {
        paused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        Cursor.visible = false;

        if (GameObject.Find("GameController") != null) {
            GameObject.Find("GameController").GetComponent<GameController>().ToggleMusic(true);
        }
    }

    void ChangePanel(int index) {
        for (int i = 0; i < pauseMenuPanels.Length; i++) {
            if (index == i) {
                pauseMenuPanels[i].SetActive(true);
                GetComponent<EventSystem>().SetSelectedGameObject(null);
                GetComponent<EventSystem>().SetSelectedGameObject(panelActiveButtons[i]);
            }
            else {
                pauseMenuPanels[i].SetActive(false);
            }
        }
    }

    public void OpenSettings() {
        ChangePanel(1);
        volumeSliders[0].value = ApplicationSettings.GetMasterVolume();
        volumeSliders[1].value = ApplicationSettings.GetSoundVolume();
        volumeSliders[2].value = ApplicationSettings.GetMusicVolume();
    }

    public void ApplySettings() {
        ApplicationSettings.ChangeVolumeSettings(volumeSliders[0].value, volumeSliders[1].value, volumeSliders[2].value);
        GameObject.Find("GameController").GetComponent<GameController>().SetMusicVolume(ApplicationSettings.MusicVolume());
    }

    public void ExitSettings() {
        ChangePanel(0);
    }

    void CancelSettings(InputAction.CallbackContext context) {
        if (pauseMenuPanels[1].activeSelf) {
            ExitSettings();
        }
    }

    public void ReturnToMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LevelWin() {
        Cursor.visible = true;
        paused = true;
        StartCoroutine(ShowEndScreen(true));
    }

    public void LevelLose() {
        Cursor.visible = true;
        paused = true;
        StartCoroutine(ShowEndScreen(false));
    }

    IEnumerator ShowEndScreen(bool hasWon) {
        float change = 0.02f;
        for (float alpha = 0f; alpha < 1; alpha += change) 
        {
            //GameObject.Find("GameController").GetComponent<GameController>().SetMusicVolume(GameObject.Find("GameController").GetComponent<GameController>().GetMusicVolume() - change);
            Color overlayColor = endOverlay.GetComponent<Image>().color;
            overlayColor.a = alpha;
            endOverlay.GetComponent<Image>().color = overlayColor;
            yield return new WaitForSeconds(0.02f);
        }

        yield return new WaitForSeconds(0.25f);

        if (hasWon) {
            winScreen.SetActive(true);
            Time.timeScale = 0;
            GetComponent<EventSystem>().SetSelectedGameObject(null);
            GetComponent<EventSystem>().SetSelectedGameObject(winScreenActiveButton);
        }
        else {
            loseScreen.SetActive(true);
            Time.timeScale = 0;
            GetComponent<EventSystem>().SetSelectedGameObject(null);
            GetComponent<EventSystem>().SetSelectedGameObject(loseScreenActiveButton);
            
            audioSrc.clip = failSound;
            audioSrc.volume = ApplicationSettings.SoundVolume() * 0.3f;
            audioSrc.Play();
        }
        yield return null;
    }

    void OnDisable() {
        inputActions.UI.Pause.performed -= TogglePause;
        inputActions.UI.Cancel.performed -= CancelSettings;
        inputActions.Disable();
    }
}
