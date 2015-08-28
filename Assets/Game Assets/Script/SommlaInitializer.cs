using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Soomla;
using Soomla.Highway;
using Soomla.Store;
using Soomla.Profile;
using Soomla.Levelup;



public class SommlaInitializer : MonoBehaviour {

    public static SommlaInitializer Instance;

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
        World world_A = new World("world_A");
        World world_B = new World("world_B");

        //Gates
        #region Gates
        Gate world_B_Gate = new WorldCompletionGate(
            "WGate_B",                              // ID
            world_A.ID                             // Associated World ID
        );
        #endregion

        Score nullScore = null;
        Mission nullMission = null;

        // Add 10 levels to each world
        world_A.BatchAddLevelsWithTemplates(
            10, 
            null,
            nullScore,
            nullMission);

        world_B.BatchAddLevelsWithTemplates(
            10,
            world_B_Gate,
            nullScore,
            nullMission);

        // Create a world that will contain all worlds of the game
        World mainWorld = new World("main_world");
        mainWorld.InnerWorldsMap.Add(world_A.ID, world_A);
        mainWorld.InnerWorldsMap.Add(world_B.ID, world_B);

        return mainWorld;
    }


}
