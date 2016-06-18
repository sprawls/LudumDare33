using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class TalkManager : MonoBehaviour {

    public static TalkManager Instance;

    private bool _isTalking;
    public bool isTalking { 
        get { return _isTalking; } 
        private set { 
            _isTalking = value;
            if (talkParticles != null) {
                talkParticles.emissionRate = 4;
                talkParticles.enableEmission = _isTalking;
            }
        }
    }

    private Text conversationText;
    private float charDelay = .05f;
    private ParticleSystem talkParticles;



    void Awake() {
        if (Instance == null) {
            Instance = this;
            gameObject.AddComponent<DialogueManager>();
        } else {
            Destroy(gameObject);
        }
    }

    void Start(){
        conversationText = GameObject.FindGameObjectWithTag("ConvoText").GetComponent<Text>();
        if (GameObject.FindGameObjectWithTag("TalkRings") != null) {
            talkParticles = GameObject.FindGameObjectWithTag("TalkRings").GetComponent<ParticleSystem>();
        }
        
    }

    void OnDestroy() {
        if (Instance == this) {
            Instance = null;
        }
    }

    public float WriteMessage(string newMessage) {
        StopAllCoroutines();
        StartCoroutine(TextAnim(newMessage));
        return newMessage.Length * charDelay;
    }

    public void FadeOutText() {
        conversationText.DOFade(0, 1f);
    }

    IEnumerator TextAnim(string Message) {
        isTalking = true;
        yield return new WaitForSeconds(0.2f);

        conversationText.text = "";
        conversationText.DOFade(1, 0.01f);

        for (int i = 0; i < Message.Length; i++) {
            conversationText.text += Message[i];
            yield return new WaitForSeconds(charDelay);
            conversationText.DOFade(1, 0.01f);
        }
        isTalking = false;

        yield return new WaitForSeconds(6f);
        FadeOutText();
    }

}
