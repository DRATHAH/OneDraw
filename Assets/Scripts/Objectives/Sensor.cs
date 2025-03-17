using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sensor : MonoBehaviour
{
    [Tooltip("Whether player must enter or leave sensor to activate it")]
    public bool isEnterTrigger = true;
    [Tooltip("Event that runs when trigger is activated")]
    public UnityEvent sensorFunction;
    public DialogueManager.TextType textType;
    public int textInt = 0;
    public string speaker = "";

    bool hasSensed = false;

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.transform.root.GetComponent<PlayerMovement>();
        if (!hasSensed && player && isEnterTrigger)
        {
            hasSensed = true;
            sensorFunction.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerMovement player = other.transform.root.GetComponent<PlayerMovement>();
        if (!hasSensed && player && !isEnterTrigger)
        {
            hasSensed = true;
            sensorFunction.Invoke();
        }
    }

    public void SendText() // Use to set the actual text to this
    {
        string message = DialogueManager.instance.SetText(textInt, textType);
        SubtitleManager.instance.SetText(speaker, message);
    }

    public void SendTempText() // Use to just temporarily set the text
    {
        string message = DialogueManager.instance.ReturnTemporaryText(textInt, textType);
        SubtitleManager.instance.SetText(speaker, message);
    }
}
