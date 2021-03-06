﻿using UnityEngine;
using System.Collections;


public class LevelSelectOrb : MonoBehaviour {
    
    // statics
    private const string STARS_DISPLAY_PAR = "LevelSelect/LevelOrb_StarDisplay_Par";
    private const string STARS_DISPLAY_COMPLETED = "LevelSelect/LevelOrb_StarDisplay_Completed";
    private const string STARS_DISPLAY_UNLOCKED = "LevelSelect/LevelOrb_StarDisplay_Unlocked";
    private const string STARS_DISPLAY_STARS_REQUIRED = "LevelSelect/LevelOrb_StarDisplay_StarRequirement";


    // public
    public enum WorldsEnum { world_1, world_2, world_3, world_4, world_5, world_6 }
    public enum UnlockMethod { WorldCompletionPoints, InverseLevelCompleted, StarsAmount }

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

    [Header("Unlock Conditions")]
    public UnlockMethod unlockMethod;
    /// <summary> Amounts of points or stars required depending on the chosen Unlock Method </summary>
    public int pointsRequired;

    [Header("Other")]
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
        if (LevelManager.Instance.CanClickOnLevels()) {   
            switch (world) {
                case WorldsEnum.world_1:
                    LevelManager.Instance.StartLevel(1, level, showTutorial);
                    break;
                case WorldsEnum.world_2:
                    LevelManager.Instance.StartLevel(2, level, false);
                    break;
                case WorldsEnum.world_3:
                    LevelManager.Instance.StartLevel(3, level, false);
                    break;
                case WorldsEnum.world_4:
                    LevelManager.Instance.StartLevel(4, level, false);
                    break;
                case WorldsEnum.world_5:
                    LevelManager.Instance.StartLevel(5, level, false);
                    break;
                case WorldsEnum.world_6:
                    LevelManager.Instance.StartLevel(6, level, false);
                    break;
            }
        }
        
    }

    private bool GetOrbReverseWorld() {
        switch (world) {
            case WorldsEnum.world_1:
                return false;
            case WorldsEnum.world_2:
                return true;
            case WorldsEnum.world_3:
                return false;
            case WorldsEnum.world_4:
                return true;
            case WorldsEnum.world_5:
                return false;
            case WorldsEnum.world_6:
                return true;
            default :
                return false;
        }
    }

    public void UpdateOrb(){
       Level currentLevel = LevelManager.Instance.GetLevel(GetLevelId());
       bool isInInverseWorld = GetOrbReverseWorld();
       if (currentLevel != null) {

           if (CheckUnlockConditions()) {
               LevelManager.Instance.UnlockLevel(GetWorldByEnum(world), level);

               buttonCollider.enabled = true;
               if (LevelManager.Instance.GetLevel(GetLevelId()).completedPar) {
                   if (!isInInverseWorld) SpawnOrb(orbPrefab_completedPar, STARS_DISPLAY_PAR);
                   else SpawnOrb(orbPrefab_completedPar_inverse, STARS_DISPLAY_PAR);
               } else if (LevelManager.Instance.GetLevel(GetLevelId()).completed) {
                   if (!isInInverseWorld) SpawnOrb(orbPrefab_completed, STARS_DISPLAY_COMPLETED);
                   else SpawnOrb(orbPrefab_completed_inverse, STARS_DISPLAY_COMPLETED);
               } else {
                   if (!isInInverseWorld) SpawnOrb(orbPrefab_unlocked, STARS_DISPLAY_UNLOCKED);
                   else SpawnOrb(orbPrefab_unlocked_inverse, STARS_DISPLAY_UNLOCKED);
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

    bool CheckUnlockConditions() {
        switch (unlockMethod) {
            case UnlockMethod.WorldCompletionPoints :
                int completedLevels = GetAmtLvlCompleted();
                return completedLevels >= pointsRequired;
            case UnlockMethod.InverseLevelCompleted :
                return(CheckUnlockCondition_InverseWorldCompleted());
            case UnlockMethod.StarsAmount :
                int AmtStars = LevelManager.Instance.GetTotalAmountStars();
                bool unlocked = AmtStars >= pointsRequired;
                if (!unlocked) SpawnStarRequirementDisplay();
                return unlocked; 
            default :
                Debug.Log("Unimplemented Unlock Method !");
                return false;
        }
    }

    int GetWorldByEnum(WorldsEnum wEnum) {
        switch (wEnum) {
            case WorldsEnum.world_1:
                return 1;
            case WorldsEnum.world_2:
                return 2;
            case WorldsEnum.world_3:
                return 3;
            case WorldsEnum.world_4:
                return 4;
            case WorldsEnum.world_5:
                return 5;
            case WorldsEnum.world_6:
                return 6;
            default:
                Debug.Log("Using an unimplemented world !");
                return -1;
        }
    }

    bool CheckUnlockCondition_InverseWorldCompleted() {
        switch (world) {
            case WorldsEnum.world_1:
                return LevelManager.Instance.GetLevelIsCompleted(2, level);
            case WorldsEnum.world_2:
                return LevelManager.Instance.GetLevelIsCompleted(1, level);
            case WorldsEnum.world_3:
                return LevelManager.Instance.GetLevelIsCompleted(4, level);
            case WorldsEnum.world_4:
                return LevelManager.Instance.GetLevelIsCompleted(3, level);
            case WorldsEnum.world_5:
                return LevelManager.Instance.GetLevelIsCompleted(6, level);
            case WorldsEnum.world_6:
                return LevelManager.Instance.GetLevelIsCompleted(5, level);
            default :
                Debug.Log("Using an unimplemented world !");
                return false;
        }
    }

    void SetPositions(Vector3 pos1, Vector3 pos2) {
        //Update Line renderer positions
        nextLevelLineRenderer.SetPosition(0, pos1 );
        nextLevelLineRenderer.SetPosition(1, pos2 );
        //Update other Line renderer properties
        nextLevelLineRenderer.SetWidth(0.15f, 0.15f);

        //Update Properties based on inverse world
        Color beamColor;
        if(LevelManager.Instance.isInverseWorld()){
            beamColor = new Color(0.6f,0.80f,1f,0.5f);
        } else {
            beamColor = new Color(0.7f,0.80f,1f,0.5f);
        }
        if(!nextLevelOrb.unlocked) beamColor = new Color(beamColor.r,beamColor.g,beamColor.b, 0.1f);
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
            case WorldsEnum.world_3:
                levelsCompleted = LevelManager.Instance.GetWorldCompletedLevels(3);
                break;
            case WorldsEnum.world_4:
                levelsCompleted = LevelManager.Instance.GetWorldCompletedLevels(4);
                break;
            case WorldsEnum.world_5:
                levelsCompleted = LevelManager.Instance.GetWorldCompletedLevels(5);
                break;
            case WorldsEnum.world_6:
                levelsCompleted = LevelManager.Instance.GetWorldCompletedLevels(6);
                break;
        }    
        return levelsCompleted;
    }

    private string GetLevelId() {
        string id = world.ToString() + "_" + level.ToString();
        return id;
    }

    private void SpawnOrb(GameObject prefab, string starsPrefabLoaction = null) {
        if (spawnedOrbPrefab != null) {
            Destroy(spawnedOrbPrefab);
            spawnedOrbPrefab = null;
        } 
        spawnedOrbPrefab = (GameObject) Instantiate(prefab, transform.position, Quaternion.identity);
        spawnedOrbPrefab.transform.parent = transform;
        spawnedOrbPrefab.transform.localScale = Vector3.one;

        if (starsPrefabLoaction != null) {
            GameObject starsPrefab = (GameObject)Resources.Load(starsPrefabLoaction);
            if (starsPrefab != null) {
                GameObject starObjectInstance = (GameObject) GameObject.Instantiate(starsPrefab, spawnedOrbPrefab.transform.position, Quaternion.identity);
                starObjectInstance.transform.parent = spawnedOrbPrefab.transform;
            }
        }
    }

    private void SpawnStarRequirementDisplay() {
        GameObject instantiatedDisplay = (GameObject) Instantiate(Resources.Load(STARS_DISPLAY_STARS_REQUIRED), transform.position, Quaternion.identity);
        if (instantiatedDisplay != null) {
            TextMesh textmesh = instantiatedDisplay.GetComponentInChildren<TextMesh>();
            if (textmesh != null) {
                textmesh.text = pointsRequired.ToString();
            }
        }
    }

    //GIZMO DEBUG
    void OnDrawGizmos() {
        if (Application.isPlaying) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.5f * transform.localScale.x);

    }
}
