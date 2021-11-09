using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerController : MonoBehaviour {
    
    [SerializeField] public int seconds;
    [SerializeField] public bool formatTime = true;
    private TextMeshProUGUI m_TextComponent;
    private System.DateTime dateTime;

    void Awake() {
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
