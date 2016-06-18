using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class TutorialManager : MonoBehaviour {

    public static TutorialManager Instance { get; private set; }

    public GameObject TutoLevel_01;

    
    public bool isInTuto { get; private set; }
    public bool tutoLevelComplete = false;

    private CanvasGroup Canvas_Tuto;
    private Button Skip_Tuto;
    private GameObject TutoLevel_ref;
    private bool tutoButtonPressed = false;
    private bool showTuto = false;


    void Awake() {
        if (Instance == null) {
            Instance = this;
            Canvas_Tuto = GameObject.FindGameObjectWithTag("TutoCanvas").GetComponent<CanvasGroup>();
            Skip_Tuto = GameObject.FindGameObjectWithTag("TutoSkip").GetComponent<Button>();
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {

        isInTuto = false;
        ToggleButtons(false);
    }

    void OnDestroy() {
        if (Instance == this) {
            Instance = null;
        }
    }

    private void ToggleButtons(bool on) {
        Button[] bs = Canvas_Tuto.GetComponentsInChildren<Button>();
        foreach (Button b in bs) {
            b.interactable = on;
        }
    }

    public void ButtonClick_YES() {
        showTuto = false;
        tutoButtonPressed = true;
    }

    public void ButtonClick_NO() {
        showTuto = true;
        tutoButtonPressed = true;
    }

    public void ButtonClick_EndTuto() {
        StartCoroutine(EndTutorial());
    }

    public void StartTutorial() {
        StartCoroutine(Convo_Start());
    }

    public void TutoLevelCompleted() {
        tutoLevelComplete = true;
        TogglePlayerInputToTutoLevel(false);
    }

    public void ScaleTutoOrbs(float scale) {
        ElementSocket[] sockets = TutoLevel_ref.GetComponentsInChildren<ElementSocket>();
        foreach (ElementSocket s in sockets) {
            if (s.element.Model != null) s.element.Model.transform.DOScale(s.element.Model.transform.localScale.x * scale, 0.5f);
        }
    }

    public IEnumerator EndTutorial() {
        Destroy(Canvas_Tuto.transform.parent.gameObject);
        if (TutoLevel_ref != null) {
            GameManager.Instance.CompleteLevel_Tuto();
            yield return new WaitForSeconds(0.6f);
            Destroy(TutoLevel_ref);
            TutoLevel_ref = null;
        }
        if (Skip_Tuto != null) {
            Destroy(Skip_Tuto.gameObject);
        }

        isInTuto = false;
        GameManager.Instance.StartGame();
        yield return null;
        StopAllCoroutines();
        

    }

    private void TogglePlayerInputToTutoLevel(bool active) {
        GameObject[] tris = GameObject.FindGameObjectsWithTag("TutoTris");
        foreach (GameObject go in tris) {
            go.GetComponentInChildren<ElementTri>().canReceivePlayerInput = active;
        }
    }

    IEnumerator Convo_Start() {
        yield return new WaitForSeconds(3f);
        float waitTime = TalkManager.Instance.WriteMessage("Hello. It has been a while.");
        yield return new WaitForSeconds(waitTime + 2.5f);
        waitTime = TalkManager.Instance.WriteMessage("It's time again.");
        yield return new WaitForSeconds(waitTime + 2.5f);
        waitTime = TalkManager.Instance.WriteMessage("Do you still remember how this works ?");
        yield return new WaitForSeconds(waitTime + 1f);
        yield return StartCoroutine(Input_Tuto());
        

        if (showTuto) {
            isInTuto = true;
            yield return StartCoroutine(TutorialLevel());
        } else {
            waitTime = TalkManager.Instance.WriteMessage("Of course you do. Let's start.");

        }

        StartCoroutine(EndTutorial());
    }

    IEnumerator TutorialLevel() {
        TutoLevel_ref = (GameObject)Instantiate(TutoLevel_01, transform.position, Quaternion.identity);

        float waitTime = TalkManager.Instance.WriteMessage("Here are two clusters. The left one is already balanced");
        yield return new WaitForSeconds(waitTime + 1.5f);
        waitTime = TalkManager.Instance.WriteMessage("To balance a cluster, all of its elements must be properly distributed.");
        yield return new WaitForSeconds(waitTime + 2.5f);
        waitTime = TalkManager.Instance.WriteMessage("Each Tri must have a different element on each of its corners.");
        yield return new WaitForSeconds(waitTime + 4f);
  
        TogglePlayerInputToTutoLevel(true);

        waitTime = TalkManager.Instance.WriteMessage("You can rotate a Tri by clicking on it. Try it on the right Cluster.");
        yield return new WaitForSeconds(waitTime + 2.5f);
        waitTime = TalkManager.Instance.WriteMessage("Try and fix the Cluster on the right to fit the left one.");
        yield return new WaitForSeconds(waitTime + 1.5f);

        while (tutoLevelComplete == false) {
            yield return null;
        }

        waitTime = TalkManager.Instance.WriteMessage("Great. You'll learn the rest along the way. Let's Start.");
        yield return new WaitForSeconds(waitTime + 3.5f);
        
    }

    IEnumerator Input_Tuto() {
        ToggleButtons(true);
        Canvas_Tuto.DOFade(1, 1.5f);
        tutoButtonPressed = false;

        while(tutoButtonPressed == false){
            yield return null;
        }

        ToggleButtons(false);
        Canvas_Tuto.DOFade(0, 1.5f);
    }
}
