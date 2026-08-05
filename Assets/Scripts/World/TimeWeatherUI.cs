using UnityEngine;
using TMPro;

public class TimeWeatherUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI timeText;

    void Update()
    {
        if (TimeManager.Instance != null && timeText != null)
        {
            timeText.text = TimeManager.Instance.GetTimeString();
        }
    }
}
