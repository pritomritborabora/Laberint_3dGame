using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class restartButtonOld : MonoBehaviour {

	public void restartGame(string sceneName) {
			Debug.Log("Restarting");
			Debug.Break();
  			Application.LoadLevel(sceneName);
	}

	public void OnClick() {
			Debug.Log("Restarting clic");
			Debug.Break();
  			Application.LoadLevel("NewMaze");
	}
	public void OnMouseDown() {
			Debug.Log("Restarting mousedown");
			Debug.Break();
  			Application.LoadLevel("NewMaze");
	}
}
