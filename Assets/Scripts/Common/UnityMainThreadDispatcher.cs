using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PURPOSE: This script allows code running in the background (like Supabase authentication) 
/// to safely talk to Unity's main thread. Without this, Unity would crash or freeze 
/// when trying to update the UI or change scenes after a login.
/// 
/// HOW IT WORKS: It maintains a queue of actions and runs them during the Update() loop.
/// </summary>
public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action> _executionQueue = new Queue<Action>();
    private static UnityMainThreadDispatcher _instance = null;

    public void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void Update()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                _executionQueue.Dequeue().Invoke();
            }
        }
    }

    /// <summary>
    /// Schedules an action to be run on the Unity main thread.
    /// </summary>
    public static void Enqueue(Action action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(action);
        }
    }

    /// <summary>
    /// Ensures an instance of the dispatcher exists in the scene.
    /// </summary>
    public static void CheckInstance()
    {
        if (_instance == null)
        {
            var go = new GameObject("UnityMainThreadDispatcher");
            _instance = go.AddComponent<UnityMainThreadDispatcher>();
        }
    }
}
