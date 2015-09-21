using UnityEngine;
using System.Collections;
using Soomla.Levelup;

public class LevelManager : MonoBehaviour {

    public static LevelManager Instance;

    public int currentSelectedWorld = 0;
    public int currentSelectedLevel = 0;
    public bool showTutorial = false;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void OnDestroy() {
        if (Instance == this) {
            Instance = null;
        }
    }


    public void StartLevel(int world, int level, bool showTuto) {
        showTutorial = showTuto;
        currentSelectedWorld = world;
        currentSelectedLevel = level;
        Application.LoadLevel("Main");
    }

    public void CompleteLevel(int moves, int parMoves) {
        string levelID = "world_" + currentSelectedWorld + "_" + currentSelectedLevel;
        Level levelRef = SoomlaLevelUp.GetLevel(levelID);

        int amtStars;
        if ((float)moves / (float)parMoves < 0.4f) amtStars = 1;
        else if ((float)moves / (float)parMoves < 0.8f) amtStars = 2;
        else amtStars = 3;

        levelRef.Start();
        levelRef.SetScoreValue(levelRef.ID + "_moves", moves);
        levelRef.SetScoreValue(levelRef.ID + "_stars", amtStars);
        levelRef.End(true);
        levelRef.SetCompleted(true);

        currentSelectedLevel++;
    }
}
