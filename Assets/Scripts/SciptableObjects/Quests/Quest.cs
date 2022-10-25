using System.Collections;
using UnityEngine;

public enum GOALTYPE
{
    ActionOnObject,
    InputAction,
    ActionOnObjectWithItem,
    Kill,
    Gathering,
    Trigger
}

[CreateAssetMenu(menuName = "Quest Data")]
public class Quest : ScriptableObject
{
    [SerializeField] public bool isActive;
    [Space]
    [Header("Quest Data")]
    [TextArea(6, 6)]
    [SerializeField] public string description;
    [TextArea(6, 6)]
    [SerializeField] public string toolTip;
    [TextArea(6, 6)]
    [SerializeField] public string subtitleText;
    [SerializeField] public int expReward;

    [Header("Quest Goal")]
    [SerializeField] public GOALTYPE goalType;           //type of goal to complete the quest
    [SerializeField] public InventoryItemData itemToUse;

    [Space(10)]
    [Header("Action")]
    [SerializeField] public KeyCode button;

    [Space(10)]
    [Header("Kill")]
    [SerializeField] public int killRequiredAmount;

    [Space(10)]
    [Header("Gathering")]
    [SerializeField] public int gatheringRequiredAmount;
}


