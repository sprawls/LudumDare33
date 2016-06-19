using UnityEngine;
using System.Collections;

//Simple class used to put on Inverse Button.
//Used to avoid losing reference to level manager in scene since the button can be destroyed on level load and not manager.
public class Button_InverseWorld : MonoBehaviour {

    public void OnPressed() {
        LevelManager.Instance.OnClick_InverseWorld();
    }
}
