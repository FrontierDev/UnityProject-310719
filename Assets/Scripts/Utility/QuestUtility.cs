using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameUtilities
{
    public static class QuestUtility {

    }

    /// <summary>
    /// Types of quest stages.
    /// </summary>
    public enum QuestStageType {
        Dialog,
        Kill_Single, Kill_Multiple,
        GiveItem_Single, GiveItem_Multiple,
        CollectItem
    }
}
