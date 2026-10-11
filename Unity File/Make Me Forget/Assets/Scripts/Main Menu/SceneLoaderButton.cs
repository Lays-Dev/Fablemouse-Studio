using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoaderButton : MonoBehaviour
{
    public string sceneName;
    //Make sure sceneName matches scene file name exactly
    public void OnClick()
    {
        Debug.Log("Loading scene: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

}
