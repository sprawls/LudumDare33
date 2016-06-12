using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {

    public static LevelManager Instance;

    public int currentSelectedWorld = 0;
    public int currentSelectedLevel = 0;
    public bool showTutorial = false;

    public LevelsData levelsData { get; private set; }

    void Awake() {
        if (Instance == null) {
            Instance = this;
            SaveAndLoad.Load();
            levelsData = GameSave.current.levels;
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
        SceneManager.LoadScene("Main");
    }

    public void CompleteLevel(int moves, int parMoves) {
        string levelID = "world_" + currentSelectedWorld + "_" + currentSelectedLevel;
        Level levelRef = GetLevel(levelID);
        if(levelRef != null) {
            levelRef.SetCompleted();
            if (moves <= parMoves) levelRef.SetCompletedPar();
            currentSelectedLevel++;
        } else {
            Debug.Log("Requested Level Does not exist");
        }

       
    }

    public Level GetLevel(string wantedID) {
        for (int i = 0; i < levelsData.levelList.Count; ++i) {
            if (levelsData.levelList[i].id == wantedID) {
                return levelsData.levelList[i];
            }
        }
        return null;
    }

    public int GetWorldCompletedLevels(int worldRequested) {
        int amt = 0;
        for (int i = 0; i < levelsData.levelList.Count; ++i) {
            if (levelsData.levelList[i].world == worldRequested && levelsData.levelList[i].completed) {
                ++amt;
            }
        }
        return amt;
    }

    public void UpdatePersistentData(GameSave newSave) {
        levelsData = newSave.levels;
    }
}

[System.Serializable]
public class LevelsData {
    public List<Level> levelList;

    public LevelsData() {
        levelList = new List<Level>();
    }
}