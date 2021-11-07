using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public static bool paused = false;
    public GameObject pauseMenu;
    [Tooltip("Set selection to this button when game is paused")] public GameObject pauseMenuActiveButton;
    [Tooltip("Set selection to this button when level is won")] public GameObject winScreenActiveButton;
    [Tooltip("Set selection to this button when level is lost")] public GameObject loseScreenActiveButton;
    

    public GameObject winScreen;
    public GameObject loseScreen;
    public GameObject endOverlay;

    void Start() {
        UnpauseGame();
    }

    void Update() {
        if (Input.GetButtonDown("Pause")) { // Esc or start button
            TogglePause();
        }
    }

    void TogglePause() {
        if (!paused) {
            PauseGame();
        }
        else {
            UnpauseGame();
        }
    }

    void PauseGame() {
        Debug.Log("Game paused");
        paused = true;
        pauseMenu.SetActive(true);
        GetComponent<AudioSource>().Pause();
        GetComponent<EventSystem>().SetSelectedGameObject(null);
        GetComponent<EventSystem>().SetSelectedGameObject(pauseMenuActiveButton);
        Time.timeScale = 0;
    }

    void UnpauseGame() {
        Debug.Log("Game unpaused");
        paused = false;
        pauseMenu.SetActive(false);
        GetComponent<AudioSource>().UnPause();
        Time.timeScale = 1;
    }

    public void ResumeGame() {
        TogglePause();
    }

    public void OpenSettings() {
        Debug.Log("Add settings");
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
        paused = true;
        StartCoroutine(ShowEndScreen(true));
    }

    public void LevelLose() {
        paused = true;
        StartCoroutine(ShowEndScreen(false));
    }

    IEnumerator ShowEndScreen(bool hasWon) {
        float change = 0.02f;
        for (float alpha = 0f; alpha < 1; alpha += change) 
        {
            GetComponent<AudioSource>().volume -= change;
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
        }
        yield return null;
    }
}
