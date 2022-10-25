using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTrigger2B : MonoBehaviour
{
    [SerializeField] private QuestManager questManager;
    [SerializeField] private Quest quest;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Quest currentQuest = questManager.quest;

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
