﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour {

    public static DialogueManager Instance;

    public bool showNonTutorialText = false;
    List<string> possibleMessages = new List<string>();

    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void OnDestroy() {
        if (Instance == this) {
            Instance = null;
        }
    }

    public void TalkAboutLevel() {
        if (showNonTutorialText) {
            StartCoroutine(TalkAboutLevel_Coroutine());
        }
    }

    public void StopTalkCoroutines() {
        StopAllCoroutines();
    }


    IEnumerator TalkAboutLevel_Coroutine() {

        possibleMessages.Clear();
        GenerateTips();

        while (true) {

            yield return new WaitForSeconds(15f);

            float messageCooldown = 0f;

            switch (LevelManager.Instance.currentSelectedWorld) {
                default:
                    switch (LevelManager.Instance.currentSelectedLevel) {
                        default:
                            messageCooldown = TalkManager.Instance.WriteMessage(GetRandomTip());
                            break;
                    }
                    break;
            }

            messageCooldown += 15f;

            yield return new WaitForSeconds(messageCooldown);
        }
    }

    private string GetRandomTip() {
        return possibleMessages[Random.Range(0,possibleMessages.Count)];
    }

    private void GenerateTips() {

        AddDefaultTips();
        if (LevelManager.Instance.isInverseWorld()) AddInverseWorldTips();
        else AddNormalWorldTips();

        switch (LevelManager.Instance.currentSelectedWorld) {
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
        }
    }

    private void AddTips_World1() {
        possibleMessages.Add("Two orbs of the same color will result in a fusion of that same color.");
        possibleMessages.Add("Two different colors in the fusion socket will result in the third color.");
    }

    private void AddDefaultTips() {
        possibleMessages.Add("Take your time, we're in no hurry.");
        possibleMessages.Add("You can always return to a level later.");
        possibleMessages.Add("Try and beat the level with the least amount of moves possible.");
        possibleMessages.Add("You can always reset the level to its origin position by using the button at the top right corner.");
        possibleMessages.Add("The optimal amount of moves to complete in a level is written in the top right corner.");
        possibleMessages.Add("Your progress on each level it represented by the color of the orb in the level select.");
        possibleMessages.Add("The solution is often simplet than it first appears.");
        possibleMessages.Add("You can return to the level select by using the button in the top left corner.");
        possibleMessages.Add("All progress is saved after the completion of a level.");
    }

    private void AddNormalWorldTips() {
        possibleMessages.Add("Complete all levels in a world to unlock its inverse version.");
    }

    private void AddInverseWorldTips() {
        possibleMessages.Add("Two multicolored orbs in a single tri will always result in a balanced tri.");
    }


}
