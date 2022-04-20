using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerObject : MonoBehaviour
{
    public TMP_Text text;

    private bool timerOn = false;
    private float seconds = 0f;

    void Update()
    {
        if (timerOn)
        {
            this.seconds += Time.deltaTime;
        }
        float minutes = Mathf.FloorToInt(this.seconds / 60);
        float seconds = Mathf.FloorToInt(this.seconds % 60);
        text.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void TimerOn()
    {
        timerOn = true;
    }

    public void TimerOff()
    {
        timerOn = false;
    }
}
