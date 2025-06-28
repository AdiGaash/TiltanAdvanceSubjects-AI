using UnityEngine;

[CreateAssetMenu(fileName = "New Sabotage Quest", menuName = "Quests/Sabotage Quest")]
public class SabotageQuestSO : QuestSO
{
    [Header("Sabotage Objective Settings")]
    public string itemType;       // e.g., "HealingHerb"
    public string targetOwner;    // e.g., "Player" or "FactionA"
    public bool destroyInsteadOfSteal = true;

    private void OnEnable()
    {
        goalType = QuestGoalType.SabotageObjective;
    }
}