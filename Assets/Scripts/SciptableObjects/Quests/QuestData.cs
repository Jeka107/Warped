using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class QuestData
{
    [SerializeField] public Quest quest;
    [SerializeField] public GameObject timeline;
    [SerializeField] public bool isComplete;

    public QuestData(Quest _quest,GameObject _timeline)
    {
        quest = _quest;
        timeline = _timeline;
    }
}
