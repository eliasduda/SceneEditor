using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Outputs the Console to a UI Text for Debug purposes
/// </summary>
public class DebugDisplay : MonoBehaviour
{
    Dictionary<string, string> debugLogs = new Dictionary<string, string>();
    public TextMeshProUGUI display;
    public bool continuous;

    private void Update()
    {
        if (!continuous)
        {
            Debug.Log("time:" + Time.time);
            Debug.Log(gameObject.name);
        }
    }

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (!continuous)
        {
            if (type == LogType.Log)
            {
                string[] splitString = logString.Split(char.Parse(":"));
                string debugKey = splitString[0];
                string debugValue = splitString.Length > 1 ? splitString[1] : "";

                if (debugLogs.ContainsKey(debugKey))
                    debugLogs[debugKey] = debugValue;
                else
                    debugLogs.Add(debugKey, debugValue);

            }

            string displayText = "";
            foreach (KeyValuePair<string, string> log in debugLogs)
            {
                if (log.Value == "")
                    displayText += log.Key + "\n";
                else
                    displayText += log.Key + ": " + log.Value + "\n";
            }

            display.text = displayText;
        }
        else
        {
            display.text = display.text + "\n" + logString ;
        }
    }
}