using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BestFitWithoutCutting : MonoBehaviour {

    private Text text;
    private LinkTextSizes linkedText;

    void Awake() {
        text = GetComponent<Text>();
        linkedText = GetComponent<LinkTextSizes>();
    }
    
    void Start() {
        StartCoroutine(FixHeight());
    }

    IEnumerator FixHeight() {
        //Debug.Log("Rezized");
        yield return null;
        TextGenerator gen = text.cachedTextGenerator;
        Rect rect = text.rectTransform.rect;
        while (true) {
            while (rect.height >= 2 * gen.fontSizeUsedForBestFit) {
                text.rectTransform.sizeDelta = new Vector2(text.rectTransform.sizeDelta.x, gen.fontSizeUsedForBestFit);
                yield return null;
                rect = text.rectTransform.rect;
            }
            yield return null;
        }
        //Debug.Log(linkedText);
        if (linkedText != null) {
            linkedText.Rezize();
        }
    }
}
