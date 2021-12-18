using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class CutsceneController : MonoBehaviour {

    public InputActions inputActions;
    public GameObject blackScreen;
    bool allowSkip = false;
    bool skipped = false;

    void Awake() {
        inputActions = new InputActions();
        inputActions.Disable();
        inputActions.UI.Submit.performed += SkipCutscene;
        inputActions.UI.Navigate.performed += SkipCutscene;
        inputActions.UI.Cancel.performed += SkipCutscene;
        inputActions.UI.Click.performed += SkipCutscene;
        inputActions.UI.RightClick.performed += SkipCutscene;
        inputActions.UI.Pause.performed += SkipCutscene;
        inputActions.Player.Move.performed += SkipCutscene;
        inputActions.Player.Fire.performed += SkipCutscene;
    }

    void Update() {
        if (allowSkip) {
            inputActions.Enable();
        }
        if ((Keyboard.current.anyKey.isPressed || Mouse.current.leftButton.isPressed || Mouse.current.rightButton.isPressed || Mouse.current.middleButton.isPressed) && !skipped && allowSkip) {
            skipped = true;
            StartCoroutine(LoadLevelOne());
        }
    }

    void SkipCutscene(InputAction.CallbackContext context) {
        StartCoroutine(LoadLevelOne());
    }

    public void AllowCutsceneSkip() {
        allowSkip = true;
    }

    IEnumerator LoadLevelOne() {
        GameObject transitionScreen = Instantiate(blackScreen, Vector3.zero, Quaternion.identity) as GameObject;

        float maskSize = 1f;
        while (true) {
            maskSize -= Time.unscaledDeltaTime;
            if (maskSize <= 0) {
                Destroy(transform.transform.gameObject);
                break;
            } else {
                transitionScreen.GetComponentInChildren<SpriteMask>().transform.localScale = new Vector2(maskSize,maskSize);
            }
            yield return null;
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("1");
        asyncLoad.allowSceneActivation = true;
        while(false) {
            // do something
            //asyncLoad.allowSceneActivation = true;
        }
    }

    void OnDisable() {
        inputActions.UI.Submit.performed -= SkipCutscene;
        inputActions.UI.Navigate.performed -= SkipCutscene;
        inputActions.UI.Cancel.performed -= SkipCutscene;
        inputActions.UI.Click.performed -= SkipCutscene;
        inputActions.UI.RightClick.performed -= SkipCutscene;
        inputActions.UI.Pause.performed -= SkipCutscene;
        inputActions.Player.Move.performed -= SkipCutscene;
        inputActions.Player.Fire.performed -= SkipCutscene;
        inputActions.Disable();
    }
}
