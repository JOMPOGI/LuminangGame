using UnityEngine;
using System;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("Time Settings")]
    [Tooltip("Starting time in hours (0-24)")]
    public float startingTime = 8.0f; 
    
    [Tooltip("How many real-world seconds equal one in-game hour")]
    public float realSecondsPerHour = 50f; 

    public float CurrentTimeOfDay { get; private set; }
    
    // Time states
    public bool IsMorning => CurrentTimeOfDay >= 6f && CurrentTimeOfDay < 12f;
    public bool IsAfternoon => CurrentTimeOfDay >= 12f && CurrentTimeOfDay < 18f;
    public bool IsEvening => CurrentTimeOfDay >= 18f || CurrentTimeOfDay < 6f;

    public Action<float> OnTimeChanged;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        CurrentTimeOfDay = startingTime;
    }

    void Update()
    {
        // Advance time
        float hoursPassed = Time.deltaTime / realSecondsPerHour;
        CurrentTimeOfDay += hoursPassed;
        
        if (CurrentTimeOfDay >= 24f)
        {
            CurrentTimeOfDay %= 24f; // wrap around
        }
        
        OnTimeChanged?.Invoke(CurrentTimeOfDay);
    }
    
    public string GetTimeString()
    {
        int hours = Mathf.FloorToInt(CurrentTimeOfDay);
        int minutes = Mathf.FloorToInt((CurrentTimeOfDay - hours) * 60f);
        
        string amPm = hours < 12 ? "AM" : "PM";
        int displayHours = hours % 12;
        if (displayHours == 0) displayHours = 12;
        
        return $"{displayHours:00}:{minutes:00} {amPm}";
    }

    public string GetGreetingTag()
    {
        if (IsMorning) return "Naimbag a bigat";
        if (IsAfternoon) return "Naimbag a malem";
        return "Naimbag a rabii";
    }
}
