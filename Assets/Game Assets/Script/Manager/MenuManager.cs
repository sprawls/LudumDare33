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
        SceneTransitionManager.Instance.TransitionToAnotherScene(ScenesEnum.levelSelect, 1.5f, 3.5f, 1f);
        GameObject.DontDestroyOnLoad(gameObject);
        yield return new WaitForSeconds(1.5f);

        Destroy(mainCanvas);
        yield return new WaitForSeconds(1f);

        foreach (GameObject go in TransitionDestroy) {
            Destroy(go);
        }
        Destroy(gameObject);
    }
}
