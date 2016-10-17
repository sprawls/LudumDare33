using UnityEngine;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;



public struct Achievements {
    public static string Learner                = "CgkI3tmTz-kLEAIQAA";
    public static string Hamory_Centaurus       = "CgkI3tmTz-kLEAIQAQ";
    public static string Hamory_Hydra           = "CgkI3tmTz-kLEAIQAg";
    public static string Chaos_Centaurus        = "CgkI3tmTz-kLEAIQBQ";
    public static string Chaos_Hydra            = "CgkI3tmTz-kLEAIQAw";
    public static string Equilibrium_Stage1     = "CgkI3tmTz-kLEAIQBw";
    public static string Equilibrium_Stage2     = "CgkI3tmTz-kLEAIQCA";
    public static string Equilibrium_Stage3     = "CgkI3tmTz-kLEAIQCQ";
    public static string Equilibrium_Stage4     = "CgkI3tmTz-kLEAIQCg";
    public static string Mastery_Rotation       = "CgkI3tmTz-kLEAIQCw";
    public static string Mastery_Fusion         = "CgkI3tmTz-kLEAIQDA";
    public static string Mastery_Limitations    = "CgkI3tmTz-kLEAIQDQ";
    public static string Mastery_Multicolor     = "CgkI3tmTz-kLEAIQDg";
    public static string Mastery_Commitment     = "CgkI3tmTz-kLEAIQEA";
    public static string Mastery_Curiousness    = "CgkI3tmTz-kLEAIQEQ";
    public static string Supporter              = "CgkI3tmTz-kLEAIQDw";

    public static int Mastery_Rotation_Needed       = 500;
    public static int Mastery_Fusion_Needed         = 100;
    public static int Mastery_Limitations_Needed    = 75;
    public static int Mastery_Multicolor_Needed     = 150;
    public static int Mastery_Commitment_Needed     = 50;
    public static int Equilibrium_Stage1_Needed     = 30;
    public static int Equilibrium_Stage2_Needed     = 60;
    public static int Equilibrium_Stage3_Needed     = 90;
    public static int Equilibrium_Stage4_Needed     = 120;
}

public class GooglePlayServiceHelper : MonoBehaviour {

    private bool _authenticated = false;

	void Awake () {
        InitializeGooglePlayGames();
	}

    public bool IsAuthenticated() {
        return Social.localUser.authenticated;
    }

    public bool AttemptToConnectUser() {
        if (IsAuthenticated()) return true; 
        Social.localUser.Authenticate((bool success) => {
            if (success) {
                Debug.Log("Successfully authenticated local user");
            } else {
                Debug.Log("Failed to authenticate local user");
            }
        });
        return IsAuthenticated();
    }

    public void SignOutUser() {
        #if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS)) 
            if (IsAuthenticated()) {
                ((GooglePlayGames.PlayGamesPlatform)Social.Active).SignOut();
            }
        #endif

    }

    public void ShowAchievementsUI() {
        if (!_authenticated) AttemptToConnectUser();
        if (!_authenticated) {
            Debug.Log("Failed to authenticate local user");
        } else {
            Debug.Log("Success: " + Social.localUser.userName);
            Social.LoadAchievements(null);
            Social.ShowAchievementsUI();
            PlayerPrefs.SetInt("GamecenterAutoAuthenticate", 1);
        }
    }

    public void UnlockAchievement(string id) {
        Social.ReportProgress(id, 100.0f, (bool success) => {
            if (success) {
                Debug.Log("Achivement unlocked : " + id);
            } else {
                Debug.Log("Failed to Unlock : " + id);
            }
        });
    }

    private void InitializeGooglePlayGames() {
        #if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS)) 
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
                // enables saving game progress.
                //.EnableSavedGames()
                .Build();

            PlayGamesPlatform.InitializeInstance(config);
            // recommended for debugging:
            PlayGamesPlatform.DebugLogEnabled = true;
            // Activate the Google Play Games platform
            PlayGamesPlatform.Activate();

            AttemptToConnectUser();
        #endif
    }

}
