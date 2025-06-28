using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dynamically generates counter-quests based on current active quests.
/// </summary>
public class DynamicQuestGenerator : MonoBehaviour
{
    public List<QuestSO> activeQuests = new List<QuestSO>();

    public void GenerateCounterQuests()
    {
        foreach (var quest in activeQuests)
        {
            if (quest == null) continue;

            switch (quest.goalType)
            {
                case QuestGoalType.KillEnemies:
                    CreateProtectEnemyQuest(quest);
                    break;

                case QuestGoalType.CollectItems:
                    CreateStealItemQuest(quest);
                    break;

                default:
                    Debug.Log($"[QuestGenerator] No counter logic for goal type: {quest.goalType}");
                    break;
            }
        }
    }

    private void CreateProtectEnemyQuest(QuestSO targetQuest)
    {
        var counterQuest = ScriptableObject.CreateInstance<ProtectQuestSO>();
        var killQuest = targetQuest as KillQuestSO;
        if (killQuest != null)
        {
            counterQuest.title = $"Defend {killQuest.enemyType}";
            counterQuest.description = $"Prevent the player from killing the {killQuest.enemyType}!";
            counterQuest.enemyType = killQuest.enemyType;
            counterQuest.rewardXP = 150;
            Debug.Log($"[QuestGenerator] Created counter-quest to protect: {killQuest.enemyType}");
        }
        else
        {
            Debug.LogError("Couldnt create Protect Quest");
        }
        
        // Optionally: Add to a quest manager here
    }

    private void CreateStealItemQuest(QuestSO targetQuest)
    {
       
        
    }
}
