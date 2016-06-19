using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIGetter_Level : MonoBehaviour {

    private Text text;
    public string customTextBefore = "";

    void Start() {
        text = GetComponent<Text>();
    }

	void Update () {
        if (GameManager.Instance != null) text.text = customTextBefore + (GameManager.Instance.currentLevel + 1).ToString();
	}
}
