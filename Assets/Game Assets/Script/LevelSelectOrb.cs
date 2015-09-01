using UnityEngine;
using System.Collections;
using Soomla.Levelup;

public class LevelSelectOrb : MonoBehaviour {

    public enum WorldsEnum { world_1, world_2 }

    public GameObject orbPrefab_locked;
    public GameObject orbPrefab_unlocked;
    public GameObject orbPrefab_completed;
    public CircleCollider2D buttonCollider;

    public WorldsEnum world;
    public int level;
    public int pointsRequired;

    private GameObject spawnedOrbPrefab;


	// Use this for initialization
	void Start () {
        UpdateOrb();
	}

    void OnEnable() {
    }

    void OnMouseDown() {
        Debug.Log("Going To World " + world + " level " + level);
    }

    void UpdateOrb(){
       int completedLevels = GetAmtLvlCompleted();
       if (completedLevels >= pointsRequired) {
           buttonCollider.enabled = true;
           Debug.Log("Level to check completion : " + SoomlaLevelUp.GetLevel(GetLevelId()) +  "   and its id : " + GetLevelId());
           if (SoomlaLevelUp.GetLevel(GetLevelId()).IsCompleted()) {
               SpawnOrb(orbPrefab_completed);
           } else {
               SpawnOrb(orbPrefab_unlocked);
           }
       } else {
           buttonCollider.enabled = false;
           SpawnOrb(orbPrefab_locked);
       }
    }

    private int GetAmtLvlCompleted() {
        int levelsCompleted = 0;
        switch (world) {
            case WorldsEnum.world_1:
                levelsCompleted = SoomlaInitializer.GetWorldCompletedLevels(0);
                Debug.Log("Amount completed in world 1 : " + levelsCompleted);
                break;
            case WorldsEnum.world_2:
                levelsCompleted = SoomlaInitializer.GetWorldCompletedLevels(1);
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
    }
}
