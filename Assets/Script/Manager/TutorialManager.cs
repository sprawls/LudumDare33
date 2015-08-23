using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class TutorialManager : MonoBehaviour {

    public static TutorialManager Instance;

    public CanvasGroup Canvas_Tuto;
    

    private bool tutoButtonPressed = false;
    private bool showTuto = false;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            GameObject.DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        Canvas_Tuto = GameObject.FindGameObjectWithTag("TutoCanvas").GetComponent<CanvasGroup>();
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
        showTuto = true;
        tutoButtonPressed = true;
    }

    public void ButtonClick_NO() {
        showTuto = false;
        tutoButtonPressed = true;
    }

    public void StartTutorial() {
        StartCoroutine(Convo_Start());
    }

    public void EndTutorial() {
        TalkManager.Instance.FadeOutText();
        GameManager.Instance.StartGame();
    }

    IEnumerator Convo_Start() {
        yield return new WaitForSeconds(3f);
        float waitTime = TalkManager.Instance.WriteMessage("Hello There. It's been a while. ");
        yield return new WaitForSeconds(waitTime + 2.5f);
        waitTime = TalkManager.Instance.WriteMessage("It's that time again.");
        yield return new WaitForSeconds(waitTime + 2.5f);
        waitTime = TalkManager.Instance.WriteMessage("Do you still remember how this works ?");
        yield return new WaitForSeconds(waitTime + 1f);
        yield return StartCoroutine(Input_Tuto());

        if (showTuto) {

        } else {
            
        }
        EndTutorial();
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
