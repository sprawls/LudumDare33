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
        StopCoroutine("TalkAboutLevel");
        StartCoroutine("TalkAboutLevel");
       
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

    IEnumerator TalkAboutLevel() {
        yield return new WaitForSeconds(2.5f);

        float messageCooldown = 0f;
        switch (currentLevel) {
            case 1 :
                messageCooldown = TalkManager.Instance.WriteMessage("Equilibrium is necessary.");
                break;
            case 2 :
                messageCooldown = TalkManager.Instance.WriteMessage("Simply put, clearing a cluster removes all its free energy.");
                break;
            case 4:
                messageCooldown = TalkManager.Instance.WriteMessage("Sometimes energy can fuse. The result from the fuse is the element you must consider.");
                yield return new WaitForSeconds(messageCooldown + 3.5f);
                messageCooldown = TalkManager.Instance.WriteMessage("Two identical orbs will produce the same element while two different will produce a new one.");
                break;
            case 5:
                messageCooldown = TalkManager.Instance.WriteMessage("It gets harder from here on out.");
                break;
            case 6:
                messageCooldown = TalkManager.Instance.WriteMessage("We're getting closer. I feel it.");
                break;
            case 7:
                messageCooldown = TalkManager.Instance.WriteMessage("Once this is over, you'll have to go back there.");
                break;
            case 8:
                messageCooldown = TalkManager.Instance.WriteMessage("No need to feel bad. We'll survive.");
                break;
            case 9:
                messageCooldown = TalkManager.Instance.WriteMessage("That's a big one.");
                break;
            case LastLevel-1:
                messageCooldown = TalkManager.Instance.WriteMessage("This is the last one. Once this cluster reaches equilibrium, so will the universe. ");
                yield return new WaitForSeconds(messageCooldown + 3.5f);
                messageCooldown = TalkManager.Instance.WriteMessage("Everything will stop. Maximum Entropy. ");
                yield return new WaitForSeconds(messageCooldown + 12f);
                messageCooldown = TalkManager.Instance.WriteMessage("Come on, you can do it.");
                yield return new WaitForSeconds(messageCooldown + 8f);
                messageCooldown = TalkManager.Instance.WriteMessage("You never had any doubt before. ");
                yield return new WaitForSeconds(messageCooldown + 12f);
                messageCooldown = TalkManager.Instance.WriteMessage("We are waiting for you.");
                yield return new WaitForSeconds(messageCooldown + 18f);
                messageCooldown = TalkManager.Instance.WriteMessage("Come on, This is the last one.");
                yield return new WaitForSeconds(messageCooldown + 40f);
                messageCooldown = TalkManager.Instance.WriteMessage("I Mean it, it really is... ");
                yield return new WaitForSeconds(messageCooldown + 100f);
                messageCooldown = TalkManager.Instance.WriteMessage("Still there ? Really ?");
                break;
        }
        messageCooldown += 15f;

        yield return new WaitForSeconds(messageCooldown);

        

        WriteRandomTip();
    }

    private void WriteRandomTip() {
        string RandomMessage= "";
        switch(Random.Range(0,8)){
            case 0 :
                RandomMessage = "Take your time, we're in no hurry.";
                break;
            case 1 :
                RandomMessage = "Stability will happen regardless. We're just accelerating the process.";
                break;
            case 2:
                RandomMessage = "Still rusty, huh.";
                break;
            case 3:
                RandomMessage = "You can always reset the cluster to its origin position by using the button at the top right corner.";
                break;
            case 4:
                RandomMessage = "The optimal amount of moves to reach equilibrium in a cluster is written in the top left corner.";
                break;
            case 5:
                RandomMessage = "Your score is the amount of moves above the par solution used. The lower the better. ";
                break;
            case 6:
                RandomMessage = "Keep going, we haven't woken you up for nothing.";
                break;
            case 7:
                RandomMessage = "What's wrong ? You've done that plenty of times.";
                break;
        }

        TalkManager.Instance.WriteMessage(RandomMessage);
    }

    IEnumerator EndSequence_HeatDeath() {
        ElementTri.ToogleAllTrisActivation(false);
        StopCoroutine("TalkAboutLevel");
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
