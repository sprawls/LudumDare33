using UnityEngine;
using System.Collections;

public class LevelSelectManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        
	}

    public void BackToMenu() {
        LevelManager.Instance.OnClick_BackToMenu();
    }

    public void InverseWorld() {
        LevelManager.Instance.OnClick_InverseWorld();
    }

    public void NextWorld() {
        LevelManager.Instance.OnClick_NextWorld();
    }

    public void PreviousWorld() {
        LevelManager.Instance.OnClick_PreviousWorld();
    }

}
