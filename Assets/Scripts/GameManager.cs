using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static int level = 1;
    [SerializeField] AudioSource startButton;
    [SerializeField] GameObject headline;

    public void RestartScene()//restart current scene
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("pressed");
    }
    public void StartGame()//put on start button
    {
        startButton.Play();
        headline.SetActive(false);
        StartCoroutine(CheckTimelineFinished());
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Lobby");
    }
    IEnumerator CheckTimelineFinished()//when time line finished callin function to load next level.
    {
        var timeline = FindObjectOfType<TimeLinePlayer>();
        timeline.StartTimeline();

        if (timeline != null)
        {
            var status = timeline.GetTimeLineStatus();
            while (!status)
            {
                status = timeline.GetTimeLineStatus();
                yield return new WaitForEndOfFrame();
            }
            LoadNexTScene();
        }
    }

    public void LoadNexTScene()//load next scene
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void LoadScene(int level)//load a level
    {
        SceneManager.LoadScene(level);
    }

    public int GetLevel()
    {
        return level;
    }
    public void SetLevel()
    {
        level++;
    }
}
