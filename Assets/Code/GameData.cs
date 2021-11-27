using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public static class GameData {

    public const string FileName = "gamedata.json";
    public const string GameFolder = "TheTaintedForest";

    public static string GetDirectory() {
        string myDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        return Path.Combine(myDocs, GameFolder);
    }

    public static string GetFilePath() {
        return Path.Combine(GetDirectory(), FileName);
    }
}
