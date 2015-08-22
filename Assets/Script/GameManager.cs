using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public static GameManager Instance {  get; private set; }

    public List<GameObject> LevelsList;
    public int currentLevel = 0;
    public GameObject currentLevel_Obj;


    void Awake() {
        if (Instance == null) {
            Instance = this;
            GameObject.DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        LoadLevel();
    }

    void OnDestroy() {
        if (Instance == this) {
            Instance = null;
        }
    }

    public void CompleteLevel() {
        //TODO : Play Anim
        currentLevel++;
        LoadLevel();
    }

    private void LoadLevel() {
        if (currentLevel_Obj != null) Destroy(currentLevel_Obj);
        if (LevelsList.Count > currentLevel) currentLevel_Obj = (GameObject)Instantiate(LevelsList[currentLevel], transform.position, Quaternion.identity);
        else Debug.Log("All LEvels COMPLETED !");
    }
	

}
