using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour {

    public GameObject scoreCounter;
    public GameObject ammoCounter;

    public GameObject[] healthContainers;
    public Sprite[] healthIcons;

    TextMeshProUGUI scoreText;
    TextMeshProUGUI ammoText;

    private int levelScore;

    public void ChangeScore(int amount) {
        levelScore += amount;
        SetScore(levelScore);
    }

    public int GetScore() {
        return levelScore;
    }

    public void EndScore() {
        // Points for time remaining + ammo remaining + full health bonus?
    }

    void Awake () {
        scoreText = scoreCounter.GetComponent<TextMeshProUGUI>();
        ammoText = ammoCounter.GetComponent<TextMeshProUGUI>();
    }

    public void SetScore(int amount) {
        scoreText.SetText(amount.ToString("#,000000"));
    }

    public void SetAmmo(int amount) {
        ammoText.SetText(amount.ToString());
    }

    public void SetHealth(int amount) {
        for (int i = 0; i < healthContainers.Length; i++) {
            if (amount - 1 < i) {
                healthContainers[i].GetComponent<Image>().sprite = healthIcons[0];
            }
            else {
                healthContainers[i].GetComponent<Image>().sprite = healthIcons[1];
            }
        }
    }
}
