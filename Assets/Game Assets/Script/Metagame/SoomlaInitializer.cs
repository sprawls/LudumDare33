using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Soomla;
using Soomla.Highway;
using Soomla.Store;
using Soomla.Profile;
using Soomla.Levelup;



public class SoomlaInitializer : MonoBehaviour {

    public static SoomlaInitializer Instance;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            GameObject.DontDestroyOnLoad(gameObject);
            // Make sure to make this call in your earlieast loading scene,
            // and before initializing any other SOOMLA components
            // i.e. before SoomlaStore.Initialize(...)
            SoomlaHighway.Initialize();
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        SoomlaStore.Initialize(new EntropyAssets());
        SoomlaProfile.Initialize();
        SoomlaLevelUp.Initialize(createMainWorld());
    }

    private World createMainWorld() {
        World world_1 = new World("world_1");
        World world_2 = new World("world_2");

        //Gates
        #region Gates
        Gate world_2_Gate = new WorldCompletionGate(
            "WGate_B",                              // ID
            world_1.ID                             // Associated World ID
        );
        #endregion

        // Add 10 levels to each world
        for (int i = 0; i < 10; i++) {
            world_1.AddInnerWorld(new Level("world_1_" + i));
            world_2.AddInnerWorld(new Level("world_2_" + i));
        }
        // Create a world that will contain all worlds of the game
        World mainWorld = new World("main_world");
        mainWorld.InnerWorldsMap.Add(world_1.ID, world_1);
        mainWorld.InnerWorldsMap.Add(world_2.ID, world_2);


        foreach (World world in mainWorld.InnerWorldsList) {
            foreach (World level in world.InnerWorldsList) {
                string levelName = level.ID;
                level.AddScore(new RangeScore(levelName + "_stars", "stars", true, new RangeScore.SRange(0, 3)));
                level.AddScore(new Score(levelName + "_moves", "moves", false));
            }
        }

        return mainWorld;
    }

    /// <summary>
    /// Get the Score of a Level
    /// </summary>
    /// <param name="world"> world of the level </param>
    /// <param name="level"> id of the level </param>
    /// <param name="isStars"> true if we want star score, false if we want moves score </param>
    /// <returns></returns>
    public static int GetLevelScore(int world, int level, bool isStars) {
        Dictionary<string, double> recordScores = null;
        string wantedScoreKey;

        //Get Level ID with world
        string levelID = "";
        switch (world) {
            case 0 :
                levelID = "world_1_";
                break;
            case 1 :
                levelID = "world_2_";
                break;
        }
        //Complete level ID with level
        levelID += level.ToString();
        Level lvl = SoomlaLevelUp.GetLevel(levelID);
        if (lvl == null) {
            Debug.Log("Level is null !");
        } else {
            recordScores = SoomlaLevelUp.GetLevel(levelID).GetRecordScores();
        }

        //Get wanted Key 
        wantedScoreKey = lvl.ID;
        if (isStars) wantedScoreKey += "_stars";
        else wantedScoreKey += "_moves";

        //Get the pairs of scores and return the wanted one
        if (recordScores != null) {
            foreach (KeyValuePair<string, double> entry in recordScores) {
                if (entry.Key == wantedScoreKey) {
                    return (int)entry.Value;
                }

                string message = entry.Key + ": " + entry.Value;
                //SoomlaUtils.LogDebug("", message);
            }
        }

        return -1;
    }

    /// <summary>
    /// Get the toal score for all levels in given world 
    /// </summary>
    /// <param name="world"> id of world </param>
    /// <param name="isStars"> true if we want star score, false if we want moves score </param>
    /// <returns></returns>
    public static int GetWorldScore(int world, bool isStars) {
        Dictionary<string, double> recordScores;
        string wantedScoreKey;

        //Get Level ID with world
        switch (world) {
            case 0:
                recordScores = SoomlaLevelUp.GetWorld("world_1").GetLatestScores();
                break;
            case 1:
                recordScores = SoomlaLevelUp.GetWorld("world_2").GetLatestScores();
                break;
            default :
                recordScores = null;
                break;
        }

        //Get wanted Key 
        if (isStars) wantedScoreKey = "stars";
        else wantedScoreKey = "moves";

        //Get the pairs of scores and return the wanted one
        int totalValue = 0;
        foreach (KeyValuePair<string, double> entry in recordScores) {
            if (entry.Key == wantedScoreKey) {
                totalValue += (int)entry.Value;
            }

            string message = entry.Key + ": " + entry.Value;
            //SoomlaUtils.LogDebug("", message);
        }

        return totalValue;
    }

    /// <summary>
    /// Get The amount of completed levels in a given world
    /// </summary>
    /// <param name="world">wanted world</param>
    /// <returns>Amount of completed levels </returns>
    public static int GetWorldCompletedLevels(int world){
        Dictionary<string, World> levels;

        //Get Level ID with world
        switch (world) {
            case 0:
                levels = SoomlaLevelUp.GetWorld("world_1").InnerWorldsMap;
                break;
            case 1:
                levels = SoomlaLevelUp.GetWorld("world_2").InnerWorldsMap;
                break;
            default :
                levels = null;
                break;
        }

        //Get the pairs of scores and return the wanted one
        int totalValue = 0;
        foreach (KeyValuePair<string, World> entry in levels) {
            if (entry.Value.IsCompleted() == true) {
                totalValue++;
            }

            string message = entry.Key + ": " + entry.Value;
            //Debug.Log(message);
        }

        return totalValue;
    }

}
