using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    #region Singleton

    public static DialogueManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of DialogueManager found!");
            return;
        }
        instance = this;
    }

    #endregion

    public enum TextType
    {
        normal,
        cutscene,
    }

    [Header("Important")]
    public int textNum = 0;
    public string speaker;
    public TextType textType = TextType.normal;
    public bool canForward = true;

    [Tooltip("Lines of dialogue that run in numerical order.")]
    [TextArea]
    public List<string> normalLines = new List<string>();
    [Tooltip("Must contain same # of entries as normalLines. Leave unchecked to make a dialogue unskippable.")]
    public List<bool> normalContinue = new List<bool>();
    [Tooltip("Contains dialogue that runs an event. Type the textNum value of the dialogue to run an event.")]
    public List<int> normalEventPlay = new List<int>();
    [Tooltip("Events to run when textType is 'normal'. Runs in order according to normalEventPlay.")]
    public List<UnityEvent> normalEvents = new List<UnityEvent>();

    [Tooltip("Lines of dialogue that require conditions to be met or happen randomly.")]
    [TextArea]
    public List<string> cutsceneLines = new List<string>();
    [Tooltip("Must contain same # of entries as cutsceneLines. Leave unchecked to make a dialogue unskippable.")]
    public List<bool> cutsceneContinue = new List<bool>();
    [Tooltip("Contains dialogue that runs an event. Type the textNum value of the dialogue to run an event.")]
    public List<int> cutsceneEventPlay = new List<int>();
    [Tooltip("Events to run when textType is 'cutscene'. Runs in order according to normalEventPlay.")]
    public List<UnityEvent> cutsceneEvents = new List<UnityEvent>();

    Dictionary<string, bool> nLines = new Dictionary<string, bool>();
    Dictionary<int, UnityEvent> normalEventsList = new Dictionary<int, UnityEvent>();

    Dictionary<string, bool> cLines = new Dictionary<string, bool>();
    Dictionary<int, UnityEvent> cutsceneEventsList = new Dictionary<int, UnityEvent>();

    private void Start()
    {
        for (int i = 0; i < normalLines.Count; i++)
        {
            nLines.Add(normalLines[i], normalContinue[i]);
        }
        for (int i = 0; i < normalEventPlay.Count; i++)
        {
            normalEventsList.Add(normalEventPlay[i], normalEvents[i]);
        }
        for (int i = 0; i < cutsceneLines.Count; i++)
        {
            cLines.Add(cutsceneLines[i], cutsceneContinue[i]);
        }
        for (int i = 0; i < cutsceneEventPlay.Count; i++)
        {
            cutsceneEventsList.Add(cutsceneEventPlay[i], cutsceneEvents[i]);
        }
    }

    private string DetermineText(TextType type) // Gets the text according to textNum and textType
    {
        string returnText = "";

        if (type == TextType.normal)
        {
            if (textNum >= normalLines.Count)
            {
                textNum = normalLines.Count - 1;
            }
            returnText = normalLines[textNum];
        }
        else
        {
            if (textNum >= cutsceneLines.Count)
            {
                textNum = cutsceneLines.Count - 1;
            }
            returnText = cutsceneLines[textNum];
        }

        return returnText;
    }

    public string ReturnText() // Gets text message of the current text type
    {
        string returnText = DetermineText(textType);
        return returnText;
    }

    public string GetText(TextType type) // Gets text message of a specific text type
    {
        string returnText = DetermineText(type);

        return returnText;
    }

    public string ReturnTemporaryText(int num, TextType type) // Used for random prompts, like enemy outbursts
    {
        int ogNum = textNum;
        textNum = num;
        string returnText = DetermineText(type);

        textNum = ogNum;

        return returnText;
    }

    public string SetText(int num, TextType type) // Sets the text to a specific line and/or type
    {
        textNum = num;

        textType = type;

        string returnText = DetermineText(textType);
        return returnText;
    }

    public bool GetContinueState() // Checks whether text can be forwarded. Also increments textNum
    {
        bool state = false;
        if (textType == TextType.normal)
        {
            state = nLines[normalLines[textNum]];
        }
        else
        {
            state = cLines[cutsceneLines[textNum]];
        }

        textNum++;
        canForward = state;

        return state;
    }

    public bool CanForward() // Only returns if a line can be skipped or not
    {
        bool state = false;
        if (textType == TextType.normal)
        {
            state = nLines[normalLines[textNum]];
        }
        else
        {
            state = cLines[cutsceneLines[textNum]];
        }

        canForward = state;

        return state;
    }

    public void CheckEvents(TextType type)
    {
        if (type == TextType.normal)
        {
            if (normalEventsList.ContainsKey(textNum))
            {
                normalEventsList[textNum].Invoke();
            }
        }
        else
        {
            if (cutsceneEventsList.ContainsKey(textNum))
            {
                cutsceneEventsList[textNum].Invoke();
            }
        }
    }
}