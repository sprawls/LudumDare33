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
    public StatsData statsData { get; private set; }

    [Header("World Switch")]
    public Camera normalCam;
    public Camera inverseCam;
    public Mesh transitionMesh;
    //JS scripts
    public GameObject screenWipeScript;
    public bool inInverseAnimation { get; private set; }

    private GooglePlayServiceHelper _GPSHelper;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            Application.targetFrameRate = 60;

            DontDestroyOnLoad(gameObject);
            inInverseAnimation = false;

            //Create Worlds
            worldList_par.Add(LevelsList_w1_par);
            worldList_par.Add(LevelsList_w2_par);
            worldList_par.Add(LevelsList_w3_par);
            worldList_par.Add(LevelsList_w4_par);
            worldList_par.Add(LevelsList_w5_par);
            worldList_par.Add(LevelsList_w6_par);
            LoadLevelsFromSave();
            if (SceneManager.GetActiveScene() != SceneManager.GetSceneAt(0)) UpdateBackgroundColor();

            //Activate GooglePlay Services
            _GPSHelper = gameObject.AddComponent<GooglePlayServiceHelper>();

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
            MoveLevelSelectTo(currentSelectedWorld, 0f);
            Debug.Log(currentSelectedWorld);
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

    void MoveLevelSelectTo(int world, float time = 1f) {
        WorldsPosition worldPosScript = GameObject.Find("Camera Parent").GetComponent<WorldsPosition>();
        worldPosScript.SetWorldPosition((int)Mathf.Ceil((float)world / 2f) - 1, time);
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
                ChangeColorByInverseWorld.UpdateAllColor();

                yield return new WaitForSeconds(2f);
            }         

            inInverseAnimation = false;
        }

    }

    IEnumerator FakeInverseWorld() {
        ChangeColorByInverseWorld.UpdateAllColor();
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
        statsData = GameSave.current.stats;

        //Create new if not existing
        if (levelsData == null) levelsData = new LevelsData();
        if (statsData == null) statsData = new StatsData();


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

        GameSave.current.levels = levelsData;
        GameSave.current.stats = statsData;
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

        UIGetter_Stars.UpdateAll();
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

    public int GetTotalAmountStars() {
        int amtStars = 0;
        for (int i = 0; i < levelsData.levelList.Count; ++i) {
            if (levelsData.levelList[i].completed) ++amtStars;
            if (levelsData.levelList[i].completedPar) ++amtStars;
        }
        return amtStars;
    }

    public void UpdatePersistentData(GameSave newSave) {
        levelsData = newSave.levels;
        statsData = newSave.stats;
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

    public void OpenGooglePlay() {
        if(!_GPSHelper.IsAuthentificated()) {
            _GPSHelper.AttemptToConnectUser();
        }
    }

    /// <summary>
    /// This is called in level select in order to re evaluate conditions for achievements in case the user was offline when he met the requirements
    /// </summary>
    public void CheckAchievementsProgress() {
        //Learner
        if (statsData.stats_tutorial_completed) _GPSHelper.UnlockAchievement(Achievements.Learner);

        //Curious
        if (statsData.stats_curiousness_achieved) _GPSHelper.UnlockAchievement(Achievements.Mastery_Curiousness);

        // Harmony-Chaos
        bool w1 = true, w2 = true, w3 = true, w4 = true;
        if(levelsData.levelList.Count > 30) {
            foreach(Level lvl in levelsData.levelList) {
                if (!lvl.completed) {
                    switch (lvl.world) {
                        case 1: w1 = false; break;
                        case 2: w2 = false; break;
                        case 3: w3 = false; break;
                        case 4: w4 = false; break;
                    }
                }
            }
            if (w1) _GPSHelper.UnlockAchievement(Achievements.Hamory_Centaurus);
            if (w2) _GPSHelper.UnlockAchievement(Achievements.Chaos_Centaurus);
            if (w3) _GPSHelper.UnlockAchievement(Achievements.Hamory_Hydra);
            if (w4) _GPSHelper.UnlockAchievement(Achievements.Chaos_Hydra);
        }

        //Mastery
        if (statsData.stats_amt_rotations > Achievements.Mastery_Rotation_Needed)
            _GPSHelper.UnlockAchievement(Achievements.Mastery_Rotation);
        if (statsData.stats_amt_fusions > Achievements.Mastery_Fusion_Needed)
            _GPSHelper.UnlockAchievement(Achievements.Mastery_Fusion);
        if (statsData.stats_amt_rotations_limitedTriangle > Achievements.Mastery_Limitations_Needed)
            _GPSHelper.UnlockAchievement(Achievements.Mastery_Limitations);
        if (statsData.stats_amt_rotations_multicolor > Achievements.Mastery_Multicolor_Needed)
            _GPSHelper.UnlockAchievement(Achievements.Mastery_Multicolor);
        if (statsData.stats_amt_resets > Achievements.Mastery_Commitment_Needed)
            _GPSHelper.UnlockAchievement(Achievements.Mastery_Commitment);

        //Equilibrium
        int amtStars = GetTotalAmountStars();
        if (amtStars >= Achievements.Equilibrium_Stage1_Needed) _GPSHelper.UnlockAchievement(Achievements.Equilibrium_Stage1);
        if (amtStars >= Achievements.Equilibrium_Stage2_Needed) _GPSHelper.UnlockAchievement(Achievements.Equilibrium_Stage2);
        if (amtStars >= Achievements.Equilibrium_Stage3_Needed) _GPSHelper.UnlockAchievement(Achievements.Equilibrium_Stage3);
        if (amtStars >= Achievements.Equilibrium_Stage4_Needed) _GPSHelper.UnlockAchievement(Achievements.Equilibrium_Stage4);
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

[System.Serializable]
public class StatsData {
    public int stats_amt_fusions;
    public int stats_amt_rotations;
    public int stats_amt_rotations_limitedTriangle;
    public int stats_amt_rotations_multicolor;
    public int stats_amt_resets;

    public bool stats_tutorial_completed;
    public bool stats_curiousness_achieved;

    public StatsData() {
        stats_amt_fusions = 0;
        stats_amt_rotations = 0;
        stats_amt_rotations_limitedTriangle = 0;
        stats_amt_rotations_multicolor = 0;
        stats_amt_resets = 0;

        stats_tutorial_completed = false;
        stats_curiousness_achieved = false;
    }
}