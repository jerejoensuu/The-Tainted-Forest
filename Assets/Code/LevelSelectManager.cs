using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using TaintedForest;

public class LevelSelectManager : MonoBehaviour {

    [Tooltip("File names of levels in desired order")] public List<string> levels = new List<string>();
    public List<string> trueLevels = new List<string>();
    int rows = 3;
    int offset = 150;

    public TextMeshProUGUI levelNumberTextField;
    public TextMeshProUGUI levelScoreTextField;
    public Button levelStartButton;
    
    void Awake() {
        BuildButtons();
        SetContainerWidth();
        DisplayLevelInfo(1);

        trueLevels.AddRange(System.IO.Directory.GetFiles(System.IO.Directory.GetCurrentDirectory() + "\\Assets\\Scenes\\Levels", "*.unity"));
        if (trueLevels.Count > levels.Count) {
            Debug.Log("All levels not set to the list in Mainmenu: Canvas\\LevelSelectContainer -> LevelSelectManager!");
        }
    }

    public void DebugUnlockAllLevels() {
        Button[] buttons = GetComponentsInChildren<Button>();
        foreach (Button button in buttons) {
            button.interactable = true;
        }
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

            if (!LevelIsUnlocked(index + 1)) {
                button.GetComponent<Button>().interactable = false;
                //button.GetComponent<Image>().color = Color.gray;
            }

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
        levelStartButton.onClick.RemoveAllListeners();
        levelStartButton.onClick.AddListener(() => OpenLevel(levelNumber));
        
        /*if (LevelIsUnlocked(levelNumber)) {
            levelStartButton.interactable = true;
        }
        else {
            levelStartButton.interactable = false;
        }*/
    }

    public bool LevelIsUnlocked(int levelNumber) {
        if (levelNumber == 1) {
            return true;
        }
        else if (GetLevelScore(levelNumber - 1) > 0) {
            return true;
        }
        return false;
    }

    public int GetLevelScore(int levelNumber) {
        int levelIndex = levelNumber - 1;
        Score score = new Score(GameData.GetFilePath());
        var scoreEntry = score.GetEntry(levelIndex);
        return scoreEntry.Score;
    }
}
