using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class TimeLinePlayer : MonoBehaviour
{
    [SerializeField] private float waitTime;

    private GameObject canvas;
    private GameObject player;
    private PlayableDirector director;

    private bool played=false;

    private void Awake()
    {
        canvas = FindObjectOfType<GamePlayCanvas>()?.gameObject;
        player = FindObjectOfType<PlayerMovement>()?.gameObject;
        director = GetComponent<PlayableDirector>();
        director.played += Played;
        director.stopped += Stoped;
    }
    private void Played(PlayableDirector ctx) //if time line is playing de activate player's movement,animation and canvas.
    {
        if (player && canvas)
        {
            player.GetComponent<PlayerMovement>().DisableAnimation();
            player.GetComponent<PlayerMovement>().enabled = false;
            canvas.SetActive(false);
        }
    }
    private void Stoped(PlayableDirector ctx)//if time line is stoped activate player's movement,animation and canvas.
    {
        if (player && canvas)
        {
            player.GetComponent<PlayerMovement>().enabled = true;
            canvas.SetActive(true);
        }
        played = true;
    }
    public void StartTimeline()//play
    {
        StartCoroutine(PlayTimeline());
    }
    IEnumerator PlayTimeline()//player after time
    {
        yield return new WaitForSeconds(waitTime);
        director.Play();
    }
    public bool GetTimeLineStatus()
    {
        return played;
    }
}
