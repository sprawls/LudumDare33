using UnityEngine;
using System.Collections;

public class DialogueManager : MonoBehaviour {

    public static DialogueManager Instance;

    public bool showNonTutorialText = false;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            GameObject.DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    void OnDestroy() {
        if (Instance == this) {
            Instance = null;
        }
    }

    public void TalkAboutLevel(int currentLevel) {
        if (showNonTutorialText) {
            StartCoroutine(TalkAboutLevel_Coroutine(currentLevel));
        }
    }

    public void StopTalkCoroutines() {
        StopAllCoroutines();
    }


    IEnumerator TalkAboutLevel_Coroutine(int currentLevel) {
        yield return new WaitForSeconds(2.5f);

        float messageCooldown = 0f;
        switch (currentLevel) {
            case 1:
                messageCooldown = TalkManager.Instance.WriteMessage("Equilibrium is necessary.");
                break;
            case 2:
                messageCooldown = TalkManager.Instance.WriteMessage("Simply put, clearing a cluster removes all its free energy.");
                break;
            case 4:
                messageCooldown = TalkManager.Instance.WriteMessage("Sometimes energy can fuse. The result from the fuse is the element you must consider.");
                yield return new WaitForSeconds(messageCooldown + 3.5f);
                messageCooldown = TalkManager.Instance.WriteMessage("Two identical orbs will produce the same element while two different will produce a new one.");
                break;
            case 5:
                messageCooldown = TalkManager.Instance.WriteMessage("It gets harder from here on out.");
                break;
            case 6:
                messageCooldown = TalkManager.Instance.WriteMessage("We're getting closer. I feel it.");
                break;
            case 7:
                messageCooldown = TalkManager.Instance.WriteMessage("Once this is over, you'll have to go back there.");
                break;
            case 8:
                messageCooldown = TalkManager.Instance.WriteMessage("No need to feel bad. We'll survive.");
                break;
            case 9:
                messageCooldown = TalkManager.Instance.WriteMessage("That's a big one.");
                break;
            case 10:
                messageCooldown = TalkManager.Instance.WriteMessage("This is the last one. Once this cluster reaches equilibrium, so will the universe. ");
                yield return new WaitForSeconds(messageCooldown + 3.5f);
                messageCooldown = TalkManager.Instance.WriteMessage("Everything will stop. Maximum Entropy. ");
                yield return new WaitForSeconds(messageCooldown + 12f);
                messageCooldown = TalkManager.Instance.WriteMessage("Come on, you can do it.");
                yield return new WaitForSeconds(messageCooldown + 8f);
                messageCooldown = TalkManager.Instance.WriteMessage("You never had any doubt before. ");
                yield return new WaitForSeconds(messageCooldown + 12f);
                messageCooldown = TalkManager.Instance.WriteMessage("We are waiting for you.");
                yield return new WaitForSeconds(messageCooldown + 18f);
                messageCooldown = TalkManager.Instance.WriteMessage("Come on, This is the last one.");
                yield return new WaitForSeconds(messageCooldown + 40f);
                messageCooldown = TalkManager.Instance.WriteMessage("I Mean it, it really is... ");
                yield return new WaitForSeconds(messageCooldown + 100f);
                messageCooldown = TalkManager.Instance.WriteMessage("Still there ? Really ?");
                break;
        }
        messageCooldown += 15f;

        yield return new WaitForSeconds(messageCooldown);

        WriteRandomTip();
    }

    private void WriteRandomTip() {
        string RandomMessage = "";
        switch (Random.Range(0, 8)) {
            case 0:
                RandomMessage = "Take your time, we're in no hurry.";
                break;
            case 1:
                RandomMessage = "Stability will happen regardless. We're just accelerating the process.";
                break;
            case 2:
                RandomMessage = "Still rusty, huh.";
                break;
            case 3:
                RandomMessage = "You can always reset the cluster to its origin position by using the button at the top right corner.";
                break;
            case 4:
                RandomMessage = "The optimal amount of moves to reach equilibrium in a cluster is written in the top left corner.";
                break;
            case 5:
                RandomMessage = "Your score is the amount of moves above the par solution used. The lower the better. ";
                break;
            case 6:
                RandomMessage = "Keep going, we haven't woken you up for nothing.";
                break;
            case 7:
                RandomMessage = "What's wrong ? You've done that plenty of times.";
                break;
        }

        TalkManager.Instance.WriteMessage(RandomMessage);
    }
}
