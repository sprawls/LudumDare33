using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class UIGetter_Moves : MonoBehaviour {

    private static List<UIGetter_Moves> currentUIGetters = new List<UIGetter_Moves>();

    void Awake() {
        text = GetComponent<Text>();
        currentUIGetters.Add(this);
    }

    void OnDestroy() {
        if (currentUIGetters.Contains(this)) {
            currentUIGetters.Remove(this);
        }
    }

    public static void UpdateAll() {
        foreach (UIGetter_Moves uigetter in currentUIGetters) {
            uigetter.UpdateText();
        }
    }

    private Text text;
    public string customTextBefore = "";

    void Start() {
        UpdateText();
    }

    void UpdateText() {
        if (GameManager.Instance != null) text.text = customTextBefore + GameManager.Instance.currentLevelMoves.ToString();
    }
}
