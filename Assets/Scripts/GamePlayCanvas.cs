using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GamePlayCanvas : MonoBehaviour
{
    [SerializeField] private GameObject pressEText;
    [SerializeField] private GameObject pressQText;


    [Header("Quest")]
    [SerializeField] private QuestManager questManager;
    [SerializeField] private GameObject ToolTip;
    [SerializeField] private GameObject ToolTipImage;
    
    [HideInInspector] public bool checkE = false;

    void Start()
    {
        pressEText.SetActive(false);
    }

    public void ShowPressEText()
    {
        pressEText.SetActive(true);
        checkE = true;
    }
    public void HidePressEText()
    {
        pressEText.SetActive(false);
        checkE = false;
    }
    public void ShowPressQText()
    {
        pressQText.SetActive(true);
    }
    public void HidePressQText()
    {
        pressQText.SetActive(false);
    }

    public void ShowQuestPressText()
    {
        var quest = questManager?.quest;

        if (string.IsNullOrWhiteSpace(quest.toolTip))
            ToolTipImage.SetActive(false);
        else
        {
            ToolTip.SetActive(true);
            ToolTipImage.SetActive(true);
        }
        ToolTip.GetComponent<TextMeshProUGUI>().text = quest?.toolTip;
        
        if(quest.button==KeyCode.E)
        {
            checkE = true;
        }
    }
    public void HideQuestPressText()
    {
        ToolTip.SetActive(false);
        ToolTipImage.SetActive(false);
        ToolTip.GetComponent<TextMeshProUGUI>().text = "";

        checkE = false;
    }
}
