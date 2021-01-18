using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameActionsLogger : MonoBehaviour
{
    [SerializeField]
    private int maxActions = 10;
    private string[] actionsLog;
    private int lastInserted;

    [SerializeField]
    private TMP_Text visualLog;

    #region Singleton
    public static GameActionsLogger instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log($"More than one {instance} instance found!");
            Destroy(this);
            return;
        }
        instance = this;
        visualLog.text = "";
        actionsLog = new string[maxActions];
        lastInserted = maxActions;
    }
    #endregion

    public void LogAction(string actionToLog)
    {
        if (lastInserted > 9)
            lastInserted = 0;
        actionToLog = actionToLog.Insert(0, $"[{Time.timeSinceLevelLoad}] ");
        string logDump = "";
        actionsLog[lastInserted] = actionToLog;
        lastInserted++;
        for(int i = 0; i < maxActions; ++i)
            logDump += $"{actionsLog[i]}\n";
        visualLog.text = logDump;
    }
}
