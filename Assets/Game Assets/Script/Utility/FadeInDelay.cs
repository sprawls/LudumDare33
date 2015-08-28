using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class FadeInDelay : MonoBehaviour {

    public float delay;
    private Text text;

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
        StartCoroutine(FadeIn());
	}

    IEnumerator FadeIn() {
        yield return new WaitForSeconds(delay);
        text.DOFade(1, 6f);
    }
}
