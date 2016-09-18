using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : MonoBehaviour {

    [Header("Debug")]
    public bool DontSpawnOnPlay = false;

    public static GameManager Instance {  get; private set; }

    [Header("Properties")]
    public CanvasGroup UICanvas;
    public CanvasGroup UICanvas_Ending;
    public CanvasGroup UICanvas_Ending_Button;
    public CanvasGroup UICanvas_ParAchieved;
    public CanvasGroup UICanvas_HighscoreAchieved;
    public MeshRenderer backgroundRenderer;
    public Button RestartButton;

    public List<GameObject> LevelsList_w1;
    public List<GameObject> LevelsList_w2;
    public List<GameObject> LevelsList_w3;
    public List<GameObject> LevelsList_w4;
    public List<GameObject> LevelsList_w5;
    public List<GameObject> LevelsList_w6;
    public List<List<GameObject>> worldList = new List<List<GameObject>>();
    public int currentLevel = 0;
    public int currentWorld = 0;
    public int currentLevelMoves {get; private set;}
    [HideInInspector] public GameObject currentLevel_Obj;
    public GameObject EndLevelExplosion;

    public int Score {get; private set;}
    public bool InAnimation { get; private set; }

    private void SetCurrentLevel(int newLevel) {
        currentLevel = newLevel;
        UIGetter_Level.UpdateAll();
    }

    private void SetCurrentLevelMoves(int newLevelMoves) {
        currentLevelMoves = newLevelMoves;
        UIGetter_Moves.UpdateAll();
    }

    void Awake() {
        if (Instance == null) {
            Instance = this;
            worldList.Add(LevelsList_w1);
            worldList.Add(LevelsList_w2);
            worldList.Add(LevelsList_w3);
            worldList.Add(LevelsList_w4);
            worldList.Add(LevelsList_w5);
            worldList.Add(LevelsList_w6);
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        RestartButton.interactable = false;
        InAnimation = false;

        if (LevelManager.Instance.showTutorial == true && LevelManager.Instance.levelsData.tutorialCompleted == false) {
            UICanvas_Ending.DOFade(0, 0.01f);
            TutorialManager.Instance.StartTutorial();
            
        } else {
            if (!DontSpawnOnPlay) {
                TutorialManager.Instance.ButtonClick_EndTuto();
                StartGame(LevelManager.Instance.currentSelectedLevel, LevelManager.Instance.currentSelectedWorld);
            }
        }

        UIGetter_Level.UpdateAll();
        UIGetter_Moves.UpdateAll();
    }

    public void StartGame() {
        StartGame(0);
    }

    public void StartGame(int lvl, int world = 1) {
        SetCurrentLevel(lvl);
        currentWorld = world;
        SetCurrentLevelMoves(0);
        Score = 0;
        LoadLevel();

    }

    void OnDestroy() {
        if (Instance == this) {
            Instance = null;
        }
    }

    public int GetParMoves() {
        return (LevelManager.Instance.GetLevelPar(currentWorld, currentLevel));
    }

    public void CompleteLevel_Tuto() {
        StartCoroutine(CompletionAnimation_Tuto());
    }

    public void AddMove() {
        SetCurrentLevelMoves(currentLevelMoves+1);
        if (currentLevelMoves > GetParMoves()) Score++;
    }

    public void CompleteLevel() {
        if (TutorialManager.Instance.isInTuto) {
            TutorialManager.Instance.TutoLevelCompleted();
        } else {
            Level curLevelRef = LevelManager.Instance.GetCurrentLevel();
            Debug.Log(GetParMoves() + " " + currentLevelMoves);
            if (currentLevelMoves <= GetParMoves()) {
                StartCoroutine(FadeEndLevelCanvas(UICanvas_ParAchieved));
            } else if (!curLevelRef.BestMoveScoreIsUnset() && currentLevelMoves < curLevelRef.bestMoveScore) {
                StartCoroutine(FadeEndLevelCanvas(UICanvas_HighscoreAchieved));
            }
            StartCoroutine(CompletionAnimation());
        }
    }

    public void StartEnding_NoHeatDeath() {

    }

    public void LoadLevel() {
        ReloadLevel(false);
        DialogueManager.Instance.StopTalkCoroutines();
        DialogueManager.Instance.TalkAboutLevel();

        UIGetter_ParMoves.UpdateAll();

    }

    public void ReloadLevel(bool incrementStat = false) {
        //Debug.Log(currentLevel + "  " + currentWorld);
        if (currentLevel_Obj != null) Destroy(currentLevel_Obj);
        switch (currentWorld) {
            case 1 :
                if (LevelsList_w1.Count > currentLevel) currentLevel_Obj = (GameObject)Instantiate(LevelsList_w1[currentLevel], transform.position, Quaternion.identity);
                else Debug.Log("LastLevelReached"); //TODO Go back to level select ?
                break;
            case 2  :
                if (LevelsList_w2.Count > currentLevel) currentLevel_Obj = (GameObject)Instantiate(LevelsList_w2[currentLevel], transform.position, Quaternion.identity);
                else Debug.Log("LastLevelReached"); //TODO Go back to level select ?
                break;
            case 3:
                if (LevelsList_w3.Count > currentLevel) currentLevel_Obj = (GameObject)Instantiate(LevelsList_w3[currentLevel], transform.position, Quaternion.identity);
                else Debug.Log("LastLevelReached"); //TODO Go back to level select ?
                break;
            case 4:
                if (LevelsList_w4.Count > currentLevel) currentLevel_Obj = (GameObject)Instantiate(LevelsList_w4[currentLevel], transform.position, Quaternion.identity);
                else Debug.Log("LastLevelReached"); //TODO Go back to level select ?
                break;
            default :
                Debug.LogError("BadWorldRequested");
                break;
        }

        SetCurrentLevelMoves(0);

        if(incrementStat) LevelManager.Instance.IncrementResets();
    }

    public void OnClick_Restart() {
        if (InAnimation) return;

        if (currentLevel < 0 || currentLevel + 1 >= worldList[currentWorld-1].Count) {

        } else {
            ReloadLevel(true);
        }
    }

    public void OnClick_BackToMenu() {
        if (InAnimation) return;

        SceneTransitionManager.Instance.TransitionToAnotherScene(ScenesEnum.menu);
    }

    public void OnClick_BackToLevelSelect() {
        if (InAnimation) return;

        SceneTransitionManager.Instance.TransitionToAnotherScene(ScenesEnum.levelSelect);
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
            if (s.element.Model != null) s.element.Model.transform.DOScale(s.element.Model.transform.localScale.x * scale, 0.5f);
        }
    }

    IEnumerator CompletionAnimation() {
        InAnimation = true;
        yield return new WaitForSeconds(1f);
        ScaleOrbs(1.5f);
        DeactivateAllColliders();
        MusicManager.Instance.PlaySound_LevelComplete();
        yield return new WaitForSeconds(0.5f);
        
        ScaleOrbs(0.7f);

        Instantiate(EndLevelExplosion, transform.position + new Vector3(0,0,-10), Quaternion.identity);
        yield return new WaitForSeconds(1f);

        LevelManager.Instance.CompleteLevel(currentLevelMoves, GetParMoves());
        currentLevel = LevelManager.Instance.currentSelectedLevel;
        SetCurrentLevelMoves(0);

        if (LevelManager.Instance.CurrentLevelExists()) {
            //Show Next LEvel
            ChangeColorByLevel.UpdateAllColor();
            LoadLevel();
            InAnimation = false;
        } else {
            yield return new WaitForSeconds(0.5f);
            SceneTransitionManager.Instance.TransitionToAnotherScene(ScenesEnum.levelSelect,2f,2f,0.25f);
        }
    }

    IEnumerator FadeEndLevelCanvas(CanvasGroup canvasToFade) {
        yield return new WaitForSeconds(1.0f);
        canvasToFade.DOFade(1.0f, 1.5f).SetEase(Ease.OutCubic);
        yield return new WaitForSeconds(2f);
        canvasToFade.DOFade(0f, 2.5f).SetEase(Ease.InCubic); ;
    }

    IEnumerator CompletionAnimation_Tuto() {
        InAnimation = true;

        TutorialManager.Instance.ScaleTutoOrbs(1.5f);

        MusicManager.Instance.PlaySound_LevelComplete();
        yield return new WaitForSeconds(0.5f);

        TutorialManager.Instance.ScaleTutoOrbs(0.7f);

        Instantiate(EndLevelExplosion, transform.position + new Vector3(0, 0, -10), Quaternion.identity);
        yield return new WaitForSeconds(1f);

        InAnimation = false;
    }


    IEnumerator EndSequence_HeatDeath() {
        InAnimation = true; 

        ElementTri.ToogleAllTrisActivation(false);
        DialogueManager.Instance.StopTalkCoroutines();
        float messageCooldown = TalkManager.Instance.WriteMessage("Here it is. The Heat Death.");
        yield return new WaitForSeconds(messageCooldown + 5.5f);
        messageCooldown = TalkManager.Instance.WriteMessage("It's time to go now.");
        yield return new WaitForSeconds(messageCooldown + 2.5f);
        UICanvas.DOFade(0, 1.5f);
        StartCoroutine(CompletionAnimation());
        MusicManager.Instance.StopAllSounds();
        backgroundRenderer.GetComponent<ChangeColorByLevel>().enabled = false;
        yield return new WaitForSeconds(3f);
        backgroundRenderer.material.DOColor(new Color(1, 1, 1, 1), 5f);
        OtherBeing ob = GameObject.FindGameObjectWithTag("OtherBeing").GetComponent<OtherBeing>();
        ob.Kill();
        yield return new WaitForSeconds(5f);
        RestartButton.interactable = true;
        UICanvas_Ending.DOFade(1, 8f);
        yield return new WaitForSeconds(3f);
        UICanvas_Ending_Button.DOFade(1, 5f);

        InAnimation = false;
    }

    IEnumerator EndSequence_NoHeatDeath() {
        float messageCooldown = TalkManager.Instance.WriteMessage("What are you doing ? This is the opposite of what you have to do.");
        yield return new WaitForSeconds(messageCooldown + 3.5f);
        messageCooldown = TalkManager.Instance.WriteMessage("Come on, balance this cluster.");
        yield return new WaitForSeconds(messageCooldown + 2.5f);
        messageCooldown = TalkManager.Instance.WriteMessage("Come on, balance this cluster.");
        yield return new WaitForSeconds(messageCooldown + 2.5f);
        
    }
}
