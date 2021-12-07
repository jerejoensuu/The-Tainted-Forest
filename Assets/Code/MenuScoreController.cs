using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuScoreController : MonoBehaviour {

    private TextMeshProUGUI m_TextComponent;
    int score;
    int timeLeft;
    int pointsPerSecond = 1000;
    int totalScore;
    
    void Start() {
        m_TextComponent = GetComponent<TextMeshProUGUI>();
        score = GameObject.Find("PlayerUI").GetComponent<PlayerUI>().GetScore();
        timeLeft = GameObject.Find("Timertext").GetComponent<TimerController>().GetTime();
        totalScore = score;
        m_TextComponent.text = totalScore.ToString();

        StartCoroutine(AnimateScore());
    }

    IEnumerator AnimateScore() {
        float toWait = 3f / timeLeft;
        while (GameObject.Find("Timertext").GetComponent<TimerController>().GetTime() > 0) {
            totalScore += pointsPerSecond;
            m_TextComponent.text = totalScore.ToString();
            GameObject.Find("Timertext").GetComponent<TimerController>().RemoveFromTimer(1);

            yield return new WaitForSecondsRealtime(toWait);
            if (GameObject.Find("Timertext").GetComponent<TimerController>().GetTime() < 7) {
                toWait += 0.05f;
            }
        }
    }
}
