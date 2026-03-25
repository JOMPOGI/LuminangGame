using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GoToLoginScene : MonoBehaviour
{
    public float transitionDelay = 0.4f;

    public void LoadLoginScene()
    {
        StartCoroutine(LoadSceneWithDelay());
    }

    private IEnumerator LoadSceneWithDelay()
    {
        yield return new WaitForSeconds(transitionDelay);
        SceneManager.LoadScene("LoginScene");
    }
}