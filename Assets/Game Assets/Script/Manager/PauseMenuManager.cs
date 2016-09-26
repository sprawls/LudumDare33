using UnityEngine;
using System.Collections;

public class PauseMenuManager : MonoBehaviour {

    public GameObject MenuObject;

    public void ToggleSounds() {
        LevelManager.Instance.ToggleMuteSounds();
    }

    public void ToggleMusic() {
        LevelManager.Instance.ToggleMuteMusic();
    }

    public void ToggleOptions() {
        if (LevelManager.Instance.gameIsPaused) {
            LevelManager.Instance.OnSetPause(false);
            MenuObject.SetActive(false);
        } else {
            LevelManager.Instance.OnSetPause(true);
            MenuObject.SetActive(true);
        }
    }

    public void ShowAchievements() {
        LevelManager.Instance.OpenGooglePlay();
    }

    public void OpenWebsite() {
        Application.OpenURL("http://sprawls.github.io/");
    }
}
