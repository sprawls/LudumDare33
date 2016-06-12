using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class Level {

    public string id {private set; get;}
    public bool unlocked { private set; get; }
    public bool completed { private set; get; }
    public bool completedPar { private set; get; }
    public int world { private set; get; }
    public int lvl { private set; get; }

    public Level(string _id) {
        id = _id;
        unlocked = false;
        completed = false;
        completedPar = false;
        SetWorldAndLvl();
    }

    public Level(string _id, bool _unlocked, bool _completed, bool _completedPar) {
        id = _id;
        unlocked = _unlocked;
        completed = _completed;
        completedPar = _completedPar;
        SetWorldAndLvl();
    }

    private void SetWorldAndLvl() {
        string[] splitID = id.Split('_');
        world = Int32.Parse(splitID[1]);
        lvl = Int32.Parse(splitID[2]);
    }

    public void SetCompleted() {
        completed = true;
    }

    public void SetCompletedPar() {
        completedPar = true;
    }

    public void Unlock() {
        unlocked = true;
    }


}
