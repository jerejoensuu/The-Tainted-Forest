using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreenController : MonoBehaviour {
    
    void Start() {
        if (GameObject.Find("GameController").GetComponent<GameController>().splashScreenPlayed == false) {
            Cursor.visible = false;
            GameObject.Find("GameController").GetComponent<GameController>().splashScreenPlayed = true;
        } else {
            ActivateCursor();
        }
        
    }

    public void ActivateCursor() {
        Cursor.visible = true;
        MainMenuController mmc = GameObject.Find("EventSystem").GetComponent<MainMenuController>();
        mmc.inputActions.Enable();
        Destroy(gameObject);
        mmc.ChangePanel(0);
    }

}
