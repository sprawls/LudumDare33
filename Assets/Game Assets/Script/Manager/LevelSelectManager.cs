using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelSelectManager : MonoBehaviour {

    public Button InverseButton;
    public Button LeftButton;
    public Button RightButton;

    public bool alwaysShowInverseButton { get; private set; }

    private SimpleSwipeInputDetector swipeInputs;

    void Awake() {
        alwaysShowInverseButton = true;
    }

	void Start () {
        swipeInputs = GetComponent<SimpleSwipeInputDetector>();
        swipeInputs.OnSwipeLeft += NextWorld;
        swipeInputs.OnSwipeRight += PreviousWorld;

        UpdateNavigationButtons();
	}

    void OnDestroy() {
        if (swipeInputs != null) {
            swipeInputs.OnSwipeLeft -= NextWorld;
            swipeInputs.OnSwipeRight -= PreviousWorld;
        }
    }

    public void BackToMenu() {
        LevelManager.Instance.OnClick_BackToMenu();
    }

    public void InverseWorld() {
        if (LevelManager.Instance.inInverseAnimation) return;
        LevelManager.Instance.OnClick_InverseWorld();
        UpdateNavigationButtons();
    }

    public void NextWorld() {
        if (LevelManager.Instance.inInverseAnimation) return;
        LevelManager.Instance.OnClick_NextWorld();
        UpdateNavigationButtons();
        CheckIfInversionNeeded();
    }

    public void PreviousWorld() {
        if (LevelManager.Instance.inInverseAnimation) return;
        LevelManager.Instance.OnClick_PreviousWorld();
        UpdateNavigationButtons();
        CheckIfInversionNeeded();
    }

    public void OpenGooglePlay() {
        LevelManager.Instance.OpenGooglePlay();
    }

    private void CheckIfInversionNeeded() {
        if (alwaysShowInverseButton) return;
        //Debug.Log(LevelManager.Instance.currentSelectedWorld);
        if (LevelManager.Instance.currentSelectedWorld % 2 == 0) {
            //Debug.Log(LevelManager.Instance.GetWorldAllCompletedLevels(LevelManager.Instance.currentSelectedWorld - 1));
            if (!LevelManager.Instance.GetWorldAllCompletedLevels(LevelManager.Instance.currentSelectedWorld-1)) {
                InverseWorld();
            }
        }
    }

    private void UpdateNavigationButtons() {
        int world = LevelManager.Instance.currentSelectedWorld;
        int worldImpair = (world % 2 == 0) ? world - 1 : world;

        //Check Inverse Button
        if (InverseButton != null) {
            if (alwaysShowInverseButton) {
                InverseButton.interactable = true;
                InverseButton.gameObject.SetActive(true);
            } else {
                if (world % 2 == 1) {
                    if (LevelManager.Instance.GetWorldAllCompletedLevels(world)) {
                        InverseButton.interactable = true;
                        InverseButton.gameObject.SetActive(true);
                    } else {
                        InverseButton.interactable = false;
                        InverseButton.gameObject.SetActive(false);
                    }
                } else {
                    InverseButton.interactable = true;
                    InverseButton.gameObject.SetActive(true);
                }
            }
        } else Debug.Log("Inverse Button is null !");


        //Check Left Button
        if (LeftButton != null) {
            if (worldImpair > 1) {
                LeftButton.interactable = true;
                LeftButton.gameObject.SetActive(true);
            } else {
                LeftButton.interactable = false;
                LeftButton.gameObject.SetActive(false);
            }
        } else Debug.Log("Left Button is null !");

        //Check Right Button
        if (RightButton != null) {
            if (worldImpair + 2 < LevelManager.Instance.worldList_par.Count) {
                RightButton.interactable = true;
                RightButton.gameObject.SetActive(true);
            } else {
                RightButton.interactable = false;
                RightButton.gameObject.SetActive(false);
            }
        } else Debug.Log("Right Button is null !");
    }

}
