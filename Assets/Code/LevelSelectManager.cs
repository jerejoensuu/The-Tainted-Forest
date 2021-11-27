using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using TaintedForest;

public class LevelSelectManager : MonoBehaviour {

    List<string> levels = new List<string>();
    int rows = 3;
    int offset = 150;

    public TextMeshProUGUI levelNumberTextField;
    public TextMeshProUGUI levelScoreTextField;
    public Button levelStartButton;
    
    void Awake() {
        levels.AddRange(System.IO.Directory.GetFiles(System.IO.Directory.GetCurrentDirectory() + "\\Assets\\Scenes\\Levels", "*.unity"));

        BuildButtons();
        SetContainerWidth();
        DisplayLevelInfo(1);
    }

    void BuildButtons() {
        bool firstButtonSet = false;
        foreach (string file in levels) {
            
            int index  = levels.IndexOf(file);
            GameObject button = Instantiate(transform.GetChild(0).gameObject, GetButtonPosition(index), Quaternion.identity) as GameObject;
            if (!firstButtonSet) {
                GameObject.Find("EventSystem").GetComponent<MainMenuController>().panelActiveButtons[1] = button;
                firstButtonSet = true;
            }
            button.transform.SetParent(transform, false);
            button.GetComponentInChildren<TextMeshProUGUI>().SetText((index+1).ToString());
            button.GetComponent<Button>().onClick.AddListener(() => DisplayLevelInfo(index + 1));

        }
        transform.GetChild(0).gameObject.SetActive(false);
    }

    void OpenLevel(int levelNumber) {
        GameObject.Find("EventSystem").GetComponent<MainMenuController>().StartLevel(levelNumber);
    }

    Vector2 GetButtonPosition(int index) {
        Vector2 position;

        int x = offset * (int)(index/rows);
        int y = -offset * (index % rows);
        position = new Vector2(x, y);

        return position;
    }

    void SetContainerWidth() {
        int buttonWidth = (int)transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta.x;
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2 (
            (buttonWidth * ((int)((levels.Count)/rows)+1)) + ( (offset - buttonWidth) * ((int)((levels.Count)/rows))),
            gameObject.GetComponent<RectTransform>().sizeDelta.y
        );
    }

    public void DisplayLevelInfo(int levelNumber) {
        levelNumberTextField.SetText("Level " + levelNumber);
        levelScoreTextField.SetText("Highscore: " + GetLevelScore(levelNumber));
        levelStartButton.onClick.AddListener(() => OpenLevel(levelNumber));
    }

    public int GetLevelScore(int levelNumber) {
        int levelIndex = levelNumber - 1;
        Score score = new Score(GameData.GetFilePath());
        var scoreEntry = score.GetEntry(levelIndex);
        return scoreEntry.Score;
    }
}
