using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameUtilities
{
    public static class NPCUtility 
    {

    }

    /// <summary>
    /// Types of dialog stages.
    /// </summary>
    public enum NPCDialogType
    {
        Dialog,
        Player_ReceiveItem, Player_GiveItem,
        Quest_Start, Quest_Advance, Quest_Finish
    }
}

