using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest {
    // Quest name
    [SerializeField] public string QuestName { get; set; }
    // Quest id
    [SerializeField] public int QuestID { get; set; }
    // Quest description (shows in Q Log before start)
    [SerializeField] public string QuestShortDesc { get; set; }

    // Quest stages. Private method returns the currently filled-out stage numbers.
    [SerializeField] public List<QuestStage> QuestStages = new List<QuestStage>();
    private List<int> StageNumbers {
        get 
        {
            List<int> _list = new List<int>();
            for(int i = 0; i < QuestStages.Count; i++)
            {
                _list.Add(QuestStages[i].StageNumber);
            }

            return _list;
        }
    }

    // Quest requirements (skills + levels)
    [SerializeField] public List<int> QuestRequirements = new List<int>();
    // Quest rewards
    [SerializeField] public List<int> QuestItemRewards = new List<int>();
    [SerializeField] public int QuestCurrencyReward;


    // Empty constructor
    public Quest()
    { 

    }

    // Constructor for duplication
    public Quest(Quest q)
    {
        this.QuestName = q.QuestName;
        this.QuestID = q.QuestID;
        this.QuestShortDesc = q.QuestShortDesc;
        this.QuestRequirements = q.QuestRequirements;
        this.QuestStages = q.QuestStages;
        this.QuestCurrencyReward = q.QuestCurrencyReward;
        this.QuestItemRewards = q.QuestItemRewards;
    }

    public void AddQuestStage(QuestStage stage) {
        // Checks to see if the quest already has a QuestStage assigned to the stage number.
        if(StageNumbers.Contains(stage.StageNumber))
        {
            Debug.LogError(string.Format("Quest '{0}' already contains a stage at number {1}", QuestName, stage.StageNumber));
            return;
        }
        else
        {
            QuestStages.Add(stage);
        }
    }

    public void AddItemReward(int _id) {
        QuestItemRewards.Add(_id);
    }
}
