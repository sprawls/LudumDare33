using UnityEngine;
using System.Collections;

public class LevelSelectOrb : MonoBehaviour {

    public enum WorldsEnum { world_1, world_2 }

    public GameObject orbPrefab_locked;
    public GameObject orbPrefab_unlocked;
    public GameObject orbPrefab_completed;
    public SphereCollider buttonCollider;

    public WorldsEnum world;
    public int level;
    public int pointsRequired;
    public bool showTutorial = false;

    private GameObject spawnedOrbPrefab;


	// Use this for initialization
	void Start () {
        UpdateOrb();
	}

    void OnEnable() {
    }

    void OnMouseDown() {
        Debug.Log("Going To World " + world + " level " + level);
        switch (world) {
            case WorldsEnum.world_1:
                LevelManager.Instance.StartLevel(1, level, showTutorial);
                break;
            case WorldsEnum.world_2:
                LevelManager.Instance.StartLevel(2, level, showTutorial);
                break;
        }
        
    }

    public void UpdateOrb(){
       Level currentLevel = LevelManager.Instance.GetLevel(GetLevelId());
       if (currentLevel != null) {
           int completedLevels = GetAmtLvlCompleted();
           if (completedLevels >= pointsRequired) {
               buttonCollider.enabled = true;
               if (LevelManager.Instance.GetLevel(GetLevelId()).completedPar) {
                   SpawnOrb(orbPrefab_completed);
               } else if (LevelManager.Instance.GetLevel(GetLevelId()).completed) {
                   SpawnOrb(orbPrefab_completed);
               } else {
                   SpawnOrb(orbPrefab_unlocked);
               }
           } else {      
               SpawnOrb(orbPrefab_locked);
               buttonCollider.enabled = false;
           }
       } else {
           Debug.Log("Requested Level Does Not Exist !");
           Debug.Log(GetLevelId());
       }
    }

    public void OnScreenVisible() {

    }

    private int GetAmtLvlCompleted() {
        int levelsCompleted = 0;
        switch (world) {
            case WorldsEnum.world_1:
                levelsCompleted = LevelManager.Instance.GetWorldCompletedLevels(1);
                Debug.Log("Amount completed in world 1 : " + levelsCompleted);
                break;
            case WorldsEnum.world_2:
                levelsCompleted = LevelManager.Instance.GetWorldCompletedLevels(2);
                Debug.Log("Amount completed in world 2 : " + levelsCompleted);
                break;
        }    
        return levelsCompleted;
    }

    private string GetLevelId() {
        string id = world.ToString() + "_" + level.ToString();
        return id;
    }

    private void SpawnOrb(GameObject prefab) {
        if (spawnedOrbPrefab != null) {
            Destroy(spawnedOrbPrefab);
            spawnedOrbPrefab = null;
        } 
        spawnedOrbPrefab = (GameObject) Instantiate(prefab, transform.position, Quaternion.identity);
        spawnedOrbPrefab.transform.parent = transform;
        spawnedOrbPrefab.transform.localScale = Vector3.one;
    }
}
