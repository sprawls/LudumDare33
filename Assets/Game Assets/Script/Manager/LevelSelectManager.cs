using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class LevelSelectManager : MonoBehaviour {

    public Button InverseButton;
    public Button LeftButton;
    public Button RightButton;
    public GameObject Prefab_PopupWindow;

    public bool alwaysShowInverseButton { get; private set; }

    private SimpleSwipeInputDetector swipeInputs;

    void Awake() {
        alwaysShowInverseButton = true;
    }

	void Start () {
        swipeInputs = GetComponent<SimpleSwipeInputDetector>();
        swipeInputs.OnSwipeRight += NextWorld;
        swipeInputs.OnSwipeLeft += PreviousWorld;

        UpdateNavigationButtons();
        HandlePopupWindows();
	}

    void OnDestroy() {
        if (swipeInputs != null) {
            swipeInputs.OnSwipeRight -= NextWorld;
            swipeInputs.OnSwipeLeft -= PreviousWorld;
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

    private void HandlePopupWindows() {
        NotificationPopup.ENotificationPopupType type = NotificationPopup.ENotificationPopupType.None;
        int amtStars = LevelManager.Instance.GetTotalAmountStars();
        DateTime currentTime = DateTime.Now;

        if (!LevelManager.Instance.statsData.notif_chaos_shown && amtStars > 10 && CanShowNotification(currentTime, LevelManager.Instance.statsData.notif_chaos_time)) {
            type = NotificationPopup.ENotificationPopupType.InverseWorld;
        } else if (!LevelManager.Instance.statsData.notif_rate_shown && amtStars > 18 && CanShowNotification(currentTime, LevelManager.Instance.statsData.notif_rate_time)) {
            type = NotificationPopup.ENotificationPopupType.Rate;
        } else if (!LevelManager.Instance.statsData.notif_tip_shown && amtStars > 36 && CanShowNotification(currentTime, LevelManager.Instance.statsData.notif_tip_time)) {
            type = NotificationPopup.ENotificationPopupType.Donate;
        }

        if (type != NotificationPopup.ENotificationPopupType.None && Prefab_PopupWindow != null) {
            GameObject popupWindow = (GameObject) Instantiate(Prefab_PopupWindow,Vector3.zero, Quaternion.identity);
            NotificationPopup notifPopup = popupWindow.GetComponent<NotificationPopup>();
            switch (type) {
                case NotificationPopup.ENotificationPopupType.Donate:
                    notifPopup.Initialize(NotificationPopup.ENotificationPopupType.Donate);
                    break;
                case NotificationPopup.ENotificationPopupType.InverseWorld:
                    notifPopup.Initialize(NotificationPopup.ENotificationPopupType.InverseWorld);
                    break;
                case NotificationPopup.ENotificationPopupType.Rate:
                    notifPopup.Initialize(NotificationPopup.ENotificationPopupType.Rate);
                    break;
                default :
                    Debug.LogWarning("Tring to spawn an unimplemented Popup window !");
                    Destroy(popupWindow);
                    break;
            }
        } else {
            Debug.LogWarning("Popup Prefab not selected in editor !");
        }

    }

    private bool CanShowNotification(DateTime now, DateTime last) {
        if ((now - last).Days >= 1) return true;
        else return false;
    }
}
