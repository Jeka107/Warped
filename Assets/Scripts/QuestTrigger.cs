using UnityEngine;

public class QuestTrigger : MonoBehaviour
{
    [SerializeField] private PlayerInputs playerInputs;
    [SerializeField] private QuestManager questManager;
    [SerializeField] private Quest quest;

    private void Start()
    {
        GravityController.onGravityStatus += Action;
    }
    private void OnDestroy()
    {
        GravityController.onGravityStatus -= Action;
    }

    public void Action(bool status)
    {
        if (status)
        {
            playerInputs.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            playerInputs.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            Destroy(gameObject, 1f);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") //on collide with player activate quest.
        {
            Quest currentQuest = questManager.quest;
            other.GetComponentInParent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

            if (quest == currentQuest)
            {
                switch (currentQuest.goalType)
                {
                    case GOALTYPE.InputAction:
                        questManager.SetActive();
                        GetComponent<Collider>().enabled = false;
                        break;
                    case GOALTYPE.Trigger:
                        questManager.QuestComplete();
                        break;
                }
                
            }
        }
    }
}
