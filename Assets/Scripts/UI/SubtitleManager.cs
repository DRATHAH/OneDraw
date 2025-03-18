using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SubtitleManager : MonoBehaviour
{
    #region Singleton

    public static SubtitleManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of SubtitleManager found!");
            return;
        }
        instance = this;
    }

    #endregion

    [Header("Display")]
    [SerializeField] Subtitle speaker;
    [SerializeField] Subtitle subtitles;
    [SerializeField] TMP_Text speakerText;
    [SerializeField] TMP_Text messageText;
    [Tooltip("Parent object of the subtitles, allows all subtitles to be hidden easily")]
    [SerializeField] GameObject visible;
    DialogueManager dialogueManager;

    [Tooltip("Speed at which each character is typed in at")]
    public float time = 0.02f;


    // Start is called before the first frame update
    void Start()
    {
        dialogueManager = DialogueManager.instance;
        visible.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (messageText.text.Contains("hide"))
        {
            speakerText.text = "lol";
            messageText.text = "lol";
            visible.SetActive(false);
        }

        if (Input.GetMouseButtonDown(0) && visible.activeSelf)
        {
            NextText();
        }
    }

    public void NextText()
    {
        if (dialogueManager && dialogueManager.CanForward() && subtitles.finished)
        {
            PlayerStats.Instance.SetCursorLock(false);
            dialogueManager.GetContinueState();
            SetText(dialogueManager.speaker, dialogueManager.ReturnText());
        }
        else if (dialogueManager && !subtitles.finished)
        {
            FinishText(dialogueManager.speaker, dialogueManager.ReturnText());
        }
        else if (dialogueManager)
        {
            speaker.HideText(speakerText);
            subtitles.HideText(messageText);
        }
    }

    public void SetText(string speak, string msg)
    {
        visible.SetActive(true);
        speaker.AddWriter(speakerText, speak, time, true);
        subtitles.AddWriter(messageText, msg, time, true);
    }

    public void FinishText(string speak, string msg)
    {
        visible.SetActive(true);
        speaker.SetDialogue(speakerText, speak);
        subtitles.SetDialogue(messageText, msg);
    }
}