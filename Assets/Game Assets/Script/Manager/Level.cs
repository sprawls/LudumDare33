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
    public int par { private set; get; }
    public int bestMoveScore { private set; get; }

    public Level(string _id) {
        id = _id;
        unlocked = false;
        completed = false;
        completedPar = false;
        par = 0;
        bestMoveScore = -1;
        SetWorldAndLvl();
    }

    public Level(string _id, bool _unlocked, bool _completed, bool _completedPar, int _bestMoveScore) {
        id = _id;
        unlocked = _unlocked;
        completed = _completed;
        completedPar = _completedPar;
        par = 0;
        bestMoveScore = _bestMoveScore;
        SetWorldAndLvl();
    }

    private void SetWorldAndLvl() {
        string[] splitID = id.Split('_');
        world = Int32.Parse(splitID[1]);
        lvl = Int32.Parse(splitID[2]);
    }

    public bool BestMoveScoreIsUnset() {
        return bestMoveScore < 1;
    }

    public void SetCompleted() {
        completed = true;
    }

    public void SetCompletedPar() {
        completedPar = true;
    }

    public void SetBestMoveScore(int _moves) {
        bestMoveScore = _moves;
    }

    public void Unlock() {
        if (!unlocked) {
            unlocked = true;
            LevelManager.Instance.NewLevelWasUnlocked();
        }
    }

    public void SetPar(int _par) {
        par = _par;
    }
}
