using UnityEngine;
using System.Collections;

public class FaceCamera : MonoBehaviour {

    /// <summary>
    /// By default, will take the main camera
    /// </summary>
    public Transform targetCamera = null;
    public bool ChooseInverseCamera = false;

	void Awake () {
        if (targetCamera == null) {
            if (!ChooseInverseCamera) {
                targetCamera = GameObject.Find("Main Camera").transform;
            } else {
                targetCamera = GameObject.Find("Main Camera_Inverse").transform;
            }
        }   
	}
	
	void Update () {
        transform.rotation = Quaternion.LookRotation(targetCamera.transform.position - transform.position);
	}
}
