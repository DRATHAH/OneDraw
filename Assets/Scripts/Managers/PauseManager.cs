using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    bool isPaused = false;

    public GameManager gameManager;
    public GameObject pauseMenu;
    public GameObject settingsMenu;
    public GameObject pauseButtonsMenu;

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
        if (isPaused)
        {
            isPaused = false;
            gameManager.SetCursorLock(true);
            gameManager.SetPlayerMove(true);
            gameManager.ResumeTime();
            settingsMenu.SetActive(false);
            pauseButtonsMenu.SetActive(true);
            pauseMenu.SetActive(false);
            StartCoroutine(AllowShoot());
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

    IEnumerator AllowShoot()
    {
        yield return new WaitForSeconds(.01f);
    }
}
