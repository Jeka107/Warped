using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public delegate void OnQuestComplete();
    public static event OnQuestComplete onQuestComplete;

    [Header("Quest Data")]
    [SerializeField] private List<QuestData> listOfQuestData;
    [SerializeField] private bool isActive = false;

    [Header("QuestUI")]
    [SerializeField] private GameObject questWindow;
    [SerializeField] private GameObject questDescription;
    [SerializeField] private TextMeshProUGUI questText;

    [Header("Subtitles")]
    [SerializeField] private GameObject subtitles;
    [SerializeField] private TextMeshProUGUI subtitleText;

    [Space(10)]
    [SerializeField] private PlayerActions playerActions;
    [SerializeField] private GameManager gameManager;

    [HideInInspector] public Quest quest;
    [HideInInspector] public GameObject director;
    
    private int questNumber;
    private Dictionary<KeyCode, bool> explationForKey;

    private void Awake()
    {
        InitialTextExplationForKeyDict();

        if(gameManager.GetLevel() != 1) //check level
        {
            List<KeyCode> keys = new List<KeyCode>(explationForKey.Keys);

            //after level 1 all skills activated.
            foreach (KeyCode keyCode in keys)
            {
                explationForKey[keyCode] = true;
            }
        }
    }
    private void Start()
    {
        if(listOfQuestData.Count!=0)
        {
            questWindow.SetActive(true); //show quest window
        }
            

        if (listOfQuestData.Count != 0)
        {
            quest = listOfQuestData[questNumber].quest;  //first quest
            director = listOfQuestData[questNumber].timeline;
            questText.text = quest.description; //
        }

        onQuestComplete += UpdateQuest;
        playerActions.onPress += CheckQuest;
        playerActions.onQuest += QuestComplete;
    }

    private void OnDestroy()
    {
        onQuestComplete -= UpdateQuest;
        playerActions.onPress -= CheckQuest;
        playerActions.onQuest -= QuestComplete;
    }

    private void InitialTextExplationForKeyDict()
    {
        explationForKey = new Dictionary<KeyCode, bool>()
        {
            { KeyCode.E, false },
            { KeyCode.F, false },
            { KeyCode.Mouse0, false },
            { KeyCode.Space, true }
        };
    }

    private void CheckQuest(KeyCode button,GameObject hitObject)
    {
        if (explationForKey.ContainsKey(button)) //first time key used in correct quest set key to true.
        {
            explationForKey.TryGetValue(button, out bool isActive);

            if (isActive == false)
            {
                explationForKey[button] = true;
            }
        }

        Destroy(hitObject?.GetComponent<QuestObject>());
        QuestComplete();
    }

    public bool GetIfKeyIsActiveFirstTime(KeyCode button) //check key status if player can use it or not.
    {
        if (explationForKey.ContainsKey(button))
        {
            explationForKey.TryGetValue(button, out bool isActive);
            return isActive;
        }
        return true;
    }

    private void UpdateQuest() //update quest UI.
    {
        questText.text = quest.description;

        if (isActive)
        {
            questWindow.SetActive(true);

            if (string.IsNullOrWhiteSpace(quest.description))
                questDescription.SetActive(false);
            else
                questDescription.SetActive(true);
        }
    }


    public void QuestComplete()
    {
        if (director != null)
            director.GetComponent<TimeLinePlayer>()?.StartTimeline();//activate timeline when quest finished.

        subtitles.SetActive(true);
        subtitleText.text = quest.subtitleText;

        listOfQuestData[questNumber].isComplete = true;
        questWindow.SetActive(false);
        questNumber++;
        
        if (questNumber < listOfQuestData.Count) //check if next quest not null
        {
            quest = listOfQuestData[questNumber].quest; //get next quest
            director = listOfQuestData[questNumber].timeline;
            isActive = quest.isActive;
            onQuestComplete();                 //active event to update quest
        }
        else
        {
            questWindow.SetActive(false);
        }
    }
    public void SetActive()
    {
        isActive = true;
        UpdateQuest();
    }

    public QuestData ReturnIndexQuestData(int i)
    {
        if (i<listOfQuestData.Count-1)
            return listOfQuestData[i];
        return null;
    }
}
