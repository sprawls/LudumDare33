using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActivationOnInverseWorld : MonoBehaviour {

    private static List<ActivationOnInverseWorld> scriptList;
    private GameObject go;

    public bool ActivateOnInverseWorld = false;

    void Awake() {
        if (scriptList == null) scriptList = new List<ActivationOnInverseWorld>();
        scriptList.Add(this);
        go = gameObject;
    }

	void Start () {
        UpdateActivation();
	}

    void OnDestroy() {
        if (scriptList.Contains(this)) scriptList.Remove(this);
    }

    public static void UpdateAll() {
        foreach (ActivationOnInverseWorld script in scriptList) {
            script.UpdateActivation();
        }
    }

    public void UpdateActivation() {
        bool inverse = LevelManager.Instance.isInverseWorld() ? true : false;
        go.SetActive(ActivateOnInverseWorld == inverse);
    }
}
