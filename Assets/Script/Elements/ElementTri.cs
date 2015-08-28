using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ElementTri : MonoBehaviour {

    public static List<ElementTri> CurrentTris;
    public GameObject movesLeft_GameObject;

    public int rotationAvailable = -1;
    public float size = 1f;
    public float addedRotation = 0;
    public bool canReceivePlayerInput = true;

    public ElementSocket Element_1;
    public ElementSocket Element_2;
    public ElementSocket Element_3;

    public MeshRenderer TriRenderer;
    public Material LineRenderersMaterial;
    private GameObject currentMovesLeftObj;

    private Vector3 pos_1;
    private Vector3 pos_2;
    private Vector3 pos_3;

    private static bool _canRotate = true;
    private bool _isComplete = false;

    void Awake(){
        AddToStaticList();

        pos_1 = (Quaternion.Euler(new Vector3(0,0,addedRotation)) * new Vector3(0, size, 0)) + transform.position;
        pos_2 = (Quaternion.Euler(new Vector3(0,0,addedRotation)) * new Vector3((size * Mathf.Sin(60 * Mathf.Deg2Rad)), -(size * Mathf.Cos(60 * Mathf.Deg2Rad)), 0)) + transform.position;
        pos_3 = (Quaternion.Euler(new Vector3(0,0,addedRotation)) * new Vector3(-(size * Mathf.Sin(60 * Mathf.Deg2Rad)), -(size * Mathf.Cos(60 * Mathf.Deg2Rad)), 0)) + transform.position;
        TriRenderer.transform.parent.Rotate(new Vector3(0, 0, addedRotation));

        DrawLinesBetweenPoints(pos_1,pos_2);
        DrawLinesBetweenPoints(pos_2, pos_3);
        DrawLinesBetweenPoints(pos_3, pos_1);

        transform.localScale = Vector3.one * size;
    }

    void Start(){
        ScaleElements();
        MoveElementsToPosition();
        UpdateCompletion();
        UpdateMovesText();
        TriRenderer.gameObject.layer = 8;
    }

    void OnDestroy() {
        if (CurrentTris.Contains(this)) {
            CurrentTris.Remove(this);
        }
        _canRotate = true;
    }

    private void AddToStaticList() {
        if (CurrentTris == null) {
            CurrentTris = new List<ElementTri>();
        }
        CurrentTris.Add(this);
    }

    private void UpdateMovesText() {
        if (rotationAvailable >= 0) {
            if (currentMovesLeftObj == null) {
                currentMovesLeftObj = (GameObject)Instantiate(movesLeft_GameObject, transform.position, Quaternion.identity);
                currentMovesLeftObj.transform.parent = transform;
            }
            if (rotationAvailable == 0) currentMovesLeftObj.GetComponentInChildren<TextMesh>().text = "X";
            else currentMovesLeftObj.GetComponentInChildren<TextMesh>().text = rotationAvailable.ToString();
        }
    }

    /// <summary> Makes all the tris unable to be used </summary>
    /// <returns></returns>
    public static void ToogleAllTrisActivation(bool canBeUsed) {
        for (int i = 0; i < CurrentTris.Count; i++) {
            CurrentTris[i].canReceivePlayerInput = canBeUsed;
        }
    }

    /// <summary> Updates all tris and return true if ALL of them are complete </summary>
    /// <returns></returns>
    public static bool UpdateAndGetAllTris() {
        bool allTrue = true;
        for (int i = 0; i < CurrentTris.Count; i++) {
            CurrentTris[i].UpdateCompletion();
            if (!CurrentTris[i]._isComplete) allTrue = false;
        }
        return allTrue;
    }

    public static bool CheckIfAllFalse() {
        bool allTrue = false;
        for (int i = 0; i < CurrentTris.Count; i++) {
            CurrentTris[i].UpdateCompletion();
            if (CurrentTris[i]._isComplete) allTrue = true;
        }
        return allTrue;
    }

    /// <summary> Draws lines inbetween three points </summary>
    private void DrawLinesBetweenPoints(Vector3 s, Vector3 e) {
        GameObject GO = new GameObject();
        GO.transform.parent = transform;
        LineRenderer lr = GO.AddComponent<LineRenderer>();
        lr.SetColors(Color.black, Color.black);
        lr.SetPosition(0, s);
        lr.SetPosition(1, e);
        lr.SetWidth(0.12f, 0.12f);
        lr.material = LineRenderersMaterial;

    }

    /// <summary> Returns true if all elements in this cycle are different </summary>
    /// <returns></returns>
    private bool IsComplete() {
        if ((Element_1.GetEType(this) != Element_2.GetEType(this)) &&
            (Element_2.GetEType(this) != Element_3.GetEType(this)) &&
            (Element_3.GetEType(this) != Element_1.GetEType(this))) {
            _isComplete = true;
            return true;
        } else {
            _isComplete = false;
            return false;
        }
    }

    public void UpdateCompletion() {
        if (IsComplete()) {
            TriRenderer.material.color = Color.green;  
        } else {
            TriRenderer.material.color = Color.red;
        }
    }

    public bool GetComplete() {
        return _isComplete;
    }

    private bool UseRotationAvailable() {
        bool rotates = false;
        if (rotationAvailable == -1) rotates = true;
        if (rotationAvailable > 0) {
            rotates = true;
            rotationAvailable--;
        }
        return rotates;
    }

    public void RotateTri() {
        //Debug.Log("current tris : " + CurrentTris.Count);
        if (_canRotate && canReceivePlayerInput) {
            if (UseRotationAvailable()) {

                if (GameManager.Instance != null) GameManager.Instance.AddMove();
                else Debug.Log("GameManager is null !");
                if (MusicManager.Instance != null) MusicManager.Instance.PlaySound_Move();
                //Logic
                Element old_1 = Element_1.GetElement(this);
                Element old_2 = Element_2.GetElement(this);
                Element old_3 = Element_3.GetElement(this);
                Element_1.ChangeElement(old_3, this);
                Element_2.ChangeElement(old_1, this);
                Element_3.ChangeElement(old_2, this);
                if (UpdateAndGetAllTris()) {
                    GameManager.Instance.CompleteLevel();
                } else if (GameManager.Instance.currentLevel == GameManager.LastLevel) {
                    if (CheckIfAllFalse()) {
                        GameManager.Instance.StartEnding_NoHeatDeath();
                    }
                }
                UpdateMovesText();

                //Start anim
                MoveElementsToPosition();
                //Start Cooldown
                StartCoroutine(RotationCooldown());

            }
        }
    }

    void MoveElementsToPosition() {
        Element_1.TranslateElement(pos_1, this);
        Element_2.TranslateElement(pos_2, this);
        Element_3.TranslateElement(pos_3, this);
    }

    void ScaleElements() {
        Element_1.GetElement(this).ScaleModel(size);
        Element_2.GetElement(this).ScaleModel(size);
        Element_3.GetElement(this).ScaleModel(size);
    }

    IEnumerator RotationCooldown() {
        _canRotate = false;
        yield return new WaitForSeconds(0.4f);
        _canRotate = true;
    }

    
}
