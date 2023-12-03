using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleFeedback : MonoBehaviour
{
    public Text feedbackText; // Reference to the UI Text element
    public int maxLogMessages = 10; // Maximum number of log messages to display

    private List<string> logMessages = new List<string>();

    private void Awake()
    {
        // Redirect Unity's Debug.Log messages to our custom function
        Application.logMessageReceived += HandleLog;
    }

    private void Update()
    {
        // Update the UI Text with the stored log messages
        feedbackText.text = string.Join("\n", logMessages);
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Append the log message to the list of log messages
        logMessages.Add(logString);

        // Limit the number of log messages
        if (logMessages.Count > maxLogMessages)
        {
            int overflow = logMessages.Count - maxLogMessages;
            logMessages.RemoveRange(0, overflow);
        }
    }
}
