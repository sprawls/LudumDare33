﻿using UnityEngine;
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
    public LevelSelectOrb nextLevelOrb = null;

    private GameObject spawnedOrbPrefab;
    private LineRenderer nextLevelLineRenderer;
    public bool unlocked { get; private set; }

    void Awake() {
        nextLevelLineRenderer = GetComponent<LineRenderer>();
        UpdateOrb();
    }

	void Start () {    
        if (nextLevelOrb != null) {
            nextLevelLineRenderer.enabled = true;
            SetPositions(transform.position, nextLevelOrb.transform.position);
        } else {
            nextLevelLineRenderer.enabled = false;
        }
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
       bool isInInverseWorld = (LevelManager.Instance.isInverseWorld());
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
               unlocked = true;
           } else {
               if (!isInInverseWorld) SpawnOrb(orbPrefab_locked);
               else SpawnOrb(orbPrefab_locked_inverse);
               buttonCollider.enabled = false;
               unlocked = false;
           }
       } else {
           Debug.Log("Requested Level Does Not Exist !");
           Debug.Log(GetLevelId());
       }
    }

    void SetPositions(Vector3 pos1, Vector3 pos2) {
        //Update Line renderer positions
        nextLevelLineRenderer.SetPosition(0, pos1);
        nextLevelLineRenderer.SetPosition(1, pos2);
        //Update other Line renderer properties
        nextLevelLineRenderer.SetWidth(0.15f, 0.15f);

        //Update Properties based on inverse world
        Color beamColor;
        if(LevelManager.Instance.isInverseWorld()){
            beamColor = new Color(0.6f,0.80f,1f,0.5f);
        } else {
            beamColor = new Color(0.7f,0.80f,1f,0.5f);
        }
        if(!nextLevelOrb.unlocked) beamColor = new Color(beamColor.r,beamColor.g,beamColor.b, 0.2f);
        nextLevelLineRenderer.SetColors(beamColor, beamColor);

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
