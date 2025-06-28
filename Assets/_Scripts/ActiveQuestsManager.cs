using System.Collections.Generic;
using UnityEngine;

public class ActiveQuestsManager : MonoBehaviour
{
    public List<ActiveQuest> activeRealtimeQuests = new List<ActiveQuest>();
    
    public void AddRealtimeQuest(QuestSO questSO, GameObject receiver)
    {
        ActiveQuest newQuest = null;

        if (questSO is ProtectQuestSO)  
        {
            var pq = new ProtectActiveQuest();
            pq.questSO = questSO;
            pq.questReceiver = receiver;
            pq.targetName = "Some Target"; 
            newQuest = pq;
        }
        else if (questSO is SabotageQuestSO)
        {
            var sq = new SabotageActiveQuest();
            sq.questSO = questSO;
            sq.questReceiver = receiver;
            sq.targetName = "Some Target";
            newQuest = sq;
        }

        if (newQuest != null)
        {
            newQuest.StartQuest();
            activeRealtimeQuests.Add(newQuest);
        }
    }
}
