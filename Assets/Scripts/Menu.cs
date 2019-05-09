using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void Start() {
        #if UNITY_WEBGL
            var buttonExit = GameObject.Find("Button Exit");
            buttonExit.SetActive(false);
        #endif
    }
    public void OnPlayHandler()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void OnExitHandler()
    {
        Application.Quit();
    }
}