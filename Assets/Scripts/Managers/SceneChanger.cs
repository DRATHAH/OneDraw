using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public Animator transition;

    private void Start()
    {
        transition.gameObject.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ChangeScene(string sceneName)
    {
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.SaveInventory(Inventory.instance);
        }
        StartCoroutine(LoadAsync(sceneName));
    }

    public void TutorialCheck()
    {
        if (PlayerStats.Instance && PlayerStats.Instance.tutorialComplete)
        {
            ChangeScene("Rest");
        }
        else
        {
            ChangeScene("Tutorial");
        }
    }

    IEnumerator LoadAsync(string levelName)
    {
        // Start transition animation
        transition.SetTrigger("Start");

        // Get info of currently playing animation clip
        AnimatorClipInfo[] clipInfo;
        clipInfo = transition.GetCurrentAnimatorClipInfo(0);

        // Wait for transition to finish
        yield return new WaitForSeconds(clipInfo.Length + 0.5f);

        // Wait until scene is fully loaded before switching to it
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelName);
        while (!operation.isDone)
        {
            // If we want to have a loading bar, we can set its progress using this variable
            float progress = Mathf.Clamp01((float)operation.progress);

            // Code to increase loading bar value

            // To not overwhelm the code while in the while statement
            yield return null;
        }
    }

    public void RestartScene()
    {
        string scene = SceneManager.GetActiveScene().name;
        StartCoroutine(LoadAsync(scene));
    }

}
