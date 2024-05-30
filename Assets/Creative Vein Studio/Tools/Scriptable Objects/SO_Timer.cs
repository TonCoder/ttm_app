﻿using System.Text;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "Timer", menuName = "CVeinStudio/Game System/Timer")]
public class SO_Timer : ScriptableObject
{
    private float _elapsedTime = 0;
    private float seconds;

    private StringBuilder _stringBuilder;

    internal void TickTime(TextMeshProUGUI textMesh)
    {
        _elapsedTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(_elapsedTime / 60F);
        seconds += Time.deltaTime;

        if (seconds >= 60)
        {
            seconds = 0;
        }

        textMesh.text = $"{minutes:00}:{seconds:00}";
    }
}