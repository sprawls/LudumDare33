using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles the touch inputs on the screen and calls the function on touched gameobjects
/// </summary>

public class TouchInput : MonoBehaviour {

    public LayerMask TouchInputMask;
    private Camera InputCamera;

    //Swipe Struct
    private struct SwipeInfo {
        public bool canBeSwipe_Vertical;
        public bool canBeSwipe_Horizontal;
        public Vector2 StartPos;
        public float StartTime;
        public float idleTime;
    }

    //New Input
    private List<MonoBehaviour> touchesReceivers = new List<MonoBehaviour>();
    private GameObject[] touchesTargets;
    private SwipeInfo[] SwipeInfos;
    //swipe values
    private float comfortZone = 100;
    private float maxSwipeTime = 1.5f;
    private float minSwipeDist = 100f;
    private float maxIdleTime = 0.2f;

    void Awake() {
        GameObject[] receiversGOs = GameObject.FindGameObjectsWithTag("ReceivesTouchInputs");
        foreach(GameObject go in receiversGOs){
            MonoBehaviour[] monos = go.GetComponents<MonoBehaviour>();
            foreach(MonoBehaviour mono in monos) {
                touchesReceivers.Add(mono);
            }
        }
        touchesTargets = new GameObject[6];
        SwipeInfos = new SwipeInfo[6];
        InputCamera = Camera.main;
    }

    void Update() {

        for (int t = 0; t < Input.touches.Length; ++t) {
            processATouchPerFingerCodeNumber(Input.touches[t], Input.touches[t].fingerId);
        }


#if (UNITY_EDITOR)
        // ONLY COMPILED IN EDITOR
        if (Input.GetMouseButtonDown(0) && Camera.main != null) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, TouchInputMask)) {
                //Debug.Log("DEBUG EDITOR : Touch Input with Mouse");
                GameObject recipient = hit.collider.transform.gameObject;
                recipient.SendMessage("FingerOn", -1, SendMessageOptions.DontRequireReceiver);
                recipient.SendMessage("FingerOff", -1, SendMessageOptions.DontRequireReceiver);
            }
        }
#endif


    }

    void processATouchPerFingerCodeNumber(Touch t, int n) {
        if (n >= 6)
            return; //Only support up to five fingers

        if (t.phase == TouchPhase.Began) {
            AddInputAt(t, n);
            return;

        } else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled) {
            RemoveInputAt(t, n);
            return;

        } else if (t.phase == TouchPhase.Moved) {
            MoveInputAt(t, n);
            return;

        } else {
            IdleInputAt(t, n);
            return;
        }

        // CHECK STATIONARY HERE IF NEEDED LATER //

    }

    void AddInputAt(Touch touch, int n) {
        Ray ray = InputCamera.ScreenPointToRay(touch.position);
        RaycastHit hit;
        //Swipe Check
        SwipeInfos[n].canBeSwipe_Vertical = true;
        SwipeInfos[n].canBeSwipe_Horizontal = true;
        SwipeInfos[n].StartTime = Time.time;
        SwipeInfos[n].StartPos = touch.position;
        SwipeInfos[n].idleTime = 0;
        //Debug.Log ("Finger id " + n + " has a Tapcount of " + touch.tapCount);
        if (Physics.Raycast(ray, out hit, TouchInputMask)) {

            GameObject recipient = hit.collider.transform.gameObject;
            recipient.SendMessage("FingerOn", n, SendMessageOptions.DontRequireReceiver);
            touchesTargets[n] = recipient;
            //Debug.Log("recipient : " + recipient);
            //Check for double click
            if (touch.tapCount > 1) {
                recipient.SendMessage("DoubleClickOn", n, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    void RemoveInputAt(Touch touch, int n) {
        //check Swipe 
        float swipeTime = Time.time - SwipeInfos[n].StartTime;
        float swipeDist_x = (touch.position.x - SwipeInfos[n].StartPos.x);
        float swipeDist_y = Mathf.Abs(touch.position.y - SwipeInfos[n].StartPos.y);

        Debug.Log("Swipe " + n + " :  canBeV : " + SwipeInfos[n].canBeSwipe_Vertical + "    canBeH : " + SwipeInfos[n].canBeSwipe_Horizontal + "   time : " + swipeTime + "     distx : " + swipeDist_x + "          disty : " + swipeDist_y);

        if (SwipeInfos[n].canBeSwipe_Vertical && (swipeTime < maxSwipeTime) && (swipeDist_y > minSwipeDist)) {
            //We have a vertical swipe :
            //inputManager.AddMiddleButton(gameObject);
            Debug.Log("Vertical Swipe by : " + n);
        } else if (SwipeInfos[n].canBeSwipe_Horizontal && (swipeTime < maxSwipeTime) && (swipeDist_x > minSwipeDist)) {
            //We have a horizontal swipe :
            SendReceivers_SwipeRight();
            Debug.Log("Right Swipe by : " + n);
        } else if (SwipeInfos[n].canBeSwipe_Horizontal && (swipeTime < maxSwipeTime) && (swipeDist_x < -minSwipeDist)) {
            //We have a horizontal swipe :
            SendReceivers_SwipeLeft();
            Debug.Log("Left Swipe by : " + n);
        }

        //remove input
        RemoveInputFromButton(touch, n);

    }

    void MoveInputAt(Touch touch, int n) {
        Ray ray = InputCamera.ScreenPointToRay(touch.position);
        RaycastHit hit;
        //Handle Swipe
        if (Mathf.Abs(touch.position.x - SwipeInfos[n].StartPos.x) > comfortZone) { //Cant be swipe is deviate too much from a straith line
            SwipeInfos[n].canBeSwipe_Vertical = false;
            //Debug.Log("Swipe Cancelled by mouv : " + n);
        }
        if (Mathf.Abs(touch.position.y - SwipeInfos[n].StartPos.y) > comfortZone) { //Cant be swipe is deviate too much from a straith line
            SwipeInfos[n].canBeSwipe_Horizontal = false;
            //Debug.Log("Swipe Cancelled by mouv : " + n);
        }
        SwipeInfos[n].idleTime = 0;

        //check Button presses
        if (Physics.Raycast(ray, out hit, TouchInputMask)) {

            GameObject newRecipient = hit.collider.transform.gameObject;
            if (touchesTargets[n] != null && touchesTargets[n] == newRecipient) {
                return;//We are on the same target even though we moved, handle swipes here
            } else {
                RemoveInputFromButton(touch, n);
                newRecipient.SendMessage("FingerOn", n, SendMessageOptions.DontRequireReceiver);
                touchesTargets[n] = newRecipient;
            }



        }
    }

    void IdleInputAt(Touch touch, int n) {
        SwipeInfos[n].idleTime += Time.deltaTime;
        //Debug.Log ("Swipe " + n + " idle for : " + SwipeInfos [n].idleTime );
        if (SwipeInfos[n].idleTime > maxIdleTime) {
            SwipeInfos[n].canBeSwipe_Vertical = false; //Cant be swipe if idle
            SwipeInfos[n].canBeSwipe_Horizontal = false; //Cant be swipe if idle
            //Debug.Log("Swipe Cancelled by idle : " + n + "      MaxIdleTime : " + maxIdleTime);
        }

    }

    void RemoveInputFromButton(Touch touch, int n) {
        if (touchesTargets[n] != null) {
            GameObject recipient = touchesTargets[n];
            recipient.SendMessage("FingerOff", n, SendMessageOptions.DontRequireReceiver);
        }
        touchesTargets[n] = null;
    }


    void SendReceivers_SwipeLeft() {
        foreach (MonoBehaviour mono in touchesReceivers) {
            mono.SendMessage("Touch_SwipedLeft", -1, SendMessageOptions.DontRequireReceiver);
        }
    }

    void SendReceivers_SwipeRight() {
        foreach (MonoBehaviour mono in touchesReceivers) {
            mono.SendMessage("Touch_SwipedRight", -1, SendMessageOptions.DontRequireReceiver);
        }
    }

}
