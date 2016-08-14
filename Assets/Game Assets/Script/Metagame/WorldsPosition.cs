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
	}

    public void SetWorldPosition(int worldToShow) {
        SetWorldPosition(worldToShow, 1f);
    }

    public void SetWorldPosition(int worldToShow, float time) {
        if (worldToShow >= 0 && worldToShow < worldsTransforms.Count) {
            currentWorld = worldToShow;
            SetWorldPosition(time);
        }
    }

    void SetWorldPosition(float time) {
        MoveToSelectedWorld(time);
    }


    void MoveToSelectedWorld(float time) {
        transform.DOMove(worldsTransforms[currentWorld].position, time);
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
