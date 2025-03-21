using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Subtitle : MonoBehaviour
{
    public bool isSpeaker = false;
    [Tooltip("Only needed for non-speaker text objects")]
    public GameObject continuePrompt;
    public bool finished = false;

    TMP_Text uiText;
    string textToWrite = "";
    int characterIndex;
    float timePerCharacter;
    float timer;
    bool invisibleCharacters;

    public void AddWriter(TMP_Text text, string toWrite, float time, bool invisChars)
    {
        if (toWrite.Equals("hide"))
        {
            HideText(text);
        }

        if (!textToWrite.Equals(toWrite) || !text.text.Contains(toWrite))
        {
            finished = false;

            if (!isSpeaker)
            {
                continuePrompt.SetActive(false);
            }
            uiText = text;

            textToWrite = toWrite;
            timePerCharacter = time;
            invisibleCharacters = invisChars;
            characterIndex = 0;
        }
    }

    public void SetDialogue(TMP_Text text, string toWrite)
    {
        if (!finished)
        {
            finished = true;
            uiText = null;
            text.text = toWrite;

            if (!isSpeaker)
            {
                continuePrompt.SetActive(true);
            }
        }
    }

    public void HideText(TMP_Text text)
    {
        finished = false;
        textToWrite = "";
        text.text = "hide";
    }

    // Update is called once per frame
    void Update()
    {
        if (uiText)
        {
            timer -= Time.deltaTime; // Start timer for when to move on to the next character

            while (timer <= 0f)
            {
                timer += timePerCharacter;
                characterIndex++;

                if (textToWrite.Length <= 0) // Check to see if all letters of textToWrite have been written
                {
                    uiText = null;
                    return;
                }
                else
                {
                    string text = textToWrite.Substring(0, characterIndex); // Adds part of textToWrite as visible characters
                    if (invisibleCharacters)
                    {
                        text += "<color=#00000000>" + textToWrite.Substring(characterIndex) + "</color>"; // Adds the rest of textToWrite as invisible characters
                    }
                    uiText.text = text;

                    if (characterIndex >= textToWrite.Length)
                    {
                        uiText = null;
                        if (!isSpeaker)
                        {
                            continuePrompt.SetActive(true);
                        }
                        finished = true;
                        return;
                    }
                }
            }
        }
    }
}