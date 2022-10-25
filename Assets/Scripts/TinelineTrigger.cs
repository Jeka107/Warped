using UnityEngine;
using UnityEngine.Playables;

public class TinelineTrigger : MonoBehaviour
{
    [SerializeField] public GameObject timeline;

    private PlayableDirector director;
    private GameObject player;
    private GameObject canvas;

    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>().gameObject;
        canvas = FindObjectOfType<GamePlayCanvas>().gameObject;
        director = timeline.GetComponent<PlayableDirector>();
        director.played += Played;
        director.stopped += Stoped;
    }
    private void Played(PlayableDirector ctx)//if time line is playing de activate player's movement,animation and canvas.
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
            GetComponent<Collider>().enabled = false;
            canvas.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Player")//on trigger with player playr timeline.
        {
            director.Play();
        }
    }
}
