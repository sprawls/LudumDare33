using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class GameManager : MonoBehaviour {

    public static GameManager Instance {  get; private set; }
    public static bool tutoShown = false;

    public List<GameObject> LevelsList;
    public int currentLevel = 0;
    public int currentLevelMoves {get; private set;}
    private int[] parMoves = new int[10] {2, 2, 6, 1, 2, 7, 6, 6,8, 11};
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
        if (tutoShown == false) {
            TutorialManager.Instance.StartTutorial();
        }
    }

    public void StartGame() {
        tutoShown = true;
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
            if (currentLevelMoves < GetParMoves()) Score -= (GetParMoves() - currentLevelMoves);
            StartCoroutine(CompletionAnimation());
            currentLevelMoves = 0;
        }
    }

    public void LoadLevel() {
        
        if (currentLevel_Obj != null) Destroy(currentLevel_Obj);
        if (LevelsList.Count > currentLevel) currentLevel_Obj = (GameObject)Instantiate(LevelsList[currentLevel], transform.position, Quaternion.identity);
        else Debug.Log("All Levels COMPLETED !");

        StopCoroutine("TalkAboutLevel");
        StartCoroutine("TalkAboutLevel");
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

        currentLevel++;
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
                messageCooldown = TalkManager.Instance.WriteMessage("No need to feel bad. Heat death is the only solution.");
                break;
            case 9:
                messageCooldown = TalkManager.Instance.WriteMessage("That's a big one.");
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
                RandomMessage = "Equilibrium will happen regardless. We're just accelerating the process.";
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
}
