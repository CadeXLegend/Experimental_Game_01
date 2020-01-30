using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeOptions : MonoBehaviour
{
    private int targetFrameRate { get; set; } = 60;
    public int TargetFrameRate { get; }
    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = TargetFrameRate;
    }
}
