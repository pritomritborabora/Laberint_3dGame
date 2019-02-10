using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class restartButton : MonoBehaviour {
    public GameObject LoadingScreen;
    public Slider slider;
    public GameObject Chat;
	public void restartGame() {
			Debug.Log("Restarting");
        //Debug.Break();
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
            StartCoroutine(loadGame());
     
    }

	public void OnClick() {
			Debug.Log("Restarting clic");
			Debug.Break();
  			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
	public void OnMouseDown() {
			Debug.Log("Restarting mousedown");
			Debug.Break();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void PlayGame()
    {
        StartCoroutine(loadGame());
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void BacktoMain()
    {
        SceneManager.LoadScene(0);
    }
    public void Resume()
    {
        this.gameObject.SetActive(false);
        Chat.SetActive(true);
    }
    IEnumerator loadGame()
    {
       AsyncOperation operation =  SceneManager.LoadSceneAsync(1);
        LoadingScreen.SetActive(true);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            Debug.Log(progress);
            slider.value = progress;
            yield return null;
        }
    }
}
