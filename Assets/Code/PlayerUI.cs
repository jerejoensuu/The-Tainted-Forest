using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour {

    public GameObject scoreCounter;
    public GameObject ammoCounter;

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
}
