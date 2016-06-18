using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : MonoBehaviour {

    public static GameManager Instance {  get; private set; }

    public CanvasGroup UICanvas;
    public CanvasGroup UICanvas_Ending;
    public CanvasGroup UICanvas_Ending_Button;
    public MeshRenderer backgroundRenderer;
    public Button RestartButton;

    public List<GameObject> LevelsList_w1;
    public List<GameObject> LevelsList_w2;
    public List<List<GameObject>> worldList = new List<List<GameObject>>();
    public int currentLevel = 0;
    public int currentWorld = 0;
    public int currentLevelMoves {get; private set;}
    [HideInInspector] public GameObject currentLevel_Obj;
    public GameObject EndLevelExplosion;

    public int Score {get; private set;}

    private bool _inAnimation = false;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            worldList.Add(LevelsList_w1);
            worldList.Add(LevelsList_w2);
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        RestartButton.interactable = false;
        if (LevelManager.Instance.showTutorial == true) {
            UICanvas_Ending.DOFade(0, 0.01f);
            TutorialManager.Instance.StartTutorial();
            
        } else {
            TutorialManager.Instance.ButtonClick_EndTuto();
            StartGame(LevelManager.Instance.currentSelectedLevel, LevelManager.Instance.currentSelectedWorld);
        }
    }

    public void StartGame() {
        StartGame(0);
    }

    public void StartGame(int lvl, int world = 1) {
        currentLevel = lvl;
        currentWorld = world;
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
        return (LevelManager.Instance.GetLevelPar(currentWorld, currentLevel));
    }

    public void CompleteLevel_Tuto() {
        StartCoroutine(CompletionAnimation_Tuto());
    }

    public void AddMove() {
        currentLevelMoves++;
        if (currentLevelMoves > GetParMoves()) Score++;
    }

    public void CompleteLevel() {
        if (TutorialManager.Instance.isInTuto) {
            TutorialManager.Instance.TutoLevelCompleted();
        } else {
            //TODO : Update Par Moves
            if (currentLevelMoves < GetParMoves()) Score -= (GetParMoves() - currentLevelMoves);

            StartCoroutine(CompletionAnimation());
        }
    }

    public void StartEnding_NoHeatDeath() {

    }

    public void LoadLevel() {

        ReloadLevel();
        DialogueManager.Instance.StopTalkCoroutines();
        DialogueManager.Instance.TalkAboutLevel(currentLevel);

       
    }

    public void ReloadLevel() {
        Debug.Log(currentLevel + "  " + currentWorld);
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
            default :
                Debug.LogError("BadWorldRequested");
                break;
        }

        currentLevelMoves = 0;
        
    }

    public void OnClick_Restart() {
        if (_inAnimation) return;

        if (currentLevel < 0 || currentLevel + 1 >= worldList[currentWorld-1].Count) {

        } else {
            ReloadLevel();
        }
    }

    public void OnClick_BackToMenu() {
        if (_inAnimation) return;

        SceneManager.LoadScene("Menu");
    }

    public void OnClick_BackToLevelSelect() {
        if (_inAnimation) return;

        SceneManager.LoadScene("Level Select");
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
        _inAnimation = true;
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
        currentLevelMoves = 0;

        ChangeColorByLevel.UpdateAllColor();
        LoadLevel();

        _inAnimation = false;
    }

    IEnumerator CompletionAnimation_Tuto() {
        _inAnimation = true;

        TutorialManager.Instance.ScaleTutoOrbs(1.5f);

        MusicManager.Instance.PlaySound_LevelComplete();
        yield return new WaitForSeconds(0.5f);

        TutorialManager.Instance.ScaleTutoOrbs(0.7f);

        Instantiate(EndLevelExplosion, transform.position + new Vector3(0, 0, -10), Quaternion.identity);
        yield return new WaitForSeconds(1f);

        _inAnimation = false;
    }


    IEnumerator EndSequence_HeatDeath() {
        _inAnimation = true; 

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

        _inAnimation = false;
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
