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
    void LateUpdate() // Makes sure it runs after the Update method in player shoot, so the player doesn't start drawing an arrow the moment interactio ends
    {
        if (messageText.text.Contains("hide"))
        {
            speakerText.text = " ";
            messageText.text = " ";
            visible.SetActive(false);
        }

        if (Input.GetMouseButtonDown(0) && visible.activeSelf) // When the player clicks and text is currently displayed
        {
            NextText();
        }
    }

    public void NextText()
    {
        if (dialogueManager && dialogueManager.CanForward() && subtitles.finished) // If text has finished typing and there's more to show, display the next text
        {
            GameManager.instance.SetCursorLock(false);
            dialogueManager.GetContinueState();
            SetText(dialogueManager.speaker, dialogueManager.ReturnText());
        }
        else if (dialogueManager && subtitles.finished) // Run any necessary events after the player is done reading text 
        {
            dialogueManager.CheckEvents(dialogueManager.textType);
        }
        else if (dialogueManager && !subtitles.finished) // If the text is still being typed, fast-forward the text to display it all
        {
            FinishText(dialogueManager.speaker, dialogueManager.ReturnText());
        }
        else if (dialogueManager) // Hide the text once it is finished
        {
            speaker.HideText(speakerText);
            subtitles.HideText(messageText);
        }
    }

    public void SetText(string speak, string msg) // Display next text method
    {
        visible.SetActive(true);
        speaker.AddWriter(speakerText, speak, time, true);
        subtitles.AddWriter(messageText, msg, time, true);
    }

    public void FinishText(string speak, string msg) // Skip text method
    {
        visible.SetActive(true);
        speaker.SetDialogue(speakerText, speak);
        subtitles.SetDialogue(messageText, msg);
    }
}