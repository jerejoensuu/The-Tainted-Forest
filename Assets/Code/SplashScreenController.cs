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
        GameObject.Find("EventSystem").GetComponent<MainMenuController>().inputActions.Enable();
        Destroy(gameObject);
        GameObject.Find("EventSystem").GetComponent<MainMenuController>().SetButtonSelection(GameObject.Find("New Game"));
    }

}
