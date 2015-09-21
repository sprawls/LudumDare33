using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
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
    public int currentLevel = 0;
    public int currentLevelMoves {get; private set;}
    private int[] parMoves = new int[11] {2, 2, 6, 1, 2, 7, 6, 6,8, 11, 2};
    [HideInInspector] public GameObject currentLevel_Obj;
    public GameObject EndLevelExplosion;

    public int Score {get; private set;}
    public const int LastLevel = 11;

    void Awake() {
        if (Instance == null) {
            Instance = this;
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
            StartGame(LevelManager.Instance.currentSelectedLevel);
        }
    }

    public void StartGame() {
        StartGame(0);
    }

    public void StartGame(int lvl) {
        currentLevel = lvl;
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

    public void CompleteLevel_Tuto() {
        StartCoroutine(CompletionAnimation_Tuto());
    }

    public void AddMove() {
        currentLevelMoves++;
        if (currentLevelMoves > parMoves[currentLevel]) Score++;
    }

    public void CompleteLevel() {
        if (TutorialManager.Instance.isInTuto) {
            TutorialManager.Instance.TutoLevelCompleted();
        } else {
            //TODO : Update Par Moves
            if (currentLevelMoves < GetParMoves()) Score -= (GetParMoves() - currentLevelMoves);

            Debug.Log("last lvl :" + LastLevel + " Cur + 1 : " + (currentLevel + 1));
            if (currentLevel + 1 == LastLevel) {
                StartCoroutine(EndSequence_HeatDeath());
            } else {
                StartCoroutine(CompletionAnimation());
                
            }
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
        if (currentLevel_Obj != null) Destroy(currentLevel_Obj);
        if (LevelsList_w1.Count > currentLevel) currentLevel_Obj = (GameObject)Instantiate(LevelsList_w1[currentLevel], transform.position, Quaternion.identity);
        else Debug.Log("LastLevelReached");
    }

    public void OnClick_Restart() {
        if (currentLevel < 0 || currentLevel + 1 >= LastLevel) {

        } else {
            ReloadLevel();
        }
    }

    public void OnClick_BackToMenu() {
        Application.LoadLevel("Menu");
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
        yield return new WaitForSeconds(1f);
        ScaleOrbs(1.5f);
        DeactivateAllColliders();
        MusicManager.Instance.PlaySound_LevelComplete();
        yield return new WaitForSeconds(0.5f);
        
        ScaleOrbs(0.7f);

        Instantiate(EndLevelExplosion, transform.position + new Vector3(0,0,-10), Quaternion.identity);
        yield return new WaitForSeconds(1f);

        LevelManager.Instance.CompleteLevel(currentLevelMoves, parMoves[currentLevel]);
        currentLevel = LevelManager.Instance.currentSelectedLevel;
        currentLevelMoves = 0;

        ChangeColorByLevel.UpdateAllColor();
        LoadLevel();

    }

    IEnumerator CompletionAnimation_Tuto() {
        TutorialManager.Instance.ScaleTutoOrbs(1.5f);

        MusicManager.Instance.PlaySound_LevelComplete();
        yield return new WaitForSeconds(0.5f);

        TutorialManager.Instance.ScaleTutoOrbs(0.7f);

        Instantiate(EndLevelExplosion, transform.position + new Vector3(0, 0, -10), Quaternion.identity);
        yield return new WaitForSeconds(1f);
    }


    IEnumerator EndSequence_HeatDeath() {
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
