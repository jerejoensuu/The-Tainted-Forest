using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace TaintedForest {
    public class Score {

        [System.Serializable]
        public struct Entry {
            public int Score;
        }

        [System.Serializable]
        public struct ScoreArray {
            public Entry[] scores;

            public ScoreArray(int length) {
                scores = new Entry[length];
            }

            public bool Add(int level, int score) {
                Entry newEntry = new Entry() {
                    Score = score
                };

                bool highscore = false;

                if (scores[level].Score < newEntry.Score) {
                    scores[level] = newEntry;
                    highscore = true;
                }

                return highscore;
            }

            public Entry GetEntry(int index) {
                return scores[index];
            }
        }

        [SerializeField]
        private ScoreArray scores;
        private string scoreFile;
        public int scoreEntries = Levels.numberOfLevels;

        public Score(string path) {
            scoreFile = path;
            scores = new ScoreArray(scoreEntries);
            Load();
        }

        public bool Add(int level, int score) {
            Debug.Log("Score saved to " + scoreFile + " (" + score + " points @ level index " + level + ")");
            return scores.Add(level, score);
        }

        public void Save() {
            if (!Directory.Exists(GameData.GetDirectory())) {
                Directory.CreateDirectory(GameData.GetDirectory());
            }

            string json = JsonUtility.ToJson(scores);
            File.WriteAllText(scoreFile, json, System.Text.Encoding.UTF8);
        }

        public bool Load() {
            if (!File.Exists(scoreFile)) {
                return false;
            }

            string json = File.ReadAllText(scoreFile, System.Text.Encoding.UTF8);
            scores = JsonUtility.FromJson<ScoreArray>(json);
            return true;
        }

        public Entry GetEntry(int index) {
            return scores.GetEntry(index);
        }

        public bool FillScoreArray() {
            Score score = new Score(GameData.GetFilePath());
            ScoreArray newScores = new ScoreArray(scoreEntries);
            int originalLength = score.scores.scores.Length;

            for (int i = 0; i < scoreEntries; i++) {
                if (i < originalLength) {
                    newScores.Add(i, score.scores.scores[i].Score);
                }
                else {
                    newScores.Add(i, 0);
                }
            }

            scores.scores = newScores.scores;
            Save();

            return true;
        }
    }
}
