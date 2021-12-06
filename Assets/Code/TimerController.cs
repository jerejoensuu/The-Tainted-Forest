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

    void Start() {
        seconds = transform.root.GetComponent<LevelManager>().time;
        m_TextComponent = GetComponent<TextMeshProUGUI>();
        SetTime();
        StartCoroutine(RunTimer());
    }

    IEnumerator RunTimer() {
        while (seconds > 0) {
            yield return new WaitForSeconds(1);
            seconds--;
            if (seconds <= 10) {
                GetComponent<Animator>().SetTrigger("Flash");
            }
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

    public int GetTime() {
        return seconds;
    }

    public void AddToTimer(int secondsAdded) {
        StartCoroutine(AnimateTimerBoost(secondsAdded));
    }

    public void RemoveFromTimer(int secondsRemoved) {
        seconds -= secondsRemoved;
        SetTime();
    }

    IEnumerator AnimateTimerBoost(int secondsAdded) {
        GetComponent<Animator>().SetBool("TimerBoost", true);
        float interval = 0.05f;
        while(secondsAdded > 0) {
            seconds++;
            secondsAdded--;
            SetTime();
            yield return new WaitForSeconds(interval);
            interval += 0.005f;
            if (secondsAdded <= 5) {
                GetComponent<Animator>().SetBool("TimerBoost", false);
            }
        }
    }

    void OutOfTime() {
        GameObject.Find("LevelManager").GetComponent<LevelManager>().LevelLose();
    }
}
