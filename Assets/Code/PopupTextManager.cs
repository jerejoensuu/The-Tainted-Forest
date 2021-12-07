using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopupTextManager : MonoBehaviour {

    void Awake() {
    }

    void Update() {
        GameObject toDestroy = null;
        if (Time.timeScale != 0) {
            for (int i = 1; i <= transform.childCount-1; i++) {
                transform.GetChild(i).transform.localPosition = new Vector3(transform.GetChild(i).transform.localPosition.x,
                                                                            transform.GetChild(i).transform.localPosition.y + (13f * Time.deltaTime));

                transform.GetChild(i).GetComponent<TextMeshProUGUI>().color = new Color(255, 255, 255, transform.GetChild(i).GetComponent<TextMeshProUGUI>().color.a - 1.25f * Time.deltaTime);

                if (transform.GetChild(i).GetComponent<TextMeshProUGUI>().color.a <= 0) {
                    toDestroy = transform.GetChild(i).gameObject;
                }
            }
        }

        if (toDestroy != null) {
            Destroy(toDestroy);
        }
        
    }

    public void NewPopupText(string text, Vector2 position) {
        GameObject popup = Instantiate(transform.GetChild(0).gameObject, position, Quaternion.identity) as GameObject;
        popup.GetComponent<TextMeshProUGUI>().SetText(text);
        popup.transform.SetParent(transform);
        popup.transform.localScale = new Vector3(1, 1);
    }

}
