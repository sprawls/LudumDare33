using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameSave {

    //STATIC INSTANCE
	public static GameSave current;

    //DATA IN A SAVE : 
    public int index = 0;

    public LevelsData levels;
    public StatsData stats;

    /// <summary> Creates a new gamesave from nothing </summary>
    public GameSave(bool createNew) {
        index = 0;

        if (createNew) {
            levels = new LevelsData();
            stats = new StatsData();
        } else {
            levels = LevelManager.Instance.levelsData;
            stats = LevelManager.Instance.statsData;
        }

    }
       
}
