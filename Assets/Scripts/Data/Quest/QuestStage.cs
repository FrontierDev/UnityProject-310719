using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtilities;

[System.Serializable]
public class QuestStage  {
    // Quest Stage name
    [SerializeField] public string StageName { get; set; }
    // A short description to show in the quest log.
    [SerializeField] public string ShortDescription { get; set; }
    // Quest Stage description (appears in the Q Log when the stage is NOT finished)
    [SerializeField] public string ProgressDescription { get; set; }
    // Quest stage description when FINISHED
    [SerializeField] public string FinishedDescription { get; set; }
    // Quest Stage number (the order it appears in the quest log)
    [SerializeField] public int StageNumber { get; set; }
    // Quest stage type
    [SerializeField] public QuestStageType StageType { get; set; }
    // Quest stage target NPCs/Items
    [SerializeField] public List<int> StageTargetID { get; set; }
    [SerializeField] public List<int> StageTargetQuantity { get; set; }

    // Empty constructor
    public QuestStage()
    {

    }

    public QuestStage(string _name, string _short, string _progDesc, string _finDesc, int _num, QuestStageType _type) {
        this.StageName = _name;
        this.ShortDescription = _short;
        this.ProgressDescription = _progDesc;
        this.FinishedDescription = _finDesc;
        this.StageNumber = _num;
        this.StageType = _type;
    }

    public void AddTargets(List<int> targetIDs, List<int> quantities)
    {
        StageTargetID = targetIDs;
        StageTargetQuantity = quantities;
        Debug.Log("Saved: " + StageTargetID.Count);
    }
}
