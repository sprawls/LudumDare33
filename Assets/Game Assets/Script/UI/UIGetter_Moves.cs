using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIGetter_Moves : MonoBehaviour {

    private Text text;

    void Start() {
        text = GetComponent<Text>();
    }

    void Update() {
        if (GameManager.Instance != null) text.text = GameManager.Instance.currentLevelMoves.ToString();
    }
}
