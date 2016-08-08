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
    public List<int> LevelsList_w3_par;
    public List<int> LevelsList_w4_par;
    public List<int> LevelsList_w5_par;
    public List<int> LevelsList_w6_par;
    public List<List<int>> worldList_par = new List<List<int>>();
    public LevelsData levelsData { get; private set; }

    [Header("World Switch")]
    public Camera normalCam;
    public Camera inverseCam;
    public Mesh transitionMesh;
    //JS scripts
    public GameObject screenWipeScript;
    public bool inInverseAnimation { get; private set; }


    void Awake() {
        if (Instance == null) {
            Instance = this;
            Application.targetFrameRate = 60;

            DontDestroyOnLoad(gameObject);
            inInverseAnimation = false;

            worldList_par.Add(LevelsList_w1_par);
            worldList_par.Add(LevelsList_w2_par);
            worldList_par.Add(LevelsList_w3_par);
            worldList_par.Add(LevelsList_w4_par);
            worldList_par.Add(LevelsList_w5_par);
            worldList_par.Add(LevelsList_w6_par);
            LoadLevelsFromSave();
            if (SceneManager.GetActiveScene() != SceneManager.GetSceneAt(0)) UpdateBackgroundColor();
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
        if (scene == 1) {
            UpdateBackgroundColor();
            if (isInverseWorld()) StartCoroutine(FakeInverseWorld());
        }
    }

    //Save when application pause
    void OnApplicationPause(bool pauseStatus) {
        SaveAndLoad.Save();
    }
    void OnApplicationQuit() {
        SaveAndLoad.Save();
    }

    public bool CanClickOnLevels() {
        if (inInverseAnimation || SceneTransitionManager.Instance.InTransition) return false;
        else return true;
    }

    public void OnClick_BackToMenu() {
        SceneTransitionManager.Instance.TransitionToAnotherScene(ScenesEnum.menu);
    }

    public void OnClick_InverseWorld() {
        StartCoroutine(InverseWorld());
    }

    public void OnClick_NextWorld() {
        if (currentSelectedWorld + 2 <= worldList_par.Count) {
            MoveLevelSelectTo(currentSelectedWorld + 2);
        }
    }

    public void OnClick_PreviousWorld() {
        if (currentSelectedWorld - 2 > 0) {
            MoveLevelSelectTo(currentSelectedWorld - 2);
        }
    }

    public bool CurrentLevelExists() {
        return currentSelectedLevel < worldList_par[currentSelectedWorld - 1].Count;
    }

    void MoveLevelSelectTo(int world) {
        WorldsPosition worldPosScript = GameObject.Find("Camera Parent").GetComponent<WorldsPosition>();
        worldPosScript.SetWorldPosition((int)Mathf.Ceil((float)world / 2f) - 1);
        currentSelectedWorld = world;
    }

    IEnumerator InverseWorld() {
        if (!inInverseAnimation) {
            MusicManager.Instance.PlaySound_InverseWorld();

            inInverseAnimation = true;

            screenWipeScript = GameObject.Find("Camera Parent");
            if (screenWipeScript != null) {
                screenWipeScript.SendMessage("InverseWorld", isInverseWorld());
                if (isInverseWorld()) --currentSelectedWorld;
                else ++currentSelectedWorld;

                MusicManager.Instance.UpdateSoundMixerSnapshots();

                yield return new WaitForSeconds(2f);
            }         

            inInverseAnimation = false;

        }
    }

    IEnumerator FakeInverseWorld() {
        if (!inInverseAnimation) {
            inInverseAnimation = true;
            yield return new WaitForSeconds(0.05f);

            screenWipeScript = GameObject.Find("Camera Parent");
            if (screenWipeScript != null) {
                screenWipeScript.SendMessage("FakeInverseWorld", !isInverseWorld());
            }

            yield return new WaitForSeconds(0.5f);
            inInverseAnimation = false;
        }
    }

    void LoadLevelsFromSave() {
        SaveAndLoad.Load();
        levelsData = GameSave.current.levels;
        //Get Levels from old data. Should support adding more levels (savefile with less levels than currently)
        List<Level> newLevelList = new List<Level>();
        for (int i = 0; i < worldList_par.Count; ++i) {
            for (int j = 0; j < worldList_par[i].Count; ++j) {
                Level previousLevelData = PopLevelInList(levelsData.levelList, i+1, j);
                if(previousLevelData != null) {
                    newLevelList.Add(new Level(GetLevelID(i+1, j), previousLevelData.unlocked, previousLevelData.completed, previousLevelData.completedPar, previousLevelData.bestMoveScore));
                    //Debug.Log("world" + (i+1) + " level " + (j) + " found. Completed : " + previousLevelData.completed + "    completedPar : " + previousLevelData.completedPar);
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
        SceneTransitionManager.Instance.TransitionToAnotherScene(ScenesEnum.main);
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
            levelRef.SetBestMoveScore(moves);
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

    public Level GetCurrentLevel() {
        string currentLevelID = GetLevelID(currentSelectedWorld, currentSelectedLevel);
        return GetLevel(currentLevelID);
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

    public bool GetWorldAllCompletedLevels(int worldRequested) {
        for (int i = 0; i < levelsData.levelList.Count; ++i) {
            if (levelsData.levelList[i].world == worldRequested && !levelsData.levelList[i].completed) {
                return false;
            }
        }
        return true;
    }

    public bool GetLevelIsCompleted(int worldRequested, int levelRequested) {
        for (int i = 0; i < levelsData.levelList.Count; ++i) {
            if (levelsData.levelList[i].world == worldRequested && levelsData.levelList[i].lvl == levelRequested) {
                if (levelsData.levelList[i].completed) return true;
                else return false;
            }
        }
        Debug.Log("Requested Level was not found : world " + worldRequested + " level " + levelRequested);
        return false;
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
        return 0;
    }

    public bool isInverseWorld() {
        return (currentSelectedWorld % 2 == 0);
    }

    public bool hasCompletedTutorial() {
        return levelsData.tutorialCompleted;
    }
}

[System.Serializable]
public class LevelsData {
    public List<Level> levelList;
    public bool tutorialCompleted;

    public LevelsData() {
        levelList = new List<Level>();
        tutorialCompleted = false;
    }
}