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
    
    void Start() {
        m_TextComponent = GetComponent<TextMeshProUGUI>();
        animator = transform.GetComponent<Animator>();
        Time.timeScale = 0;
        m_TextComponent.text = SceneManager.GetActiveScene().name;
    }

    
    void Update() {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) {
            Time.timeScale = 1;
        }
    }

    public void StartLevel() {
        m_TextComponent.text = "Start!";
    }
}
