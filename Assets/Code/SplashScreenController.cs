using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreenController : MonoBehaviour {
    
    void Awake() {
        Cursor.visible = false;
    }

    public void ActivateCursor() {
        Cursor.visible = true;
        GameObject.Find("EventSystem").GetComponent<MainMenuController>().inputActions.Enable();
        Destroy(GameObject.Find("Skip splash screen"));
        Destroy(GameObject.Find("LOGO"));
        Destroy(GameObject.Find("blackScreen"));
        GameObject.Find("EventSystem").GetComponent<MainMenuController>().SetButtonSelection(GameObject.Find("New Game"));
    }

}
