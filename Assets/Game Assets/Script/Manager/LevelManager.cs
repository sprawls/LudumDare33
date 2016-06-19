using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {

    public static LevelManager Instance;

    public int currentSelectedWorld = 1;
    public int currentSelectedLevel = 0;
    public bool showTutorial = false;

    public List<int> LevelsList_w1_par;
    public List<int> LevelsList_w2_par;
    public List<List<int>> worldList_par = new List<List<int>>();
    public LevelsData levelsData { get; private set; }

    [Header("World Switch")]
    public Camera normalCam;
    public Camera inverseCam;
    public Mesh transitionMesh;
    //JS scripts
    public GameObject screenWipeScript;
    private bool inInverseAnimation = false;


    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            worldList_par.Add(LevelsList_w1_par);
            worldList_par.Add(LevelsList_w2_par);
            LoadLevelsFromSave();
            UpdateBackgroundColor();
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

    void OnLevelWasLoaded(int scene) {
        if (scene == 1) UpdateBackgroundColor();
    }

    public void OnClick_BackToMenu() {
        SceneManager.LoadScene("Menu");
    }

    public void OnClick_InverseWorld() {
        StartCoroutine(InverseWorld());
    }

    IEnumerator InverseWorld() {
        if (!inInverseAnimation) {
            inInverseAnimation = true;

            screenWipeScript = GameObject.Find("Camera Parent");
            if (screenWipeScript != null) {
                screenWipeScript.SendMessage("InverseWorld", isInverseWorld());
                if (isInverseWorld()) --currentSelectedWorld;
                else ++currentSelectedWorld;
                yield return new WaitForSeconds(2f);
            }

            

            inInverseAnimation = false;
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
                    Debug.Log("world" + (i+1) + " level " + (j) + " found. Completed : " + previousLevelData.completed + "    completedPar : " + previousLevelData.completedPar);
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

    public void UpdateBackgroundColor() {
        if (isInverseWorld()) Camera.main.backgroundColor = Color.white;
        else Camera.main.backgroundColor = Color.black;
    }

    public void CompleteLevel(int moves, int parMoves) {
        Debug.Log("Complete world/lvl : " + currentSelectedWorld + " / " + currentSelectedLevel);
        string levelID = GetLevelID(currentSelectedWorld,currentSelectedLevel);
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
        return -1;
    }

    public bool isInverseWorld() {
        return (currentSelectedWorld % 2 == 0);
    }
}

[System.Serializable]
public class LevelsData {
    public List<Level> levelList;

    public LevelsData() {
        levelList = new List<Level>();
    }
}