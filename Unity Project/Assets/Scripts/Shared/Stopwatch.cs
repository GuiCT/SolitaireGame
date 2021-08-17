using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Stopwatch : MonoBehaviour
{
float currentTime;
    public Text TimeText;
    bool stopwatchActive;

    void Start()
    {
        StartStopwatch();
        currentTime = 0;
    }

    void Update()
    {
        TimeText.text = currentTime.ToString();
        
        if (stopwatchActive == true)
        {
            currentTime = currentTime + Time.deltaTime;
        }

        TimeSpan time = TimeSpan.FromSeconds(currentTime);

        if (currentTime < 3600)
        {
            TimeText.text = time.ToString(@"mm\:ss");
        } else
        {
            TimeText.text = time.ToString(@"hh\:mm\:ss");
        }
    }

    public void StartStopwatch()
    {
        stopwatchActive = true;
    }
    public void StopStopwatch()
    {
        stopwatchActive = false;
    }
}
