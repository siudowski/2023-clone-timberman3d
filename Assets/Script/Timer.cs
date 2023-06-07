using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Timer : MonoBehaviour
{
    Action timerCallback;
    float timer;

    public float TimerSet { get; private set; }
    public float TimerRemaining { get { return timer; } }
    public bool IsPaused { get; set; }

    public void SetTimer(float timeInSeconds, Action callback)
    {
        timer = timeInSeconds;
        timerCallback = callback;
        TimerSet = timeInSeconds;
        IsPaused = false;
    }

    private void Update()
    {
        if (!IsPaused)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;

                if (IsTimerComplete())
                    timerCallback();
            }
        }
    }

    public bool IsTimerComplete()
    {
        return timer <= 0;
    }
}