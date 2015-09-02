using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

    public static LevelManager Instance;

    public int currentSelectedWorld = 0;
    public int currentSelectedLevel = 0;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void OnDestroy() {
        if (Instance == this) {
            Instance = null;
        }
    }


    public void StartLevel(int world, int level) {
        currentSelectedWorld = world;
        currentSelectedLevel = world;
        Application.LoadLevel("Main");
    }
}
