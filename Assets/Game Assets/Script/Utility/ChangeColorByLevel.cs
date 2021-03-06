﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class ChangeColorByLevel : MonoBehaviour {

    public bool ChangeText = false;
    public bool ChangeMeshRenderer = false;
    public bool ChangeImage = false;

    private static List<ChangeColorByLevel> colorList;

    public Color StartColor;
    public Color EndColor;

    private MeshRenderer meshRendrer;
    private Text text;
    private Image image;

	void Awake(){
        if(colorList == null) colorList = new List<ChangeColorByLevel>();
        colorList.Add(this);
    }

    void Start() {
        if(ChangeText) text = GetComponent<Text>();
        if(ChangeMeshRenderer) meshRendrer = GetComponent<MeshRenderer>();
        if (ChangeImage) image = GetComponent<Image>();
    }

    void OnDestroy(){
        if(colorList.Contains(this)) colorList.Remove(this);
    }

    public static void UpdateAllColor(){
        foreach(ChangeColorByLevel col in colorList){
            col.UpdateColor();
        }
    }

    public void UpdateColor(){
        Color TargetColor = Color.Lerp(StartColor, EndColor, (float)GameManager.Instance.currentLevel / (float)GameManager.Instance.LevelsList_w1.Count);
        
        if(meshRendrer != null) meshRendrer.material.DOColor(TargetColor, 2f);
        if(text != null) text.DOColor(TargetColor, 2f);
        if (image != null) image.DOColor(TargetColor, 2f);
    }
}
