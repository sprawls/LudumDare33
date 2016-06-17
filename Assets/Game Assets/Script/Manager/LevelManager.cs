﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {

    public static LevelManager Instance;

    public int currentSelectedWorld = 0;
    public int currentSelectedLevel = 0;
    public bool showTutorial = false;

    public List<int> LevelsList_w1_par;
    public List<int> LevelsList_w2_par;
    public List<List<int>> worldList_par = new List<List<int>>();
    public LevelsData levelsData { get; private set; }

    void Awake() {
        if (Instance == null) {
            Instance = this;
            worldList_par.Add(LevelsList_w1_par);
            worldList_par.Add(LevelsList_w2_par);
            LoadLevelsFromSave();
        } else {
            Destroy(gameObject);
        }
    }

    void OnDestroy() {
        if (Instance == this) {
            Instance = null;
            SaveAndLoad.Save();
        }
    }

    void LoadLevelsFromSave() {
        SaveAndLoad.Load();
        levelsData = GameSave.current.levels;

        List<Level> newLevelList = new List<Level>();
        for (int i = 0; i < worldList_par.Count; ++i) {
            for (int j = 0; j < worldList_par[i].Count; ++j) {
                Level previousLevelData = PopLevelInList(levelsData.levelList, i+1, j);
                if(previousLevelData != null) {
                    newLevelList.Add(new Level(GetLevelID(i+1, j), previousLevelData.unlocked, previousLevelData.completed, previousLevelData.completedPar));
                    Debug.Log("level " + (j) + " found. Completed : " + previousLevelData.completed + "    completedPar : " + previousLevelData.completedPar);
                } else {
                    newLevelList.Add(new Level(GetLevelID(i+1,j)));
                }
                newLevelList[newLevelList.Count - 1].SetPar( worldList_par[i][j]);
            }
        }
        levelsData.levelList = newLevelList;
        SaveAndLoad.Save();
    }

    Level PopLevelInList(List<Level> list, int world, int lvl) {
        foreach (Level curLevel in list) {
            if (curLevel.world == world && curLevel.lvl == lvl) {
                list.Remove(curLevel); // remove for performance reason
                return (curLevel);
            }
        }
        return null;
    }

    public void StartLevel(int world, int level, bool showTuto) {
        showTutorial = showTuto;
        currentSelectedWorld = world;
        currentSelectedLevel = level;
        SceneManager.LoadScene("Main");
    }

    public void CompleteLevel(int moves, int parMoves) {
        string levelID = GetLevelID(currentSelectedWorld+1,currentSelectedLevel);
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

    public string GetLevelID(int world, int level) {
        return ("world_" + world + "_" + level);
    }

    public int GetLevelPar(int worldRequested, int levelRequested) {
        for (int i = 0; i < levelsData.levelList.Count; ++i) {
            if (levelsData.levelList[i].world == worldRequested && levelsData.levelList[i].lvl == levelRequested) {
                return levelsData.levelList[i].par;
            }
        }
        Debug.LogError("Tried to get par of level that does not exist !");
        return -1;
    }
}

[System.Serializable]
public class LevelsData {
    public List<Level> levelList;

    public LevelsData() {
        levelList = new List<Level>();
    }
}