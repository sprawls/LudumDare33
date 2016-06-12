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
        if (isfadin == false) {
            isfadin = true;
            StartCoroutine(StartGame());
        }
    }

    public void OnClick_OpenTwitter() {
        if(SceneManager.GetActiveScene().name == "Main")
            Application.OpenURL("https://twitter.com/Alexis_Lessard");
    }

    IEnumerator StartGame() {
        frontImage.DOFade(1, 1.5f);
        GameObject.DontDestroyOnLoad(frontImage.gameObject);
        GameObject.DontDestroyOnLoad(gameObject);
        frontImage.gameObject.AddComponent<DestroyOnTimer>().time = 5f;
        yield return new WaitForSeconds(1.5f);

        Destroy(mainCanvas);
        yield return new WaitForSeconds(1.5f);

        foreach (GameObject go in TransitionDestroy) {
            Destroy(go);
        }
        frontImage.DOFade(0, 4f);
        Application.LoadLevel(1);

        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }
}
