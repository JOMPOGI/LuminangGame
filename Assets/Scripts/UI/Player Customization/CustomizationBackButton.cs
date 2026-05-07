using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CustomizationBackButton : MonoBehaviour
{
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnBackClicked);
    }

    private void OnBackClicked()
    {
        SceneNavigationManager.ReturnToPreviousScene();
    }
}
