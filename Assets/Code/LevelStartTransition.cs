using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelStartTransition : MonoBehaviour {

    TextMeshProUGUI txt;
    public GameObject textObject;
    Animator animator;
    private TextMeshProUGUI m_TextComponent;
    public bool levelStarted = false;
    
    void Start() {
        transform.parent.parent.Find("UIController").GetComponent<UIController>().paused = true;
        Time.timeScale = 0;

        m_TextComponent = GetComponent<TextMeshProUGUI>();
        animator = transform.GetComponent<Animator>();
        m_TextComponent.text = "Level " + SceneManager.GetActiveScene().name;
    }

    
    void Update() {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !levelStarted) {
            levelStarted = true;
            Time.timeScale = 1;
            transform.parent.parent.Find("UIController").GetComponent<UIController>().UnpauseGame();
        }
    }

    public void StartLevel() {
        m_TextComponent.text = "Start!";
    }
}
