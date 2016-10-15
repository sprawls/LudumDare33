using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeactivateIfRecording : MonoBehaviour {

    private static List<DeactivateIfRecording> currentRecordings = new List<DeactivateIfRecording>();

    void Awake() {
        currentRecordings.Add(this);
    }

    void OnDestroy() {
        if (currentRecordings.Contains(this)) {
            currentRecordings.Remove(this);
        }
    }

    public static void UpdateAll() {
        foreach (DeactivateIfRecording currentRecording in currentRecordings) {
            currentRecording.ChangeActivation();
        }
    }

	void Start () {
        ChangeActivation();
	}

    void ChangeActivation() {
        gameObject.SetActive(LevelManager.RecordingMode);
    }
	
}
