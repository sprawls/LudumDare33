using UnityEngine;
using System.Collections;
using Soomla.Levelup;

public class LevelSelectManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Debug_LevelScore(int level) {
        Debug.Log("lvl 0 stars: " + SoomlaInitializer.GetLevelScore(0, level, true)) ;
        Debug.Log("lvl 0 moves: " + SoomlaInitializer.GetLevelScore(0, level, false));
        Debug.Log("lvl 1 stars: " + SoomlaInitializer.GetLevelScore(0, level+1, true));
        Debug.Log("lvl 1 moves: " + SoomlaInitializer.GetLevelScore(0, level+1, false));
    }

    public void Debug_Set3Starts(int level) {
        string id = "world_A_" + level.ToString();
        Debug.Log("Amount of worlds + levels : " + SoomlaLevelUp.GetWorldCount(true));

        Level levelRef = SoomlaLevelUp.GetLevel(id);

        levelRef.Start();
        levelRef.SetScoreValue(levelRef.ID + "_moves", 5);
        levelRef.SetScoreValue(levelRef.ID + "_stars", 1);
        levelRef.End (true);
    }


}
