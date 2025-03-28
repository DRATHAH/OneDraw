using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Singleton

    public static GameManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Inventory found!");
            return;
        }
        instance = this;
    }

    #endregion

    public PlayerMovement player;
    public Camera playerCamera;

    public void SetCursorLock(bool locked)
    {
        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void SetPlayerMove(bool canMove)
    {
        player.canMove = canMove;
    }

    public void PauseTime()
    {
        Time.timeScale = 0;
    }

    public void ResumeTime()
    {
        Time.timeScale = 1.0f;
    }

    public void SetSensitivity(Slider level)
    {

        player.sensitivity = level.value;
        PlayerStats.Instance.playerSensitivity = level.value;
    }

    public void SetFieldOfView(float level)
    {
        playerCamera.fieldOfView = (int)level;
    }

    public void CompletedTutorial()
    {
        PlayerStats.Instance.tutorialComplete = true;
    }
}
