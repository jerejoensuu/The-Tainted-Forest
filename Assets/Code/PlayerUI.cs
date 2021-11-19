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

    TextMeshProUGUI score;
    TextMeshProUGUI ammo;

    void Awake () {
        score = scoreCounter.GetComponent<TextMeshProUGUI>();
        ammo = ammoCounter.GetComponent<TextMeshProUGUI>();
    }

    public void SetScore(int amount) {
        score.SetText(amount.ToString("#,000000"));
    }

    public void SetAmmo(int amount) {
        ammo.SetText(amount.ToString());
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
