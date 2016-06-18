using UnityEngine;
using System.Collections;

//Simple class used to put on return to menu button. Used to avoid losing reference to level manager in scene since the button can be destroyed on level load and not manager.
public class Button_ReturnToMenu : MonoBehaviour {

	public void OnPressed(){
        LevelManager.Instance.OnClick_BackToMenu();
    }
}
