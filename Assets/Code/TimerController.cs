using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TimerController : MonoBehaviour {
    
    int seconds = 60;
    [SerializeField] public bool formatTime = true;
    private TextMeshProUGUI m_TextComponent;
    private System.DateTime dateTime;

    void Awake() {
        seconds = transform.root.GetComponent<LevelManager>().time;
        m_TextComponent = GetComponent<TextMeshProUGUI>();
        SetTime();
        StartCoroutine(RunTimer());
    }

    IEnumerator RunTimer() {
        while (seconds > 0) {
            yield return new WaitForSeconds(1);
            seconds--;
            SetTime();
        }
        OutOfTime();
    }

    void SetTime() {
        dateTime = System.DateTime.Today.AddSeconds(seconds);
        
        if (formatTime) {
            m_TextComponent.text = string.Format($"{dateTime.Minute:D2}:{dateTime.Second:D2}");
        } else {
            m_TextComponent.text = seconds.ToString();
        }
    }

    void OutOfTime() {
        GameObject.Find("LevelManager").GetComponent<LevelManager>().LevelLose();
    }
}
