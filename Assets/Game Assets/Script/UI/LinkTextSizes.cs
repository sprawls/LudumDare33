using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LinkTextSizes : MonoBehaviour {

    public Text linkedTextSize;
    public bool UseSmallestFont = true;
    public bool RezizeOnStart = true;
    public bool RezizeContinuously = true;

    private Text text;

    void Awake() {
        text = GetComponent<Text>();
    }

	void Start () {
        if(RezizeOnStart) Rezize();
	}

    public void Rezize() {
        if (linkedTextSize == null) return;
        if (RezizeContinuously) {
            StartCoroutine(RezizeContinuous());
            return;
        }
        int targetSize;
        if (UseSmallestFont) targetSize = Mathf.Min(text.fontSize, linkedTextSize.fontSize);
        else targetSize = Mathf.Max(text.fontSize, linkedTextSize.fontSize);

        text.fontSize = targetSize;
        linkedTextSize.fontSize = targetSize;
    }

    IEnumerator RezizeContinuous() {
        while (true) { 
            int targetSize;
            if (UseSmallestFont) targetSize = Mathf.Min(text.fontSize, linkedTextSize.fontSize);
            else targetSize = Mathf.Max(text.fontSize, linkedTextSize.fontSize);

            text.fontSize = targetSize;
            linkedTextSize.fontSize = targetSize;

            yield return new WaitForSeconds(0.5f);
        }
    }
}
