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

    public GameObject weaponImage;
    public Sprite[] weaponIcons;

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

    void Start () {
        scoreText = scoreCounter.GetComponent<TextMeshProUGUI>();
        ammoText = ammoCounter.GetComponent<TextMeshProUGUI>();
    }

    public void SetScore(int amount) {
        scoreText.SetText(amount.ToString("#,000000"));
    }

    public void SetAmmo(int amount) {
        ammoText.SetText(amount.ToString());
    }

    public void SetWeapon(string type) {
        switch (type) {
            case "Vine":
                            weaponImage.GetComponent<Image>().sprite = weaponIcons[0];
                            break;
            case "RapidFire":
                            weaponImage.GetComponent<Image>().sprite = weaponIcons[1];
                            break;
            case "DoubleVines":
                            weaponImage.GetComponent<Image>().sprite = weaponIcons[2];
                            break;
            case "StickyVines":
                            weaponImage.GetComponent<Image>().sprite = weaponIcons[3];
                            break;
            default:
                            Debug.Log("Invalid weapon type in UI");
                            break;
        }
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
