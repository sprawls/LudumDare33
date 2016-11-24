using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;

public class LevelManager : MonoBehaviour {

    public static event Action NewLevelUnlocked;

    public static bool RecordingMode = true;

    public static LevelManager Instance;

    public int currentSelectedWorld = 1;
    public int currentSelectedLevel = 0;
    public bool showTutorial = false;
    public bool gameIsPaused { get; private set; }
    public bool popupIsOnScreen { get; private set; }

    public List<int> LevelsList_w1_par;
    public List<int> LevelsList_w2_par;
    public List<int> LevelsList_w3_par;
    public List<int> LevelsList_w4_par;
    public List<int> LevelsList_w5_par;
    public List<int> LevelsList_w6_par;
    public List<List<int>> worldList_par = new List<List<int>>();

    public LevelsData levelsData { get; private set; }
    public StatsData statsData { get; private set; }
    public OptionsData optionsData { get; private set; }

    [Header("World Switch")]
    public Camera normalCam;
    public Camera inverseCam;
    public Mesh transitionMesh;

    //JS scripts
    public GameObject screenWipeScript;
    public bool inInverseAnimation { get; private set; }

    private GooglePlayServiceHelper _GPSHelper;

    [Header("Audio")]
    public AudioMixerGroup soundMixerGroup;
    public AudioMixerGroup musicMixerGroup;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            Application.targetFrameRate = 60;

            DontDestroyOnLoad(gameObject);
            inInverseAnimation = false;
            OnSetPause(false);
            popupIsOnScreen = false;

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

            StartCoroutine(UpdateAudioGroupsOnDelay(0f));
        } else {
            Destroy(gameObject);
        }
    }

    void Update() {
        if (Input.GetKeyDown("space")) {
            RecordingMode = !RecordingMode;
            DeactivateIfRecording.UpdateAll();
        }      
    }

    void OnDestroy() {
        if (Instance == this) {
            Instance = null;
            SaveAndLoad.Save();
        }
    }

    void OnLevelWasLoaded(int scene) {
        if (Instance != this) return;

        StartCoroutine(UpdateAudioGroupsOnDelay(0f));
        OnSetPause(false);

        if (scene == 1) {
            UpdateBackgroundColor();
            if (isInverseWorld()) StartCoroutine(FakeInverseWorld());
            MoveLevelSelectTo(currentSelectedWorld, 0f);
            
        }
    }

    IEnumerator UpdateAudioGroupsOnDelay(float delay) {
        yield return new WaitForSeconds(delay);
        UpdateAudioGroups();
    }

    public void OnSetPause(bool isPaused) {
        if (isPaused) gameIsPaused = true;
        else gameIsPaused = false;
    }

    public void OnSetPopupIsOnScreen(bool isOnScreen) {
        if (isOnScreen) popupIsOnScreen = true;
        else popupIsOnScreen = false;
    }

    //Save when application pause
    void OnApplicationPause(bool pauseStatus) {
        SaveAndLoad.Save();
    }
    void OnApplicationQuit() {
        SaveAndLoad.Save();
    }

    public bool CanClickOnLevels() {
        if (inInverseAnimation || SceneTransitionManager.Instance.InTransition || gameIsPaused || popupIsOnScreen) return false;
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

    public bool CurrentLevelIsUnlocked() {
        Level curLevel = GetCurrentLevel();
        return curLevel.unlocked;
    }

    public void UnlockLevel(int world, int level) {
        Level levelInfo = GetLevel(GetLevelID(world,level));
        levelInfo.Unlock();
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
        optionsData = GameSave.current.options;

        //Create new if not existing
        if (levelsData == null) levelsData = new LevelsData();
        if (statsData == null) statsData = new StatsData();
        if (optionsData == null) optionsData = new OptionsData();


        //Get Levels from old data. Should support adding more levels (savefile with less levels than currently)
        List<Level> newLevelList = new List<Level>();
        for (int i = 0; i < worldList_par.Count; ++i) {
            for (int j = 0; j < worldList_par[i].Count; ++j) {
                Level previousLevelData = PopLevelInList(levelsData.levelList, i+1, j);
                if(previousLevelData != null) {
                    newLevelList.Add(new Level(GetLevelID(i+1, j), previousLevelData.unlocked, previousLevelData.completed, previousLevelData.completedPar, previousLevelData.bestMoveScore));
                    //Debug.Log("world" + (i + 1) + " level " + (j) + " found. Completed : " + previousLevelData.completed + "    completedPar : " + previousLevelData.completedPar + "    unlocked : " + previousLevelData.unlocked);
                } else {
                    newLevelList.Add(new Level(GetLevelID(i+1,j)));
                }
                newLevelList[newLevelList.Count - 1].SetPar( worldList_par[i][j]);
            }
        }
        levelsData.levelList = newLevelList;

        GameSave.current.levels = levelsData;
        GameSave.current.stats = statsData;
        GameSave.current.options = optionsData;

        //Debug.Log("Chaos Shown :" + statsData.notif_chaos_shown + "      Chaos Time : " + statsData.notif_chaos_time);

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

    public void NewLevelWasUnlocked(){
        if(NewLevelUnlocked != null) NewLevelUnlocked();
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

        CheckHarmonyChaosUnlockRequierements();
        CheckEquilibriumUnlockRequierements();
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

    public void OpenGameWebsite() {
        Application.OpenURL("http://sprawls.github.io/entropy");
    }

    public void OpenGooglePlayPage() {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.StudioMastodonte.Entropy");
    }

    //////////////////////////////////////////////////////
    ///  DATA 
    //////////////////////////////////////////////////////

    public bool hasCompletedTutorial() {
        return levelsData.tutorialCompleted;
    }

    public void SetTutorialCompleted() {
        LevelManager.Instance.levelsData.tutorialCompleted = true;
        LevelManager.Instance.statsData.stats_tutorial_completed = true;
        _GPSHelper.UnlockAchievement(Achievements.Learner);
    }

    public void IncrementRotations(int amount = 1) {
        statsData.stats_amt_rotations += amount;
        Debug.Log("Incremented Rotations by " + amount + " to " + statsData.stats_amt_rotations);
        CheckMasteryUnlockRequierements();
    }
    public void IncrementRotations_LimitedTri(int amount = 1) {
        statsData.stats_amt_rotations_limitedTriangle += amount;
        Debug.Log("Incremented LimitedMoves by " + amount + " to " + statsData.stats_amt_rotations_limitedTriangle);
        CheckMasteryUnlockRequierements();
    }
    public void IncrementRotations_Multicolor(int amount = 1) {
        statsData.stats_amt_rotations_multicolor += amount;
        Debug.Log("Incremented Multicolor by " + amount + " to " + statsData.stats_amt_rotations_multicolor);
        CheckMasteryUnlockRequierements();
    }
    public void IncrementResets(int amount = 1) {
        statsData.stats_amt_resets += amount;
        Debug.Log("Incremented Resets by " + amount + " to " + statsData.stats_amt_resets);
        CheckMasteryUnlockRequierements();
    }
    public void IncrementFusions(int amount = 1) {
        statsData.stats_amt_fusions += amount;
        Debug.Log("Incremented Fusions by " + amount + " to " + statsData.stats_amt_fusions);
        CheckMasteryUnlockRequierements();
    }

    public void ToggleMuteSounds() {
        optionsData.sound_muted = !optionsData.sound_muted;
        UpdateAudioGroups();
    }

    public void ToggleMuteMusic() {
        optionsData.music_muted = !optionsData.music_muted;
        UpdateAudioGroups();
    }

    private void UpdateAudioGroups() {
        if (optionsData.music_muted) musicMixerGroup.audioMixer.SetFloat("MusicVolume", -80f);
        else musicMixerGroup.audioMixer.SetFloat("MusicVolume", 0f);

        if (optionsData.sound_muted) soundMixerGroup.audioMixer.SetFloat("SFXVolume", -80f);
        else soundMixerGroup.audioMixer.SetFloat("SFXVolume", 0f);
    }

    //////////////////////////////////////////////////////
    ///  GOOGLE PLAY
    //////////////////////////////////////////////////////
    public void OpenGooglePlay() {
        if(!_GPSHelper.IsAuthenticated()) {
            _GPSHelper.AttemptToConnectUser();
        }

        OpenGooglePlayAchievements();
    }

    public void OpenGooglePlayAchievements() {
        if (!_GPSHelper.IsAuthenticated()) return;
        _GPSHelper.ShowAchievementsUI();
    }

    //////////////////////////////////////////////////////
    ///  ACHIEVEMENTS
    //////////////////////////////////////////////////////
    /// <summary>
    /// This is called in level select in order to re evaluate conditions for achievements in case the user was offline when he met the requirements
    /// </summary>
    public void CheckAchievementsProgress() {
        //Learner
        if (statsData.stats_tutorial_completed || levelsData.tutorialCompleted) _GPSHelper.UnlockAchievement(Achievements.Learner);
        //Curious
        if (statsData.stats_curiousness_achieved) _GPSHelper.UnlockAchievement(Achievements.Mastery_Curiousness);
        // Harmony-Chaos
        CheckHarmonyChaosUnlockRequierements();
        //Mastery
        CheckMasteryUnlockRequierements();
        //Equilibrium
        CheckEquilibriumUnlockRequierements();
    }

    private void CheckHarmonyChaosUnlockRequierements() {
        bool w1 = true, w2 = true, w3 = true, w4 = true;
        if (levelsData.levelList.Count > 30) {
            foreach (Level lvl in levelsData.levelList) {
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
    }

    private void CheckMasteryUnlockRequierements() {
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
    }

    private void CheckEquilibriumUnlockRequierements() {
        int amtStars = GetTotalAmountStars();
        if (amtStars >= Achievements.Equilibrium_Stage1_Needed) _GPSHelper.UnlockAchievement(Achievements.Equilibrium_Stage1);
        if (amtStars >= Achievements.Equilibrium_Stage2_Needed) _GPSHelper.UnlockAchievement(Achievements.Equilibrium_Stage2);
        if (amtStars >= Achievements.Equilibrium_Stage3_Needed) _GPSHelper.UnlockAchievement(Achievements.Equilibrium_Stage3);
        if (amtStars >= Achievements.Equilibrium_Stage4_Needed) _GPSHelper.UnlockAchievement(Achievements.Equilibrium_Stage4);
    }


}

//////////////////////////////////////////////////////
///  Data Structures
//////////////////////////////////////////////////////

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

    public DateTime notif_chaos_time;
    public DateTime notif_rate_time;
    public DateTime notif_tip_time;
    public bool notif_chaos_shown;
    public bool notif_rate_shown;
    public bool notif_tip_shown;

    public StatsData() {
        stats_amt_fusions = 0;
        stats_amt_rotations = 0;
        stats_amt_rotations_limitedTriangle = 0;
        stats_amt_rotations_multicolor = 0;
        stats_amt_resets = 0;

        stats_tutorial_completed = false;
        stats_curiousness_achieved = false;

        notif_chaos_time = DateTime.MinValue;
        notif_rate_time = DateTime.MinValue;
        notif_tip_time = DateTime.MinValue;
        notif_chaos_shown = false;
        notif_rate_shown = false;
        notif_tip_shown = false;
    }
}

[System.Serializable]
public class OptionsData {
    public bool sound_muted;
    public bool music_muted;
    public bool low_quality;

    public OptionsData() {
        sound_muted = false;
        music_muted = false;
        low_quality = false;
    }
}