using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// Centers the correct world select on the screen. 
/// </summary>
public class WorldsPosition : MonoBehaviour {

    public List<Transform> worldsTransforms;
    public int currentWorld {get; private set;}

	// Use this for initialization
	void Start () {
        SetWorldPosition(0);
	}

    public void SetWorldPosition(int worldToShow) {
        Debug.Log("moving to world : " + worldToShow);
        if (worldToShow >= 0 && worldToShow < worldsTransforms.Count) {
            currentWorld = worldToShow;
            SetWorldPosition();
        }
    }

    void SetWorldPosition() {
        MoveToSelectedWorld();
    }


    void MoveToSelectedWorld() {
        transform.DOMove(worldsTransforms[currentWorld].position, 1f);
    }

    /// <summary>
    /// Deactivate all level select colliders
    /// </summary>
    void DeactivateSphereColliders() {
        foreach (Transform t in worldsTransforms) {
            SphereCollider[] colls = t.GetComponentsInChildren<SphereCollider>();
            foreach (SphereCollider s in colls) { s.enabled = false; }
        }
    }

    /// <summary>
    /// Activate Children Colliders of given object
    /// </summary>
    /// <param name="go"></param>
    void ActivateSphereCollidersInObject(GameObject go) {
        LevelSelectOrb[] orbs = go.GetComponentsInChildren<LevelSelectOrb>();
        foreach (LevelSelectOrb lso in orbs) { lso.UpdateOrb(); }
    }

    public void Touch_SwipedLeft() {
        SetWorldPosition(currentWorld -2);
    }

    public void Touch_SwipedRight() {
        SetWorldPosition(currentWorld + 2);
    }
}
