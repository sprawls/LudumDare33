using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class GameManager : MonoBehaviour {

    public static GameManager Instance {  get; private set; }

    public List<GameObject> LevelsList;
    public int currentLevel = 0;
    public int currentLevelMoves {get; private set;}
    private int[] parMoves = new int[9] {2, 2, 6, 1, 2, 7, 6, 6,8};
    [HideInInspector] public GameObject currentLevel_Obj;
    public GameObject EndLevelExplosion;

    public int Score {get; private set;}

    void Awake() {
        if (Instance == null) {
            Instance = this;
            GameObject.DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        currentLevelMoves = 0;
        Score = 0;
        LoadLevel();

    }

    void OnDestroy() {
        if (Instance == this) {
            Instance = null;
        }
    }

    public int GetParMoves() {
        if (parMoves.Length > currentLevel) {
            return parMoves[currentLevel];
        } else {
            Debug.Log("No Par Moves with current level");
            return 5;
        }
    }

    public void AddMove() {
        currentLevelMoves++;
        if (currentLevelMoves > parMoves[currentLevel]) Score++;
    }

    public void CompleteLevel() {
        
        StartCoroutine(CompletionAnimation());
        currentLevelMoves = 0;
    }

    public void LoadLevel() {
        

        if (currentLevel_Obj != null) Destroy(currentLevel_Obj);
        if (LevelsList.Count > currentLevel) currentLevel_Obj = (GameObject)Instantiate(LevelsList[currentLevel], transform.position, Quaternion.identity);
        else Debug.Log("All LEvels COMPLETED !");
    }

    private void DeactivateAllColliders(){
        MeshCollider[] mcs = currentLevel_Obj.GetComponentsInChildren<MeshCollider>();
        foreach (MeshCollider mc in mcs) {
            mc.enabled = false;
        }
    }
    private void ScaleOrbs(float scale) {
        ElementSocket[] sockets = currentLevel_Obj.GetComponentsInChildren<ElementSocket>();
        foreach (ElementSocket s in sockets) {
            s.element.Model.transform.DOScale(s.element.Model.transform.localScale.x * scale, 0.5f);
        }
    }

    IEnumerator CompletionAnimation() {
        yield return new WaitForSeconds(1f);
        ScaleOrbs(1.5f);
        DeactivateAllColliders();
        MusicManager.Instance.PlaySound_LevelComplete();
        yield return new WaitForSeconds(0.5f);
        
        ScaleOrbs(0.7f);

        Instantiate(EndLevelExplosion, transform.position + new Vector3(0,0,-10), Quaternion.identity);
        yield return new WaitForSeconds(1f);

        currentLevel++;
        ChangeColorByLevel.UpdateAllColor();
        LoadLevel();
    }
}
