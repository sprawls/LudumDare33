using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;


public class NotificationPopup : MonoBehaviour {

    public enum ENotificationPopupType {
        Donate, Rate, InverseWorld, None
    }


    public Text Text_Title;
    public Button Button_Refuse;
    public Button Button_Accept;
    public Button Button_Later;

    private Text _Text_Refuse;
    private Text _Text_Accept;
    private Text _Text_Later;

    private ENotificationPopupType _popupType;

    private void Awake() {
        _Text_Refuse = Button_Refuse.GetComponentInChildren<Text>();
        _Text_Accept = Button_Accept.GetComponentInChildren<Text>();
        _Text_Later = Button_Later.GetComponentInChildren<Text>();

        LevelManager.Instance.OnSetPopupIsOnScreen(true);
    }

    private void OnDestroy() {
        LevelManager.Instance.OnSetPopupIsOnScreen(false);
    }

    public void Initialize(ENotificationPopupType type) {
        _popupType = type;

        UpdateButtons();
        UpdateText();
    }

    private void UpdateButtons() {
        switch(_popupType){
            case ENotificationPopupType.Donate :
                Button_Accept.onClick.AddListener(this.OpenWebsite);
                Button_Refuse.onClick.AddListener(this.DontRemindLater);
                Button_Later.onClick.AddListener(this.RemindLater);

                _Text_Accept.text = "Donate";
                _Text_Refuse.text = "No Thanks";
                _Text_Later.text = "Remind me Later";
                break;
            case ENotificationPopupType.InverseWorld:
                Button_Accept.onClick.AddListener(this.CloseWindow);
                Button_Refuse.onClick.AddListener(this.DontRemindLater);
                Destroy(Button_Later.gameObject);

                _Text_Accept.text = "Try it now";
                _Text_Refuse.text = "Maybe Later";
                break;
            case ENotificationPopupType.Rate :
                Button_Accept.onClick.AddListener(this.OpenGooglePlayPage);
                Button_Refuse.onClick.AddListener(this.CloseWindow);
                Button_Later.onClick.AddListener(this.RemindLater);

                _Text_Accept.text = "Rate it";
                _Text_Refuse.text = "No Thanks";
                _Text_Later.text = "Remind me Later";
                break;
        }
    }


    private void UpdateText() {
        switch(_popupType){
            case ENotificationPopupType.Donate :
                Text_Title.text = "You've been playing for a while. Please consider donating to support the game !";
                break;
            case ENotificationPopupType.InverseWorld:
                Text_Title.text = "By clicking the button at the top right, you can access chaos mode and play a new version of every level you completed !";
                break;
            case ENotificationPopupType.Rate :
                Text_Title.text = "Thanks for playing ! Support the game by rating it on Google Play !";
                break;
        }
    }

    private void CloseWindow() {
        Destroy(gameObject);
    }

    private void OpenWebsite() {
        LevelManager.Instance.OpenGameWebsite();
        DontRemindLater();
    }

    private void OpenGooglePlayPage() {
        LevelManager.Instance.OpenGooglePlayPage();
        DontRemindLater();
    }

    private void RemindLater() {
        switch (_popupType) {
            case ENotificationPopupType.Donate:
                LevelManager.Instance.statsData.notif_tip_time = DateTime.Now;
                break;
            case ENotificationPopupType.InverseWorld:
                LevelManager.Instance.statsData.notif_chaos_time = DateTime.Now;
                break;
            case ENotificationPopupType.Rate:
                LevelManager.Instance.statsData.notif_rate_time = DateTime.Now;
                break;
        }
        CloseWindow();
    }

    private void DontRemindLater() {
        switch (_popupType) {
            case ENotificationPopupType.Donate:
                LevelManager.Instance.statsData.notif_tip_shown = true;
                break;
            case ENotificationPopupType.InverseWorld:
                LevelManager.Instance.statsData.notif_chaos_shown = true;
                break;
            case ENotificationPopupType.Rate:
                LevelManager.Instance.statsData.notif_rate_shown = true;
                break;
        }
        CloseWindow();
    }
}
