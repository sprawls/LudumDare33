using UnityEngine;
using System.Collections;

public class LevelSelectOrb : MonoBehaviour {

    public GameObject orbPrefab_locked;
    public GameObject orbPrefab_unlocked;
    public GameObject orbPrefab_completed;

    public Collider buttonCollider;
	// Use this for initialization
	void Start () {
        buttonCollider = GetComponentInChildren<Collider>();
	}

    void OnMouseDown() {
        Debug.Log("Clicked");
    }
	
	
}
