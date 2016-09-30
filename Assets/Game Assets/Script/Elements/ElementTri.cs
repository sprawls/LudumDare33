using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR_WIN
using UnityEditor;
#endif

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
    public Transform _transform;

    public MeshRenderer TriRenderer;
    public Material LineRenderersMaterial;
    private GameObject currentMovesLeftObj;
    private MovesLeftObject currentMovesLeftScript;

    private Vector3 pos_1;
    private Vector3 pos_2;
    private Vector3 pos_3;

    private static bool _canRotate = true;
    private bool _isComplete = false;

    [Header("Gizmos")]
    public Mesh GizmoMesh;

    void Awake(){
        _transform = transform;
        AddToStaticList();

        pos_1 = (Quaternion.Euler(new Vector3(0, 0, addedRotation)) * new Vector3(0, size, 0)) + _transform.position;
        pos_2 = (Quaternion.Euler(new Vector3(0, 0, addedRotation)) * new Vector3((size * Mathf.Sin(60 * Mathf.Deg2Rad)), -(size * Mathf.Cos(60 * Mathf.Deg2Rad)), 0)) + _transform.position;
        pos_3 = (Quaternion.Euler(new Vector3(0, 0, addedRotation)) * new Vector3(-(size * Mathf.Sin(60 * Mathf.Deg2Rad)), -(size * Mathf.Cos(60 * Mathf.Deg2Rad)), 0)) + _transform.position;
        TriRenderer.transform.parent.Rotate(new Vector3(0, 0, addedRotation));

        Vector3 ZLineOffset = new Vector3(0, 0, 1);
        DrawLinesBetweenPoints(pos_1 + ZLineOffset, pos_2 + ZLineOffset);
        DrawLinesBetweenPoints(pos_2 + ZLineOffset, pos_3 + ZLineOffset);
        DrawLinesBetweenPoints(pos_3 + ZLineOffset, pos_1 + ZLineOffset);

        _transform.localScale = Vector3.one * size;
    }

    void Start(){
        ScaleElements();
        SetupElementsToPosition();
        ChangeOrbBackgroundsColor();
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
                currentMovesLeftObj = (GameObject)Instantiate(movesLeft_GameObject, _transform.position, Quaternion.identity);
                currentMovesLeftObj.transform.parent = _transform;
                currentMovesLeftScript = currentMovesLeftObj.GetComponentInChildren<MovesLeftObject>();
                if (currentMovesLeftScript == null) Debug.Log("No moves left script found !");
            }
            if (rotationAvailable == 0) currentMovesLeftScript.UpdateText("0");
            else currentMovesLeftScript.UpdateText(rotationAvailable.ToString());
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
        if (!LevelManager.Instance.isInverseWorld()) return CheckCompletionNormal();
        else return CheckCompletionInverse();
    }

    private static bool CheckCompletionNormal() {
        bool allTrue = true;
        for (int i = 0; i < CurrentTris.Count; i++) {
            CurrentTris[i].UpdateCompletion();
            if (!CurrentTris[i]._isComplete) allTrue = false;
        }
        return allTrue;
    }

    private static bool CheckCompletionInverse() {
        bool allFalse = true;
        for (int i = 0; i < CurrentTris.Count; i++) {
            CurrentTris[i].UpdateCompletion();
            if (CurrentTris[i]._isComplete) allFalse = false;
        }
        return allFalse;
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
        GO.transform.parent = _transform;
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
        if (((Element_1.GetEType(this) != Element_2.GetEType(this)) || (Element_1.GetEType(this) == ElementType.all)) &&
            ((Element_2.GetEType(this) != Element_3.GetEType(this)) || (Element_2.GetEType(this) == ElementType.all)) &&
            ((Element_3.GetEType(this) != Element_1.GetEType(this)) || (Element_3.GetEType(this) == ElementType.all))) {
            _isComplete = true;
            return true;
        } else {
            _isComplete = false;
            return false;
        }
    }

    private void UpdateCompletion() {
        if (LevelManager.Instance.isInverseWorld()) {
            if (IsComplete()) {
                TriRenderer.material.color = new Color(0.25f,0.7f,0.25f,1);
            } else {
                TriRenderer.material.color = new Color(0.7f, 0.15f, 0.15f, 1);
            }
        } else {
            if (IsComplete()) {
                TriRenderer.material.color = new Color(0,0.9f,0,1);
            } else {
                TriRenderer.material.color = Color.red;
            }
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
        if (_canRotate && canReceivePlayerInput && !GameManager.Instance.InAnimation && LevelManager.Instance.CanClickOnLevels()) {
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
                } 
                UpdateMovesText();

                //Start anim
                MoveElementsToPosition();
                ChangeOrbBackgroundsColor();
                //Start Cooldown
                StartCoroutine(RotationCooldown());

                UpdateRotationStats();
            }
        }
    }

    void MoveElementsToPosition() {
        Element_1.TranslateElement(pos_1, this);
        Element_2.TranslateElement(pos_2, this);
        Element_3.TranslateElement(pos_3, this);
    }

    void SetupElementsToPosition() {
        Element_1.SetupElement(pos_1, this);
        Element_2.SetupElement(pos_2, this);
        Element_3.SetupElement(pos_3, this); 
    }

    void ChangeOrbBackgroundsColor() {
        Element_1.ColorElements();
        Element_2.ColorElements();
        Element_3.ColorElements();
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

    void UpdateRotationStats() {
        //Rotation
        LevelManager.Instance.IncrementRotations();

        //Multicolor Rotation
        int amtMulticolorOrbs = 0;
        if (Element_1.GetEType(this) == ElementType.all) ++amtMulticolorOrbs;
        if (Element_2.GetEType(this) == ElementType.all) ++amtMulticolorOrbs;
        if (Element_3.GetEType(this) == ElementType.all) ++amtMulticolorOrbs;
        if (amtMulticolorOrbs > 0) LevelManager.Instance.IncrementRotations_Multicolor(amtMulticolorOrbs);

        //Limited Rotation
        if (rotationAvailable >= 0) LevelManager.Instance.IncrementRotations_LimitedTri();

        //Fusion 
        int amtFusionSockets = 0;
        if (Element_1.GetType() == typeof(FusionSocket)) ++amtFusionSockets;
        if (Element_2.GetType() == typeof(FusionSocket)) ++amtFusionSockets;
        if (Element_3.GetType() == typeof(FusionSocket)) ++amtFusionSockets;
        if (amtFusionSockets > 0) LevelManager.Instance.IncrementFusions(amtFusionSockets);
    }

#if UNITY_EDITOR_WIN
    //GIZMO DEBUG
    void OnDrawGizmos() {
        if(Application.isPlaying) return; 
        if(GizmoMesh) Gizmos.DrawMesh(  GizmoMesh,
                                        _transform.position + (Quaternion.Euler(0, 0, addedRotation) * new Vector3(0f, 0.3f, 2f)), 
                                        Quaternion.Euler(0,0,addedRotation), 
                                        new Vector3(1.65f,1.545f,1.65f) * size) ;

        Vector3 pos_1 = (Quaternion.Euler(new Vector3(0, 0, addedRotation)) * new Vector3(0, size, 0)) + _transform.position;
        Vector3 pos_2 = (Quaternion.Euler(new Vector3(0, 0, addedRotation)) * new Vector3((size * Mathf.Sin(60 * Mathf.Deg2Rad)), -(size * Mathf.Cos(60 * Mathf.Deg2Rad)), 0)) + _transform.position;
        Vector3 pos_3 = (Quaternion.Euler(new Vector3(0, 0, addedRotation)) * new Vector3(-(size * Mathf.Sin(60 * Mathf.Deg2Rad)), -(size * Mathf.Cos(60 * Mathf.Deg2Rad)), 0)) + _transform.position;

        if (Element_1.element.EType != ElementType.none) {
            Gizmos.color = GetSocketGizmoColor(Element_1);
            Gizmos.DrawSphere(pos_1, 0.5f * size);
        }
        if (Element_2.element.EType != ElementType.none) {
            Gizmos.color = GetSocketGizmoColor(Element_2);
            Gizmos.DrawSphere(pos_2, 0.5f * size);
        }
        if (Element_3.element.EType != ElementType.none) {
            Gizmos.color = GetSocketGizmoColor(Element_3);
            Gizmos.DrawSphere(pos_3, 0.5f * size);
        }

    }

    private Color GetSocketGizmoColor(ElementSocket es) {
        switch (es.element.EType) {
            case ElementType.fire :
                return Color.red;
            case ElementType.water:
                return Color.blue;
            case ElementType.wind:
                return Color.cyan;
            case ElementType.all:
                return Color.green;
            default:
                return Color.grey;
        }
    }
#endif
}
