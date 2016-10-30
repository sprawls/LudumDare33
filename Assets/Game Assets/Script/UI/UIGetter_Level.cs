using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIGetter_Level : MonoBehaviour {

    private static List<UIGetter_Level> currentUIGetters = new List<UIGetter_Level>();

    private Text text;
    public string customTextBefore = "";

    void Awake() {
        text = GetComponent<Text>();
        currentUIGetters.Add(this);
    }

    void OnDestroy() {
        if (currentUIGetters.Contains(this)) {
            currentUIGetters.Remove(this);
        }
    }

    public static void UpdateAll(){
        foreach(UIGetter_Level uigetter in currentUIGetters){
            uigetter.UpdateText();
        }
    }

    void Start() {
        UpdateText();
    }

	void UpdateText () {
        //Debug.Log(GameManager.Instance.currentLevel + 1);
        if (GameManager.Instance != null) text.text = customTextBefore + (GameManager.Instance.currentLevel + 1).ToString();
	}
}
