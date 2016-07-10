using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class ChangeColorByInverseWorld : MonoBehaviour {

    public bool ChangeText = false;
    public bool ChangeMeshRenderer = false;
    public bool ChangeImage = false;

    private static List<ChangeColorByInverseWorld> colorList;

    public Color normalColor;
    public Color InverseColor;

    private MeshRenderer meshRendrer;
    private Text text;
    private Image image;

	void Awake(){
        if (colorList == null) colorList = new List<ChangeColorByInverseWorld>();
        colorList.Add(this);
    }

    void Start() {
        if(ChangeText) text = GetComponent<Text>();
        if(ChangeMeshRenderer) meshRendrer = GetComponent<MeshRenderer>();
        if (ChangeImage) image = GetComponent<Image>();
        UpdateColor(0f);
    }

    void OnDestroy(){
        if(colorList.Contains(this)) colorList.Remove(this);
    }

    public static void UpdateAllColor(){
        foreach (ChangeColorByInverseWorld col in colorList) {
            col.UpdateColor(2f);
        }
    }

    public void UpdateColor(float time){
        Color TargetColor = LevelManager.Instance.isInverseWorld() ? InverseColor : normalColor;

        if (meshRendrer != null) meshRendrer.material.DOColor(TargetColor, time);
        if (text != null) text.DOColor(TargetColor, time);
        if (image != null) image.DOColor(TargetColor, time);
    }
}
