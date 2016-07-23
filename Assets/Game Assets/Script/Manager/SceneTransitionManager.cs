using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;

public enum ScenesEnum { menu, levelSelect, main }

public class SceneTransitionManager : MonoBehaviour {



    public static SceneTransitionManager Instance;
    public Image TransitionUI;
    public bool InTransition { get; private set; }

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InTransition = false;
            TransitionUI.DOFade(0,0);
        } else {
            Destroy(gameObject);
        }
    }

    void OnDestroy() {
        if (Instance == this) {
            Instance = null;
        }
    }

    public void TransitionToAnotherScene(ScenesEnum scene, float inTime = 1f, float outTime = 1.5f, float midTime = 0.5f){
        if (InTransition) Debug.Log("Attempted To Transition while in another transition");
        else {
            StartCoroutine(SceneTransition(scene, inTime, outTime, midTime));
        }
    }

    IEnumerator SceneTransition(ScenesEnum scene, float inTime, float outTime, float midTime) {
        InTransition = true;
        TransitionUI.DOFade(1, inTime);
        yield return new WaitForSeconds(inTime);
        switch (scene) {
            case ScenesEnum.menu :
                SceneManager.LoadScene("Menu");
                break;
            case ScenesEnum.main :
                SceneManager.LoadScene("Main");
                break;
            case ScenesEnum.levelSelect :
                SceneManager.LoadScene("Level Select");
                break;
            default :
                Debug.LogError("No case for selected scene : " + scene.ToString());
                break;
        }
        yield return new WaitForSeconds(midTime);
        TransitionUI.DOFade(0, outTime);

        yield return new WaitForSeconds(outTime * 0.7f);
        InTransition = false;
    }
}
