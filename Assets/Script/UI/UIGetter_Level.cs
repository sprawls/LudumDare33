using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIGetter_Level : MonoBehaviour {

    private Text text;

    void Start() {
        text = GetComponent<Text>();
    }

	void Update () {
        text.text = (GameManager.Instance.currentLevel + 1).ToString();
	}
}
