using UnityEngine;
using System.Collections;

public class LevelSelectOrb : MonoBehaviour {

    public enum WorldsEnum { world_1, world_2 }

    public GameObject orbPrefab_locked;
    public GameObject orbPrefab_unlocked;
    public GameObject orbPrefab_completed;
    public GameObject orbPrefab_completedPar;
    public GameObject orbPrefab_locked_inverse;
    public GameObject orbPrefab_unlocked_inverse;
    public GameObject orbPrefab_completed_inverse;
    public GameObject orbPrefab_completedPar_inverse;
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

    void OnMouseDown() {
        switch (world) {
            case WorldsEnum.world_1:
                LevelManager.Instance.StartLevel(1, level, showTutorial);
                break;
            case WorldsEnum.world_2:
                LevelManager.Instance.StartLevel(2, level, false);
                break;
        }
        
    }

    public void UpdateOrb(){
       Level currentLevel = LevelManager.Instance.GetLevel(GetLevelId());
       bool isInInverseWorld = (LevelManager.Instance.currentSelectedWorld % 2 == 0);
       if (currentLevel != null) {
           int completedLevels = GetAmtLvlCompleted();
           if (completedLevels >= pointsRequired) {
               buttonCollider.enabled = true;
               if (LevelManager.Instance.GetLevel(GetLevelId()).completedPar) {
                   if(!isInInverseWorld) SpawnOrb(orbPrefab_completedPar);
                   else SpawnOrb(orbPrefab_completedPar_inverse);
               } else if (LevelManager.Instance.GetLevel(GetLevelId()).completed) {
                   if (!isInInverseWorld) SpawnOrb(orbPrefab_completed);
                   else SpawnOrb(orbPrefab_completed_inverse);
               } else {
                   if (!isInInverseWorld) SpawnOrb(orbPrefab_unlocked);
                   else SpawnOrb(orbPrefab_unlocked_inverse);
               }
           } else {
               if (!isInInverseWorld) SpawnOrb(orbPrefab_locked);
               else SpawnOrb(orbPrefab_locked_inverse);
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
                break;
            case WorldsEnum.world_2:
                levelsCompleted = LevelManager.Instance.GetWorldCompletedLevels(2);
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
