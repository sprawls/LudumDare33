using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;

public class MenuManager : MonoBehaviour {

    public Image frontImage;
    public GameObject mainCanvas;
    public GameObject[] TransitionDestroy;

    private bool isfadin = false;

    public void OnClick_GoNextScene() {
        if (isfadin == false && SceneTransitionManager.Instance.InTransition == false) {
            isfadin = true;
            StartCoroutine(StartGame());
        }
    }

    public void OnClick_OpenTwitter() {
        if(SceneManager.GetActiveScene().name == "Main")
            Application.OpenURL("https://twitter.com/Alexis_Lessard");
    }

    IEnumerator StartGame() {
        float fadeIn = 0.8f;
        float transitionTime = 0.5f;

        SceneTransitionManager.Instance.TransitionToAnotherScene(ScenesEnum.levelSelect, fadeIn, 1f, transitionTime);
        GameObject.DontDestroyOnLoad(gameObject);
        yield return new WaitForSeconds(fadeIn);

        Destroy(mainCanvas);
        yield return new WaitForSeconds(transitionTime);

        foreach (GameObject go in TransitionDestroy) {
            Destroy(go);
        }
        Destroy(gameObject);
    }
}
