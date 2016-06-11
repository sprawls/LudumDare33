using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {

    public static LevelManager Instance;

    public int currentSelectedWorld = 0;
    public int currentSelectedLevel = 0;
    public bool showTutorial = false;

    private List<Level> levelList;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            levelList = new List<Level>();
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
        for (int i = 0; i < levelList.Count; ++i) {
            if (levelList[i].id == wantedID) {
                return levelList[i];
            }
        }
        return null;
    }

    public int GetWorldCompletedLevels(int worldRequested) {
        int amt = 0;
        for (int i = 0; i < levelList.Count; ++i) {
            if (levelList[i].world == worldRequested && levelList[i].completed) {
                ++amt;
            }
        }
        return amt;
    }
}
