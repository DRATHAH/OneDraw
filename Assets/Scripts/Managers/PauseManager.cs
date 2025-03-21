using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    bool isPaused = false;
    GameManager gameManager;

    [Tooltip("If the player can pause the game or not.")]
    public bool pausable = true;
    public GameObject pauseMenu;
    public GameObject settingsMenu;
    public GameObject pauseButtonsMenu;

    private void Start()
    {
        gameManager = GameManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    public void Pause()
    {
        if (pausable)
        {
            if (isPaused)
            {
                isPaused = false;
                gameManager.SetCursorLock(true);
                gameManager.SetPlayerMove(true);
                gameManager.ResumeTime();
                settingsMenu.SetActive(false);
                pauseButtonsMenu.SetActive(true);
                pauseMenu.SetActive(false);
            }
            else
            {
                isPaused = true;
                gameManager.SetCursorLock(false);
                gameManager.SetPlayerMove(false);
                gameManager.PauseTime();
                pauseMenu.SetActive(true);
            }
        }
    }

    public void CanPause(bool state)
    {
        pausable = state;
    }
}
