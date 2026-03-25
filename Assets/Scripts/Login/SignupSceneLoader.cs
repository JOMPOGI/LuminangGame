using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GoToSignupScene : MonoBehaviour
{
    public float transitionDelay = 0.4f;

    public void LoadSignupScene()
    {
        StartCoroutine(LoadSceneWithDelay());
    }

    private IEnumerator LoadSceneWithDelay()
    {
        yield return new WaitForSeconds(transitionDelay);
        SceneManager.LoadScene("SignupScene");
    }
}