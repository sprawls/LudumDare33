using UnityEngine;
using System.Collections;

//sh*t script handling the tap and mouse clicks of the donate button
public class HandleDonateButtonClickedOrTaped : MonoBehaviour {

    PauseMenuManager pauseMenuManager;
    float openWebsiteCooldown = 1f;
    bool canOpenWebsite = true;

    void Awake() {
        pauseMenuManager = GetComponentInParent<PauseMenuManager>();
    }

    void OnMouseDown() {
        AttemptOpenWebsite();
    }

	// Update is called once per frame
	void Update () {
	    if(CheckForTaps()) {
            AttemptOpenWebsite();
        }
	}

    private void AttemptOpenWebsite() {
        if(canOpenWebsite) {
            LevelManager.Instance.OpenGameWebsite();
            StartCoroutine(OpenWebsiteCooldown());
        }
    }

    private bool CheckForTaps() {
        foreach(Touch touch in Input.touches) {
            if (touch.phase == TouchPhase.Ended) {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit)) {
                    if (hit.collider.gameObject == gameObject) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    IEnumerator OpenWebsiteCooldown() {
        if(canOpenWebsite) { 
            canOpenWebsite = false;
            yield return new WaitForSeconds(openWebsiteCooldown);
            canOpenWebsite = true;
        }
    }
       
}
