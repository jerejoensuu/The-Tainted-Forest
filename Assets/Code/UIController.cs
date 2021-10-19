using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIController : MonoBehaviour {

    public static bool paused = false;
    public GameObject pauseMenu;
    [Tooltip("Set selection to this button when game is paused")] public GameObject activeButton;

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
        GetComponent<EventSystem>().SetSelectedGameObject(null);
        GetComponent<EventSystem>().SetSelectedGameObject(activeButton);
        Time.timeScale = 0;
    }

    void UnpauseGame() {
        Debug.Log("Game unpaused");
        paused = false;
        pauseMenu.SetActive(false);
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
}
